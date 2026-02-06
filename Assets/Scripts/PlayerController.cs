using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float MoveSpeed = 5f;

    [Header("Компоненты")]
    public Rigidbody2D Rb;
    public SpriteRenderer SpriteRenderer;

    [Header("Сортировка")]
    public int BaseSortingOrder = 10000;  // ? ИЗМЕНЕНО: было 100, стало 1000

    private Vector2 moveInput;

    void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveInput.y = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveInput.y = -1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput.x = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput.x = 1;

        if (moveInput.magnitude > 1f)
            moveInput.Normalize();

        if (moveInput.x < 0)
            SpriteRenderer.flipX = true;
        else if (moveInput.x > 0)
            SpriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        Rb.linearVelocity = moveInput * MoveSpeed;
    }

    void LateUpdate()
    {
        // Теперь order будет: 1000 - 126 = 874 (положительный!)
        SpriteRenderer.sortingOrder = BaseSortingOrder - Mathf.RoundToInt(transform.position.y * 10);
    }
}