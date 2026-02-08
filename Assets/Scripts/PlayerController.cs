using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float MoveSpeed = 5f;
    public float SneakSpeed = 2f;  // Скорость крадучись

    [Header("Компоненты")]
    public Rigidbody2D Rb;
    public SpriteRenderer SpriteRenderer;

    [Header("Сортировка")]
    public int BaseSortingOrder = 10000;

    [Header("Здоровье")]
    public int MaxHealth = 3;
    public int CurrentHealth = 3;
    public float InvincibilityTime = 2f;  // Неуязвимость после урона

    [Header("Укрытие")]
    public bool IsHidden = false;  // Прячется ли в кустах

    private Vector2 moveInput;
    private bool isSneaking = false;
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

        // Shift = красться
        isSneaking = Keyboard.current.leftShiftKey.isPressed;

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
                // Мигание
                float alpha = Mathf.PingPong(Time.time * 10f, 1f) > 0.5f ? 1f : 0.3f;
                SpriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
        }
    }

    void FixedUpdate()
    {
        float speed = isSneaking ? SneakSpeed : MoveSpeed;
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
            // Включаем неуязвимость
            isInvincible = true;
            invincibilityTimer = InvincibilityTime;
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER!");
        // Тут можно перезагрузить уровень или показать меню
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Вход/выход из укрытия
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