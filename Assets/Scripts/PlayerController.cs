using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 플레이어 이동 속도
    public float moveSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.leftArrowKey.isPressed)
            input.x -= 1;

        if (Keyboard.current.rightArrowKey.isPressed)
            input.x += 1;

        if (Keyboard.current.upArrowKey.isPressed)
            input.y += 1;

        if (Keyboard.current.downArrowKey.isPressed)
            input.y -= 1;

        Vector3 direction = new Vector3(input.x, input.y, 0f);

        transform.position +=
            direction.normalized * moveSpeed * Time.deltaTime;
    }
}