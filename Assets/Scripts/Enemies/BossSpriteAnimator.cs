using System.Collections;
using System.Linq;
using UnityEngine;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 프리팹의 SpriteRenderer에 Idle, Hit, Attack 프레임을 간단히 재생하는 공통 애니메이터다.
    /// Resources 폴더의 보스별 프레임을 불러와서 Unity Animator Controller 없이도 보스 상태를 표현한다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BossSpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private string resourceFolder = "BossAnimations/Stage01";

        [SerializeField]
        private float idleFrameRate = 6f;

        [SerializeField]
        private float hitFrameRate = 12f;

        [SerializeField]
        private float attackFrameRate = 12f;

        private SpriteRenderer spriteRenderer;
        private Sprite[] idleFrames;
        private Sprite[] hitFrames;
        private Sprite[] attackFrames;
        private Coroutine animationRoutine;
        private AnimationPriority currentPriority = AnimationPriority.Idle;

        private void Awake()
        {
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
            idleFrameRate = Mathf.Max(1f, idleFrameRate);
            hitFrameRate = Mathf.Max(1f, hitFrameRate);
            attackFrameRate = Mathf.Max(1f, attackFrameRate);
        }

        public void PlayHit()
        {
            if (hitFrames == null || hitFrames.Length == 0)
                return;

            PlayOnce(hitFrames, hitFrameRate, AnimationPriority.Hit);
        }

        public void PlayAttack()
        {
            if (attackFrames == null || attackFrames.Length == 0)
                return;

            PlayOnce(attackFrames, attackFrameRate, AnimationPriority.Attack);
        }

        private void LoadFrames()
        {
            idleFrames = LoadFrameSet("Idle");
            hitFrames = LoadFrameSet("Hit");
            attackFrames = LoadFrameSet("Attack");
        }

        private Sprite[] LoadFrameSet(string prefix)
        {
            return Resources.LoadAll<Sprite>(resourceFolder)
                .Where(sprite => sprite.name.StartsWith(prefix, System.StringComparison.Ordinal))
                .OrderBy(sprite => sprite.name)
                .ToArray();
        }

        private void PlayIdle()
        {
            if (idleFrames == null || idleFrames.Length == 0)
                return;

            currentPriority = AnimationPriority.Idle;
            StopCurrentAnimation();
            animationRoutine = StartCoroutine(LoopAnimation(idleFrames, idleFrameRate));
        }

        private void PlayOnce(Sprite[] frames, float frameRate, AnimationPriority priority)
        {
            if (priority < currentPriority)
                return;

            currentPriority = priority;
            StopCurrentAnimation();
            animationRoutine = StartCoroutine(PlayOnceRoutine(frames, frameRate));
        }

        private IEnumerator LoopAnimation(Sprite[] frames, float frameRate)
        {
            int frameIndex = 0;
            WaitForSeconds delay = new WaitForSeconds(1f / frameRate);

            while (true)
            {
                spriteRenderer.sprite = frames[frameIndex];
                frameIndex = (frameIndex + 1) % frames.Length;
                yield return delay;
            }
        }

        private IEnumerator PlayOnceRoutine(Sprite[] frames, float frameRate)
        {
            WaitForSeconds delay = new WaitForSeconds(1f / frameRate);

            foreach (Sprite frame in frames)
            {
                spriteRenderer.sprite = frame;
                yield return delay;
            }

            animationRoutine = null;
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
            Hit = 2
        }
    }
}
