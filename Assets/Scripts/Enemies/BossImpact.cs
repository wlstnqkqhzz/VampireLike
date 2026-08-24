using System.Collections;
using UnityEngine;
using VampireLike.VFX;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스 패턴의 타격 순간에 사용할 공통 화면 흔들림, 히트스톱, 임팩트 VFX를 제공합니다.
    /// 패턴별 피해 판정은 건드리지 않고 연출만 담당합니다.
    /// </summary>
    public static class BossImpact
    {
        private static BossImpactRunner runner;

        /// <summary>
        /// 돌진이 끝나는 지점에 짧은 충격 연출을 재생합니다.
        /// </summary>
        public static void PlayDashImpact(Vector2 position, Vector2 direction, float size = 0.85f)
        {
            CombatVFX.PlayBurst(position, CombatVFXKind.TargetImpact, size, 0.18f, 1800);
            CombatVFX.PlayDirectionalStreak(position, direction, CombatVFXKind.TargetImpact, size * 0.9f, 0.16f, 0.16f, 1800);
            ShakeCamera(0.08f, 0.05f);
            HitStop(0.035f);
        }

        /// <summary>
        /// 충격파가 발생하는 순간 중심부 폭발과 화면 반응을 재생합니다.
        /// </summary>
        public static void PlayShockwaveImpact(Vector2 position, float radius)
        {
            CombatVFX.PlayBurst(position, CombatVFXKind.Shockwave, Mathf.Max(0.5f, radius * 0.55f), 0.22f, 1800);
            ShakeCamera(0.11f, 0.065f);
            HitStop(0.045f);
        }

        /// <summary>
        /// 페이즈 전환 시 보스 중심으로 분노/각성 느낌의 공통 연출을 재생합니다.
        /// </summary>
        public static void PlayPhaseTransitionImpact(Transform target, int phase, float duration)
        {
            if (target == null)
                return;

            float size = phase >= 3 ? 1.65f : 1.35f;
            CombatVFX.PlayBossCastAura(target, CombatVFXKind.ArcaneImpact, size, Mathf.Max(0.2f, duration), 1850);
            CombatVFX.PlayExpandingRing(target.position, CombatVFXKind.ArcaneImpact, 0.35f, size * 1.8f, Mathf.Max(0.2f, duration), 1840);
            ShakeCamera(0.14f, 0.06f);
        }

        public static void ShakeCamera(float duration, float intensity)
        {
            GetRunner().ShakeCamera(duration, intensity);
        }

        public static void HitStop(float duration)
        {
            GetRunner().HitStop(duration);
        }

        private static BossImpactRunner GetRunner()
        {
            if (runner != null)
                return runner;

            GameObject runnerObject = new GameObject("Boss Impact Runner");
            Object.DontDestroyOnLoad(runnerObject);
            runner = runnerObject.AddComponent<BossImpactRunner>();
            return runner;
        }
    }

    /// <summary>
    /// 정적 연출 도구가 코루틴을 실행할 수 있도록 유지되는 내부 실행자입니다.
    /// </summary>
    public class BossImpactRunner : MonoBehaviour
    {
        private Coroutine cameraShakeRoutine;
        private Coroutine hitStopRoutine;

        public void ShakeCamera(float duration, float intensity)
        {
            if (cameraShakeRoutine != null || duration <= 0f || intensity <= 0f)
                return;

            cameraShakeRoutine = StartCoroutine(ShakeCameraRoutine(duration, intensity));
        }

        public void HitStop(float duration)
        {
            if (hitStopRoutine != null || duration <= 0f || Time.timeScale <= 0.01f)
                return;

            hitStopRoutine = StartCoroutine(HitStopRoutine(duration));
        }

        private IEnumerator ShakeCameraRoutine(float duration, float intensity)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                cameraShakeRoutine = null;
                yield break;
            }

            Transform cameraTransform = mainCamera.transform;
            Vector3 originalPosition = cameraTransform.localPosition;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float strength = Mathf.Lerp(intensity, 0f, elapsedTime / duration);
                Vector2 offset = Random.insideUnitCircle * strength;
                cameraTransform.localPosition = originalPosition + new Vector3(offset.x, offset.y, 0f);
                yield return null;
            }

            cameraTransform.localPosition = originalPosition;
            cameraShakeRoutine = null;
        }

        private IEnumerator HitStopRoutine(float duration)
        {
            float originalTimeScale = Time.timeScale;
            Time.timeScale = Mathf.Min(originalTimeScale, 0.08f);
            yield return new WaitForSecondsRealtime(duration);

            if (Time.timeScale > 0f)
                Time.timeScale = originalTimeScale;

            hitStopRoutine = null;
        }
    }
}
