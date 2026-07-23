using UnityEngine;

namespace VampireLike.Combat
{
    /// <summary>
    /// 적이 플레이어와 접촉했을 때 줄 피해량을 적 개체별로 관리한다.
    /// </summary>
    public class EnemyContactDamage : MonoBehaviour
    {
        [SerializeField]
        private int contactDamage = 1;

        public int ContactDamage => contactDamage;

        private void OnValidate()
        {
            contactDamage = Mathf.Max(1, contactDamage);
        }

        public void SetContactDamage(int value)
        {
            contactDamage = Mathf.Max(1, value);
        }
    }
}
