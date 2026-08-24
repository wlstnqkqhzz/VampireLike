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

        [Header("페이즈 전환 연출")]
        [SerializeField]
        private float phaseTransitionDuration = 0.55f;

        [SerializeField]
        private bool phaseTransitionInvulnerability = true;

        [SerializeField]
        private float phase2HealthRatio = 0.6f;

        [SerializeField]
        private float phase3HealthRatio = 0.3f;

        [SerializeField]
        private float globalPatternCooldownMultiplier = 0.85f;

        [SerializeField]
        private float phase2PatternCooldownMultiplier = 0.88f;

        [SerializeField]
        private float phase3PatternCooldownMultiplier = 0.72f;

        [SerializeField]
        private float phase2RecoveryTimeMultiplier = 0.85f;

        [SerializeField]
        private float phase3RecoveryTimeMultiplier = 0.65f;

        [SerializeField]
        private bool useDefaultEnemyMovement = true;

        private BossPattern[] patterns;
        private BossPattern currentPattern;
        private BossPattern lastPattern;
        private EnemyController enemyController;
        private EnemyHealth enemyHealth;
        private BossSpriteAnimator spriteAnimator;
        private Rigidbody2D rb;
        private Coroutine patternRoutine;
        private Coroutine phaseTransitionRoutine;
        private float basePatternCooldownMultiplier = 1f;
        private int requestedPhase = 1;

        public Transform Player => player;
        public Rigidbody2D BossRigidbody => rb;
        public BossState State { get; private set; } = BossState.Chasing;
        public int CurrentPhase { get; private set; } = 1;
        public int BossStage { get; private set; } = 1;
        public float PatternCooldownMultiplier => basePatternCooldownMultiplier * globalPatternCooldownMultiplier * GetPhasePatternCooldownMultiplier();
        public bool IsDead => enemyHealth == null || enemyHealth.IsDead;
        public float HealthProgress => enemyHealth == null ? 0f : enemyHealth.HealthProgress;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyController = GetComponent<EnemyController>();
            enemyHealth = GetComponent<EnemyHealth>();
            spriteAnimator = GetComponentInChildren<BossSpriteAnimator>();
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
                StopPhaseTransition();
                return;
            }

            UpdateRequestedPhase();
            UpdateFacingDirection();

            if (phaseTransitionRoutine != null)
                return;

            if (requestedPhase > CurrentPhase && patternRoutine == null)
            {
                phaseTransitionRoutine = StartCoroutine(PlayPhaseTransition(requestedPhase));
                return;
            }

            if (patternRoutine == null)
                TryExecuteNextPattern();
        }

        private void OnDisable()
        {
            StopRunningPattern();
            StopPhaseTransition();
        }

        private void OnValidate()
        {
            commonRecoveryTime = Mathf.Max(0f, commonRecoveryTime);
            phaseTransitionDuration = Mathf.Max(0f, phaseTransitionDuration);
            phase2HealthRatio = Mathf.Clamp01(phase2HealthRatio);
            phase3HealthRatio = Mathf.Clamp(phase3HealthRatio, 0f, phase2HealthRatio);
            globalPatternCooldownMultiplier = Mathf.Clamp(globalPatternCooldownMultiplier, 0.2f, 1.5f);
            phase2PatternCooldownMultiplier = Mathf.Clamp(phase2PatternCooldownMultiplier, 0.2f, 1f);
            phase3PatternCooldownMultiplier = Mathf.Clamp(phase3PatternCooldownMultiplier, 0.2f, phase2PatternCooldownMultiplier);
            phase2RecoveryTimeMultiplier = Mathf.Clamp(phase2RecoveryTimeMultiplier, 0.2f, 1f);
            phase3RecoveryTimeMultiplier = Mathf.Clamp(phase3RecoveryTimeMultiplier, 0.2f, phase2RecoveryTimeMultiplier);
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

            if (state == BossState.Chasing)
                spriteAnimator?.PlayIdle();
        }

        public void MultiplyPatternCooldown(float multiplier)
        {
            basePatternCooldownMultiplier = Mathf.Max(0.1f, basePatternCooldownMultiplier * multiplier);
        }

        public void SetPatternCooldownMultiplier(float multiplier)
        {
            basePatternCooldownMultiplier = Mathf.Max(0.1f, multiplier);
        }

        public void MultiplyMoveSpeed(float multiplier)
        {
            if (enemyController == null)
                return;

            enemyController.SetMoveSpeed(enemyController.MoveSpeed * Mathf.Max(0f, multiplier));
        }

        public void PlayAttackAnimation()
        {
            spriteAnimator?.PlayAttack();
        }

        public void PlaySkillAnimation()
        {
            spriteAnimator?.PlaySkill();
        }

        public void FaceDirection(Vector2 direction)
        {
            spriteAnimator?.FaceDirection(direction);
        }

        public void ShowAttackFrame(int frameIndex)
        {
            spriteAnimator?.ShowAttackFrame(frameIndex);
        }

        public void PlayWalkAnimation()
        {
            spriteAnimator?.PlayWalk();
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

            if (!IsDead && !GameState.IsGameOver)
            {
                SetState(BossState.Recovering, false);
                yield return new WaitForSeconds(commonRecoveryTime * basePatternCooldownMultiplier * GetPhaseRecoveryTimeMultiplier());

                if (phaseTransitionRoutine == null && !IsDead && !GameState.IsGameOver)
                    SetState(BossState.Chasing, true);
            }

            lastPattern = pattern;
            currentPattern = null;
            patternRoutine = null;
        }

        private void StopRunningPattern()
        {
            if (currentPattern != null)
                currentPattern.CancelPattern();

            if (patternRoutine != null)
            {
                StopCoroutine(patternRoutine);
                patternRoutine = null;
            }

            currentPattern = null;

            if (!IsDead && !GameState.IsGameOver && phaseTransitionRoutine == null)
                SetState(BossState.Chasing, true);
        }

        private void UpdateRequestedPhase()
        {
            if (enemyHealth == null)
                return;

            float healthProgress = enemyHealth.HealthProgress;
            int nextPhase = 1;

            if (healthProgress <= phase3HealthRatio)
                nextPhase = 3;
            else if (healthProgress <= phase2HealthRatio)
                nextPhase = 2;

            requestedPhase = Mathf.Max(requestedPhase, nextPhase);
        }

        /// <summary>
        /// 새 페이즈 진입 시 보스를 잠깐 멈추고 무적, 오라, 흔들림 연출을 재생한다.
        /// </summary>
        private IEnumerator PlayPhaseTransition(int nextPhase)
        {
            SetState(BossState.PhaseChanging, false);

            if (phaseTransitionInvulnerability)
                enemyHealth.SetInvulnerable(true);

            PlaySkillAnimation();
            BossImpact.PlayPhaseTransitionImpact(transform, nextPhase, phaseTransitionDuration);

            if (phaseTransitionDuration > 0f)
                yield return new WaitForSeconds(phaseTransitionDuration);

            CurrentPhase = Mathf.Max(CurrentPhase, nextPhase);

            if (phaseTransitionInvulnerability)
                enemyHealth.SetInvulnerable(false);

            if (!IsDead && !GameState.IsGameOver)
                SetState(BossState.Chasing, true);

            phaseTransitionRoutine = null;
        }

        private void StopPhaseTransition()
        {
            if (phaseTransitionRoutine != null)
            {
                StopCoroutine(phaseTransitionRoutine);
                phaseTransitionRoutine = null;
            }

            if (enemyHealth != null)
                enemyHealth.SetInvulnerable(false);
        }

        private float GetPhasePatternCooldownMultiplier()
        {
            if (CurrentPhase >= 3)
                return phase3PatternCooldownMultiplier;

            if (CurrentPhase >= 2)
                return phase2PatternCooldownMultiplier;

            return 1f;
        }

        private float GetPhaseRecoveryTimeMultiplier()
        {
            if (CurrentPhase >= 3)
                return phase3RecoveryTimeMultiplier;

            if (CurrentPhase >= 2)
                return phase2RecoveryTimeMultiplier;

            return 1f;
        }

        private void UpdateFacingDirection()
        {
            if (player == null || State != BossState.Chasing)
                return;

            FaceDirection(player.position - transform.position);
            PlayWalkAnimation();
        }
    }
}
