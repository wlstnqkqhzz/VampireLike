using UnityEngine;

/// <summary>
/// 카메라가 지정된 대상, 기본적으로 Player 오브젝트를 따라가도록 처리한다.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // 따라갈 대상이다. 비어 있으면 런타임에 이름이 "Player"인 오브젝트를 찾는다.
    [SerializeField]
    private Transform target;

    // 카메라가 대상에서 얼마나 떨어져 있을지 정한다. 2D에서는 Z -10이 기본 카메라 위치다.
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void OnEnable()
    {
        FollowTarget();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        // 씬에서 수동 연결하지 않아도 동작하도록 Player를 자동 탐색한다.
        if (target == null)
        {
            GameObject player = GameObject.Find("Player");

            if (player != null)
                target = player.transform;
        }

        if (target == null)
            return;

        transform.position = target.position + offset;
    }
}
