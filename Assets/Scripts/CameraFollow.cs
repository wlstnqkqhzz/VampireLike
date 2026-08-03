using UnityEngine;
using VampireLike.World;

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

    [SerializeField]
    private bool clampToMapBounds = true;

    private Camera followCamera;

    private void Awake()
    {
        followCamera = GetComponent<Camera>();
    }

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

        Vector3 nextPosition = target.position + offset;

        if (clampToMapBounds)
            nextPosition = ClampPositionToMapBounds(nextPosition);

        transform.position = nextPosition;
    }

    private Vector3 ClampPositionToMapBounds(Vector3 position)
    {
        if (!MapBoundary.TryGetWorldBounds(out Bounds mapBounds))
            return position;

        if (followCamera == null)
            followCamera = GetComponent<Camera>();

        if (followCamera == null || !followCamera.orthographic)
            return position;

        float halfHeight = followCamera.orthographicSize;
        float halfWidth = halfHeight * followCamera.aspect;
        float minX = mapBounds.min.x + halfWidth;
        float maxX = mapBounds.max.x - halfWidth;
        float minY = mapBounds.min.y + halfHeight;
        float maxY = mapBounds.max.y - halfHeight;

        if (minX > maxX)
            position.x = mapBounds.center.x;
        else
            position.x = Mathf.Clamp(position.x, minX, maxX);

        if (minY > maxY)
            position.y = mapBounds.center.y;
        else
            position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
