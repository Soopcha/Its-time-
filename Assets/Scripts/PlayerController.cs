using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float MoveSpeed = 5f;

    [Header("Компоненты")]
    public Rigidbody2D Rb;
    public SpriteRenderer SpriteRenderer;

    private Vector2 moveInput;

    void Update()
    {
        // Читаем ввод с клавиатуры
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveInput.y = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveInput.y = -1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput.x = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput.x = 1;

        // Нормализуем чтобы по диагонали не бежал быстрее
        if (moveInput.magnitude > 1f)
            moveInput.Normalize();

        // Флип спрайта влево/вправо
        if (moveInput.x < 0)
            SpriteRenderer.flipX = true;
        else if (moveInput.x > 0)
            SpriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        // Двигаем персонажа через физику
        Rb.linearVelocity = moveInput * MoveSpeed;
    }
}