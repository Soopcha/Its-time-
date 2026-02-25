using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float WalkSpeed = 3f;
    public float RunSpeed = 6f;

    [Header("Компоненты")]
    public Rigidbody2D Rb;
    public SpriteRenderer SpriteRenderer;
    public Animator Animator;  // ? НОВОЕ!

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
    private Vector2 lastMoveDirection = Vector2.down;  // ? НОВОЕ!
    private bool isRunning = false;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    void Start()
    {
        CurrentHealth = MaxHealth;

        if (Animator == null)
            Animator = GetComponent<Animator>();
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

        // Запоминаем последнее направление
        if (moveInput.magnitude > 0.1f)
            lastMoveDirection = moveInput;

        // Shift = бежать
        isRunning = Keyboard.current.leftShiftKey.isPressed;

        // УБРАЛИ flipX — теперь анимации сами переключают спрайт!

        // Анимация
        UpdateAnimation();

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

    void UpdateAnimation()
    {
        if (Animator == null) return;

        bool isMoving = moveInput.magnitude > 0.1f;

        Animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // Передаём направление движения
            Animator.SetFloat("MoveX", moveInput.x);
            Animator.SetFloat("MoveY", moveInput.y);
        }
        else
        {
            // Передаём последнее направление
            Animator.SetFloat("MoveX", lastMoveDirection.x);
            Animator.SetFloat("MoveY", lastMoveDirection.y);
        }
    }

    void FixedUpdate()
    {
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