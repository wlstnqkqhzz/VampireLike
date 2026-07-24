using System.Collections;
using System.Linq;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 프리팹의 SpriteRenderer에 Idle, Attack, Skill, Hit, Death 프레임을 재생하는 간단한 공통 애니메이터입니다.
    /// Resources 폴더의 보스별 프레임을 불러와 Unity Animator Controller 없이 보스 상태를 표현합니다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BossSpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private string resourceFolder = "BossAnimations/Stage01";

        [SerializeField]
        private float idleFrameRate = 6f;

        [SerializeField]
        private float walkFrameRate = 8f;

        [SerializeField]
        private float hitFrameRate = 12f;

        [SerializeField]
        private float attackFrameRate = 12f;

        [SerializeField]
        private float skillFrameRate = 12f;

        [SerializeField]
        private float deathFrameRate = 8f;

        [SerializeField]
        private float hitAnimationCooldown = 0.25f;

        private SpriteRenderer spriteRenderer;
        private Sprite[] idleFrames;
        private Sprite[] walkFrames;
        private Sprite[] hitFrames;
        private Sprite[] attackFrames;
        private Sprite[] skillFrames;
        private Sprite[] deathFrames;
        private Coroutine animationRoutine;
        private AnimationPriority currentPriority = AnimationPriority.Idle;
        private LoopAnimationType currentLoop = LoopAnimationType.None;
        private float nextHitAnimationTime;
        private bool isFacingLeft;

        private void Awake()
        {
            ValidateValues();
            spriteRenderer = GetComponent<SpriteRenderer>();
            LoadFrames();
        }

        private void OnEnable()
        {
            PlayIdle();
        }

        private void OnDisable()
        {
            StopCurrentAnimation();
        }

        private void OnValidate()
        {
            ValidateValues();
        }

        private void ValidateValues()
        {
            idleFrameRate = Mathf.Max(1f, idleFrameRate);
            walkFrameRate = Mathf.Max(1f, walkFrameRate);
            hitFrameRate = Mathf.Max(1f, hitFrameRate);
            attackFrameRate = Mathf.Max(1f, attackFrameRate);
            skillFrameRate = Mathf.Max(1f, skillFrameRate);
            deathFrameRate = Mathf.Max(1f, deathFrameRate);
            hitAnimationCooldown = Mathf.Max(0f, hitAnimationCooldown);
        }

        public void PlayHit()
        {
            if (hitFrames == null || hitFrames.Length == 0)
                return;

            if (currentPriority != AnimationPriority.Idle)
                return;

            if (Time.time < nextHitAnimationTime)
                return;

            nextHitAnimationTime = Time.time + hitAnimationCooldown;
            PlayOnce(hitFrames, hitFrameRate, AnimationPriority.Hit);
        }

        public void PlayAttack()
        {
            if (attackFrames == null || attackFrames.Length == 0)
                return;

            PlayOnce(attackFrames, attackFrameRate, AnimationPriority.Attack);
        }

        public void FaceDirection(Vector2 direction)
        {
            if (spriteRenderer == null || Mathf.Abs(direction.x) <= 0.01f)
                return;

            isFacingLeft = direction.x < 0f;
            spriteRenderer.flipX = isFacingLeft;
        }

        public void ShowAttackFrame(int frameIndex)
        {
            if (attackFrames == null || attackFrames.Length == 0)
                return;

            currentPriority = AnimationPriority.Attack;
            currentLoop = LoopAnimationType.None;
            StopCurrentAnimation();
            spriteRenderer.sprite = attackFrames[Mathf.Clamp(frameIndex, 0, attackFrames.Length - 1)];
            spriteRenderer.flipX = isFacingLeft;
        }

        public void PlaySkill()
        {
            if (skillFrames == null || skillFrames.Length == 0)
            {
                PlayAttack();
                return;
            }

            PlayOnce(skillFrames, skillFrameRate, AnimationPriority.Attack);
        }

        public float PlayDeath()
        {
            if (deathFrames == null || deathFrames.Length == 0)
                return 0f;

            currentPriority = AnimationPriority.Death;
            currentLoop = LoopAnimationType.None;
            StopCurrentAnimation();
            animationRoutine = StartCoroutine(PlayOnceRoutine(deathFrames, deathFrameRate, false));
            return deathFrames.Length / deathFrameRate;
        }

        private void LoadFrames()
        {
            idleFrames = LoadFrameSet("Idle");
            walkFrames = LoadFrameSet("Walk");
            hitFrames = LoadFrameSet("Hit");
            attackFrames = LoadFrameSet("Attack");
            skillFrames = LoadFrameSet("Skill");
            deathFrames = LoadFrameSet("Death");
        }

        public void SetResourceFolder(string nextResourceFolder)
        {
            if (string.IsNullOrWhiteSpace(nextResourceFolder) || resourceFolder == nextResourceFolder)
                return;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            resourceFolder = nextResourceFolder;
            LoadFrames();
            currentPriority = AnimationPriority.Idle;
            currentLoop = LoopAnimationType.None;
            PlayIdle();
        }

        private Sprite[] LoadFrameSet(string prefix)
        {
            return Resources.LoadAll<Sprite>(resourceFolder)
                .Where(sprite => IsExactFrameName(sprite.name, prefix))
                .OrderBy(sprite => sprite.name)
                .ToArray();
        }

        private bool IsExactFrameName(string spriteName, string prefix)
        {
            string expectedPrefix = prefix + "_";

            if (!spriteName.StartsWith(expectedPrefix, System.StringComparison.Ordinal))
                return false;

            string frameNumber = spriteName.Substring(expectedPrefix.Length);
            return int.TryParse(frameNumber, out _);
        }

        public void PlayIdle()
        {
            if (idleFrames == null || idleFrames.Length == 0)
                return;

            if (currentPriority == AnimationPriority.Idle && currentLoop == LoopAnimationType.Idle)
                return;

            currentPriority = AnimationPriority.Idle;
            StopCurrentAnimation();
            currentLoop = LoopAnimationType.Idle;
            animationRoutine = StartCoroutine(LoopAnimation(idleFrames, idleFrameRate));
        }

        public void PlayWalk()
        {
            if (walkFrames == null || walkFrames.Length == 0)
            {
                PlayIdle();
                return;
            }

            if (currentPriority != AnimationPriority.Idle)
                return;

            if (currentLoop == LoopAnimationType.Walk)
                return;

            StopCurrentAnimation();
            currentLoop = LoopAnimationType.Walk;
            animationRoutine = StartCoroutine(LoopAnimation(walkFrames, walkFrameRate));
        }

        private void PlayOnce(Sprite[] frames, float frameRate, AnimationPriority priority)
        {
            if (priority < currentPriority)
                return;

            currentPriority = priority;
            currentLoop = LoopAnimationType.None;
            StopCurrentAnimation();
            animationRoutine = StartCoroutine(PlayOnceRoutine(frames, frameRate, true));
        }

        private IEnumerator LoopAnimation(Sprite[] frames, float frameRate)
        {
            int frameIndex = 0;
            WaitForSeconds delay = new WaitForSeconds(1f / frameRate);

            while (true)
            {
                spriteRenderer.sprite = frames[frameIndex];
                spriteRenderer.flipX = isFacingLeft;
                frameIndex = (frameIndex + 1) % frames.Length;
                yield return delay;
            }
        }

        private IEnumerator PlayOnceRoutine(Sprite[] frames, float frameRate, bool returnToIdle)
        {
            WaitForSeconds delay = new WaitForSeconds(1f / frameRate);

            foreach (Sprite frame in frames)
            {
                spriteRenderer.sprite = frame;
                spriteRenderer.flipX = isFacingLeft;
                yield return delay;
            }

            animationRoutine = null;

            if (returnToIdle)
                PlayIdle();
        }

        private void StopCurrentAnimation()
        {
            if (animationRoutine == null)
                return;

            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        private enum AnimationPriority
        {
            Idle = 0,
            Attack = 1,
            Hit = 2,
            Death = 3
        }

        private enum LoopAnimationType
        {
            None = 0,
            Idle = 1,
            Walk = 2
        }
    }
}
