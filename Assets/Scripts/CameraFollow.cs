using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 카메라가 따라갈 대상
    [SerializeField]
    private Transform target;

    private void LateUpdate()
    {
        // 플레이어를 화면 중앙에 유지
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            -10f
        );
    }
}