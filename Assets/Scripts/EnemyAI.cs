using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Состояния")]
    public EnemyState CurrentState = EnemyState.Patrol;

    public enum EnemyState
    {
        Patrol,     // Спокойно патрулирует
        Alert,      // Насторожен — что-то заметил
        Chase       // Преследует игрока
    }

    [Header("Скорости")]
    public float PatrolSpeed = 1f;      // Медленно бродит
    public float AlertSpeed = 1.5f;     // Чуть быстрее
    public float ChaseSpeed = 3f;       // Бежит за игроком

    [Header("Поле зрения")]
    public float ViewDistance = 5f;     // Как далеко видит
    public float ViewAngle = 60f;       // Угол конуса зрения (в градусах)

    [Header("Обнаружение")]
    public float AlertDuration = 3f;    // Сколько секунд насторожен
    public float LosePlayerTime = 5f;   // Через сколько теряет игрока из виду

    [Header("Патрулирование")]
    public List<Transform> PatrolPoints = new List<Transform>();
    public float WaitAtPoint = 2f;      // Сколько ждать в точке патруля

    [Header("Ссылки")]
    public Transform Player;
    public SpriteRenderer SpriteRenderer;
    public Rigidbody2D Rb;

    // Приватные переменные
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private float alertTimer = 0f;
    private float losePlayerTimer = 0f;
    private Vector2 lastKnownPlayerPosition;
    private Vector2 moveDirection;

    [Header("Сортировка (как у деревьев)")]
    public int BaseSortingOrder = 10000;

    void Start()
    {
        if (Player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                Player = playerObj.transform;
        }

        if (Rb == null)
            Rb = GetComponent<Rigidbody2D>();

        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Проверяем видим ли игрока
        bool canSeePlayer = CanSeePlayer();

        // Логика состояний
        switch (CurrentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                if (canSeePlayer)
                {
                    CurrentState = EnemyState.Chase;
                    losePlayerTimer = LosePlayerTime;
                }
                break;

            case EnemyState.Alert:
                HandleAlert();
                if (canSeePlayer)
                {
                    CurrentState = EnemyState.Chase;
                    losePlayerTimer = LosePlayerTime;
                }
                break;

            case EnemyState.Chase:
                HandleChase();
                if (canSeePlayer)
                {
                    losePlayerTimer = LosePlayerTime;
                    lastKnownPlayerPosition = Player.position;
                }
                else
                {
                    losePlayerTimer -= Time.deltaTime;
                    if (losePlayerTimer <= 0)
                    {
                        CurrentState = EnemyState.Alert;
                        alertTimer = AlertDuration;
                    }
                }
                break;
        }

        // Флип спрайта
        if (moveDirection.x < -0.1f)
            SpriteRenderer.flipX = true;
        else if (moveDirection.x > 0.1f)
            SpriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        float speed = GetCurrentSpeed();
        Rb.linearVelocity = moveDirection.normalized * speed;
    }

    void LateUpdate()
    {
        // Сортировка по Y (как у игрока и деревьев)
        SpriteRenderer.sortingOrder = BaseSortingOrder - Mathf.RoundToInt(transform.position.y * 10);
    }

    float GetCurrentSpeed()
    {
        switch (CurrentState)
        {
            case EnemyState.Patrol: return PatrolSpeed;
            case EnemyState.Alert: return AlertSpeed;
            case EnemyState.Chase: return ChaseSpeed;
            default: return PatrolSpeed;
        }
    }

    void HandlePatrol()
    {
        if (PatrolPoints.Count == 0)
        {
            moveDirection = Vector2.zero;
            return;
        }

        Transform targetPoint = PatrolPoints[currentPatrolIndex];
        float distance = Vector2.Distance(transform.position, targetPoint.position);

        if (distance < 0.3f)
        {
            // Дошли до точки — ждём
            moveDirection = Vector2.zero;
            waitTimer += Time.deltaTime;

            if (waitTimer >= WaitAtPoint)
            {
                waitTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % PatrolPoints.Count;
            }
        }
        else
        {
            // Идём к точке
            moveDirection = (targetPoint.position - transform.position).normalized;
        }
    }

    void HandleAlert()
    {
        // Идём к последнему известному месту игрока
        float distance = Vector2.Distance(transform.position, lastKnownPlayerPosition);

        if (distance < 0.5f)
        {
            moveDirection = Vector2.zero;
        }
        else
        {
            moveDirection = (lastKnownPlayerPosition - (Vector2)transform.position).normalized;
        }

        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0)
        {
            CurrentState = EnemyState.Patrol;
        }
    }

    void HandleChase()
    {
        if (Player == null) return;

        // Бежим к игроку!
        moveDirection = (Player.position - transform.position).normalized;
        lastKnownPlayerPosition = Player.position;
    }

    bool CanSeePlayer()
    {
        if (Player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                Player = playerObj.transform;
            else
                return false;
        }

        // Игрок в укрытии?
        PlayerController playerController = Player.GetComponent<PlayerController>();
        if (playerController != null && playerController.IsHidden)
            return false;

        Vector2 directionToPlayer = Player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Проверка дистанции
        if (distanceToPlayer > ViewDistance)
            return false;

        // Проверка угла
        float angle = Vector2.Angle(GetFacingDirection(), directionToPlayer);
        if (angle > ViewAngle / 2f)
            return false;

        // Проверка препятствий — ИГНОРИРУЕМ СЕБЯ!
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer.normalized, distanceToPlayer);

        foreach (var hit in hits)
        {
            // Пропускаем СЕБЯ
            if (hit.collider.gameObject == gameObject)
                continue;

            // Пропускаем триггеры (кусты, зоны и т.д.)
            if (hit.collider.isTrigger)
                continue;

            // Если это игрок — видим!
            if (hit.collider.CompareTag("Player"))
                return true;

            // Что-то другое мешает (стена)
            return false;
        }

        // Ничего не мешает — видим игрока!
        return true;
    }

    Vector2 GetFacingDirection()
    {
        // Направление куда смотрит враг
        if (moveDirection.magnitude > 0.1f)
            return moveDirection.normalized;

        return SpriteRenderer.flipX ? Vector2.left : Vector2.right;
    }

    // Визуализация поля зрения в редакторе
    void OnDrawGizmosSelected()
    {
        Vector2 facing = GetFacingDirection();

        // Конус зрения
        Gizmos.color = CurrentState == EnemyState.Chase ? Color.red : Color.yellow;

        Vector2 leftDir = Quaternion.Euler(0, 0, ViewAngle / 2f) * facing;
        Vector2 rightDir = Quaternion.Euler(0, 0, -ViewAngle / 2f) * facing;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDir * ViewDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDir * ViewDistance);
        Gizmos.DrawLine(
            (Vector2)transform.position + leftDir * ViewDistance,
            (Vector2)transform.position + rightDir * ViewDistance
        );

        // Точки патруля
        Gizmos.color = Color.blue;
        foreach (var point in PatrolPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.3f);
        }
    }
}