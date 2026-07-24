using UnityEngine;
using VampireLike.Combat;

namespace VampireLike.Enemies
{
    /// <summary>
    /// 적이 제거될 때 작은 적을 여러 마리 생성하는 분열형 적 행동이다.
    /// 게임 종료나 씬 종료 중에는 추가 생성을 하지 않도록 OnApplicationQuit 상태를 확인한다.
    /// </summary>
    public class SplitOnDeath : MonoBehaviour
    {
        [SerializeField]
        private GameObject splitPrefab;

        [SerializeField]
        private int splitCount = 3;

        [SerializeField]
        private float spawnRadius = 0.45f;

        private bool isQuitting;

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        private void OnDestroy()
        {
            if (isQuitting || GameState.IsGameOver || splitPrefab == null || splitCount <= 0)
                return;

            for (int i = 0; i < splitCount; i++)
            {
                float angle = Mathf.PI * 2f * i / splitCount;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
                Instantiate(splitPrefab, (Vector2)transform.position + offset, Quaternion.identity, transform.parent);
            }
        }

        private void OnValidate()
        {
            splitCount = Mathf.Max(0, splitCount);
            spawnRadius = Mathf.Max(0f, spawnRadius);
        }
    }
}
