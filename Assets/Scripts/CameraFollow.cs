using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

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
