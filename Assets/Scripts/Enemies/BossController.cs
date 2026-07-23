using System.Collections;
using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 공통 상태, 페이즈, 패턴 실행 순서를 관리한다.
    /// 개별 공격은 BossPattern 컴포넌트로 분리해서 프리팹마다 조합한다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(EnemyController))]
    [RequireComponent(typeof(EnemyHealth))]
    public class BossController : MonoBehaviour
    {
        [SerializeField]
        private Transform player;

        [SerializeField]
        private float commonRecoveryTime = 0.8f;

        [SerializeField]
        private float phase2HealthRatio = 0.6f;

        [SerializeField]
        private float phase3HealthRatio = 0.3f;

        [SerializeField]
        private bool useDefaultEnemyMovement = true;

        private BossPattern[] patterns;
        private BossPattern currentPattern;
        private BossPattern lastPattern;
        private EnemyController enemyController;
        private EnemyHealth enemyHealth;
        private Rigidbody2D rb;
        private Coroutine patternRoutine;

        public Transform Player => player;
        public Rigidbody2D BossRigidbody => rb;
        public BossState State { get; private set; } = BossState.Chasing;
        public int CurrentPhase { get; private set; } = 1;
        public int BossStage { get; private set; } = 1;
        public float PatternCooldownMultiplier { get; private set; } = 1f;
        public bool IsDead => enemyHealth == null || enemyHealth.IsDead;
        public float HealthProgress => enemyHealth == null ? 0f : enemyHealth.HealthProgress;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyController = GetComponent<EnemyController>();
            enemyHealth = GetComponent<EnemyHealth>();
            patterns = GetComponents<BossPattern>();

            if (player == null)
                player = GameObject.Find("Player")?.transform;

            foreach (BossPattern pattern in patterns)
                pattern.Initialize(this);
        }

        private void Update()
        {
            if (IsDead || GameState.IsGameOver)
            {
                SetState(BossState.Dead, false);
                StopRunningPattern();
                return;
            }

            UpdatePhase();

            if (patternRoutine == null)
                TryExecuteNextPattern();
        }

        private void OnDisable()
        {
            StopRunningPattern();
        }

        private void OnValidate()
        {
            commonRecoveryTime = Mathf.Max(0f, commonRecoveryTime);
            phase2HealthRatio = Mathf.Clamp01(phase2HealthRatio);
            phase3HealthRatio = Mathf.Clamp(phase3HealthRatio, 0f, phase2HealthRatio);
        }

        public void InitializeBoss(int bossStage, Transform target)
        {
            BossStage = Mathf.Max(1, bossStage);

            if (target != null)
                player = target;
        }

        public void SetState(BossState state, bool allowMovement)
        {
            State = state;

            if (enemyController != null)
                enemyController.SetMovementEnabled(useDefaultEnemyMovement && allowMovement);
        }

        public void MultiplyPatternCooldown(float multiplier)
        {
            PatternCooldownMultiplier = Mathf.Max(0.1f, PatternCooldownMultiplier * multiplier);
        }

        public void MultiplyMoveSpeed(float multiplier)
        {
            if (enemyController == null)
                return;

            enemyController.SetMoveSpeed(enemyController.MoveSpeed * Mathf.Max(0f, multiplier));
        }

        public float ActiveBossHealthRatio()
        {
            return HealthProgress;
        }

        private void TryExecuteNextPattern()
        {
            BossPattern selectedPattern = SelectPattern();

            if (selectedPattern == null)
                return;

            patternRoutine = StartCoroutine(RunPattern(selectedPattern));
        }

        private BossPattern SelectPattern()
        {
            BossPattern bestPattern = null;
            BossPattern fallbackPattern = null;

            foreach (BossPattern pattern in patterns)
            {
                if (pattern == null || !pattern.CanExecute())
                    continue;

                if (fallbackPattern == null || pattern.Priority > fallbackPattern.Priority)
                    fallbackPattern = pattern;

                if (pattern == lastPattern)
                    continue;

                if (bestPattern == null || pattern.Priority > bestPattern.Priority)
                    bestPattern = pattern;
            }

            return bestPattern != null ? bestPattern : fallbackPattern;
        }

        private IEnumerator RunPattern(BossPattern pattern)
        {
            currentPattern = pattern;
            SetState(BossState.Attacking, pattern.AllowMovementDuringPattern);

            yield return pattern.Execute();

            if (!IsDead)
            {
                SetState(BossState.Recovering, false);
                yield return new WaitForSeconds(commonRecoveryTime * PatternCooldownMultiplier);
                SetState(BossState.Chasing, true);
            }

            lastPattern = pattern;
            currentPattern = null;
            patternRoutine = null;
        }

        private void StopRunningPattern()
        {
            if (patternRoutine != null)
            {
                StopCoroutine(patternRoutine);
                patternRoutine = null;
            }

            currentPattern = null;

            if (!IsDead)
                SetState(BossState.Chasing, true);
        }

        private void UpdatePhase()
        {
            if (enemyHealth == null)
                return;

            float healthProgress = enemyHealth.HealthProgress;
            int nextPhase = 1;

            if (healthProgress <= phase3HealthRatio)
                nextPhase = 3;
            else if (healthProgress <= phase2HealthRatio)
                nextPhase = 2;

            CurrentPhase = Mathf.Max(CurrentPhase, nextPhase);
        }
    }
}
