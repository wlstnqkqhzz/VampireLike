using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 보스가 만든 장판 하나의 둔화, 지속 피해, 수명 처리를 담당한다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BossAreaZone : MonoBehaviour
    {
        private float lifetime;
        private float slowMultiplier;
        private int damage;
        private float damageInterval;
        private float nextDamageTime;
        private global::PlayerController slowedPlayer;

        public void Initialize(float duration, float playerSlowMultiplier, int tickDamage, float tickInterval, float radius)
        {
            lifetime = Mathf.Max(0.1f, duration);
            slowMultiplier = Mathf.Clamp(playerSlowMultiplier, 0.25f, 1f);
            damage = Mathf.Max(0, tickDamage);
            damageInterval = Mathf.Max(0.1f, tickInterval);

            Collider2D zoneCollider = GetComponent<Collider2D>();
            zoneCollider.isTrigger = true;

            if (zoneCollider is CircleCollider2D circleCollider)
                circleCollider.radius = Mathf.Max(0.05f, radius);
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;

            if (lifetime <= 0f)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryApplySlow(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryApplySlow(other);
            TryApplyDamage(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            global::PlayerController playerController = other.GetComponentInParent<global::PlayerController>();

            if (playerController != null && playerController == slowedPlayer)
                RemoveSlow();
        }

        private void OnDisable()
        {
            RemoveSlow();
        }

        private void OnDestroy()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();

            if (lineRenderer != null && lineRenderer.material != null)
                Destroy(lineRenderer.material);
        }

        private void TryApplySlow(Collider2D other)
        {
            global::PlayerController playerController = other.GetComponentInParent<global::PlayerController>();

            if (playerController == null || playerController == slowedPlayer)
                return;

            RemoveSlow();
            slowedPlayer = playerController;
            slowedPlayer.AddMoveSpeedMultiplier(this, slowMultiplier);
        }

        private void TryApplyDamage(Collider2D other)
        {
            if (damage <= 0 || Time.time < nextDamageTime)
                return;

            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                return;

            nextDamageTime = Time.time + damageInterval;
            playerHealth.TakeDamage(damage);
        }

        private void RemoveSlow()
        {
            if (slowedPlayer == null)
                return;

            slowedPlayer.RemoveMoveSpeedMultiplier(this);
            slowedPlayer = null;
        }
    }
}
