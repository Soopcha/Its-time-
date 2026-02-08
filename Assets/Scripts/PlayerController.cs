using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float WalkSpeed = 3f;    // Обычная ходьба
    public float RunSpeed = 6f;     // Бег (Shift)

    [Header("Компоненты")]
    public Rigidbody2D Rb;
    public SpriteRenderer SpriteRenderer;

    [Header("Сортировка")]
    public int BaseSortingOrder = 10000;

    [Header("Здоровье")]
    public int MaxHealth = 3;
    public int CurrentHealth = 3;
    public float InvincibilityTime = 2f;

    [Header("Укрытие")]
    public bool IsHidden = false;

    // Публичные свойства для EnemyAI
    public bool IsRunning => isRunning && moveInput.magnitude > 0.1f;
    public bool IsMoving => moveInput.magnitude > 0.1f;

    private Vector2 moveInput;
    private bool isRunning = false;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    void Update()
    {
        // Читаем ввод
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

        // Shift = бежать (теперь БЫСТРЕЕ!)
        isRunning = Keyboard.current.leftShiftKey.isPressed;

        // Флип спрайта
        if (moveInput.x < 0)
            SpriteRenderer.flipX = true;
        else if (moveInput.x > 0)
            SpriteRenderer.flipX = false;

        // Таймер неуязвимости
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                SpriteRenderer.color = Color.white;
            }
            else
            {
                float alpha = Mathf.PingPong(Time.time * 10f, 1f) > 0.5f ? 1f : 0.3f;
                SpriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
        }
    }

    void FixedUpdate()
    {
        // Shift = быстрее, без Shift = медленнее
        float speed = isRunning ? RunSpeed : WalkSpeed;
        Rb.linearVelocity = moveInput * speed;
    }

    void LateUpdate()
    {
        SpriteRenderer.sortingOrder = BaseSortingOrder - Mathf.RoundToInt(transform.position.y * 10);
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        CurrentHealth--;
        Debug.Log($"Урон! Осталось жизней: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            isInvincible = true;
            invincibilityTimer = InvincibilityTime;
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HidingSpot"))
        {
            IsHidden = true;
            Debug.Log("Спрятался!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HidingSpot"))
        {
            IsHidden = false;
            Debug.Log("Вышел из укрытия");
        }
    }
}