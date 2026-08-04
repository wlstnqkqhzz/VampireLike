using System.Collections;
using System.Linq;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 일반 적 프리팹의 SpriteRenderer에 Resources 기반 간단 애니메이션을 적용한다.
    /// 프리팹 이름으로 기본 폴더를 추론하므로, 적 프리팹마다 수동 연결을 최소화할 수 있다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemySpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private string resourceFolder;

        [SerializeField]
        private float idleFrameRate = 5f;

        [SerializeField]
        private float walkFrameRate = 8f;

        [SerializeField]
        private float hitFrameRate = 12f;

        [SerializeField]
        private float deathFrameRate = 8f;

        [SerializeField]
        private float hitAnimationCooldown = 0.18f;

        [SerializeField]
        private bool invertFacing;

        private SpriteRenderer spriteRenderer;
        private Sprite[] idleFrames;
        private Sprite[] walkFrames;
        private Sprite[] hitFrames;
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

            if (string.IsNullOrWhiteSpace(resourceFolder))
                resourceFolder = GuessResourceFolder(gameObject.name);

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

        public void FaceDirection(Vector2 direction)
        {
            if (spriteRenderer == null || Mathf.Abs(direction.x) <= 0.01f)
                return;

            isFacingLeft = direction.x < 0f;
            spriteRenderer.flipX = invertFacing ? !isFacingLeft : isFacingLeft;
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

        public void PlayHit()
        {
            if (hitFrames == null || hitFrames.Length == 0)
                return;

            if (currentPriority != AnimationPriority.Idle)
                return;

            if (Time.time < nextHitAnimationTime)
                return;

            nextHitAnimationTime = Time.time + hitAnimationCooldown;
            PlayOnce(hitFrames, hitFrameRate, AnimationPriority.Hit, true);
        }

        public float PlayDeath()
        {
            if (deathFrames == null || deathFrames.Length == 0)
                return 0f;

            PlayOnce(deathFrames, deathFrameRate, AnimationPriority.Death, false);
            return deathFrames.Length / deathFrameRate;
        }

        private void LoadFrames()
        {
            idleFrames = LoadFrameSet("Idle");
            walkFrames = LoadFrameSet("Walk");
            hitFrames = LoadFrameSet("Hit");
            deathFrames = LoadFrameSet("Death");
        }

        private Sprite[] LoadFrameSet(string prefix)
        {
            return Resources.LoadAll<Sprite>(resourceFolder)
                .Where(sprite => IsExactFrameName(sprite.name, prefix))
                .OrderBy(sprite => sprite.name)
                .ToArray();
        }

        private static bool IsExactFrameName(string spriteName, string prefix)
        {
            string expectedPrefix = prefix + "_";

            if (!spriteName.StartsWith(expectedPrefix, System.StringComparison.Ordinal))
                return false;

            string frameNumber = spriteName.Substring(expectedPrefix.Length);
            return int.TryParse(frameNumber, out _);
        }

        private void PlayOnce(Sprite[] frames, float frameRate, AnimationPriority priority, bool returnToIdle)
        {
            if (priority < currentPriority)
                return;

            currentPriority = priority;
            currentLoop = LoopAnimationType.None;
            StopCurrentAnimation();
            animationRoutine = StartCoroutine(PlayOnceRoutine(frames, frameRate, returnToIdle));
        }

        private IEnumerator LoopAnimation(Sprite[] frames, float frameRate)
        {
            int frameIndex = 0;
            WaitForSeconds delay = new WaitForSeconds(1f / frameRate);

            while (true)
            {
                ApplyFrame(frames[frameIndex]);
                frameIndex = (frameIndex + 1) % frames.Length;
                yield return delay;
            }
        }

        private IEnumerator PlayOnceRoutine(Sprite[] frames, float frameRate, bool returnToIdle)
        {
            WaitForSeconds delay = new WaitForSeconds(1f / frameRate);

            foreach (Sprite frame in frames)
            {
                ApplyFrame(frame);
                yield return delay;
            }

            animationRoutine = null;

            if (returnToIdle)
                PlayIdle();
        }

        private void ApplyFrame(Sprite frame)
        {
            spriteRenderer.sprite = frame;
            spriteRenderer.flipX = invertFacing ? !isFacingLeft : isFacingLeft;
        }

        private void StopCurrentAnimation()
        {
            if (animationRoutine == null)
                return;

            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        private void ValidateValues()
        {
            idleFrameRate = Mathf.Max(1f, idleFrameRate);
            walkFrameRate = Mathf.Max(1f, walkFrameRate);
            hitFrameRate = Mathf.Max(1f, hitFrameRate);
            deathFrameRate = Mathf.Max(1f, deathFrameRate);
            hitAnimationCooldown = Mathf.Max(0f, hitAnimationCooldown);
        }

        private static string GuessResourceFolder(string objectName)
        {
            string cleanName = objectName.Replace("(Clone)", string.Empty).Trim();

            return cleanName switch
            {
                "EnemyFast" => "EnemyAnimations/Fast",
                "EnemyTank" => "EnemyAnimations/Tank",
                "EnemyRanged" => "EnemyAnimations/Ranged",
                "EnemyCharger" => "EnemyAnimations/Charger",
                "EnemyExploder" => "EnemyAnimations/Exploder",
                "EnemySplitter" => "EnemyAnimations/Splitter",
                "EnemySplitSmall" => "EnemyAnimations/SplitSmall",
                "EnemySupport" => "EnemyAnimations/Support",
                _ => "EnemyAnimations/Basic"
            };
        }

        private enum AnimationPriority
        {
            Idle = 0,
            Hit = 1,
            Death = 2
        }

        private enum LoopAnimationType
        {
            None = 0,
            Idle = 1,
            Walk = 2
        }
    }
}
