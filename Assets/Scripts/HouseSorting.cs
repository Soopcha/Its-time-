using UnityEngine;

public class HouseSorting : MonoBehaviour
{
    [Header("Сортировка")]
    public int BaseSortingOrder = 10000;
    public string SortingLayer = "Player";

    [Header("Отступ")]
    [Tooltip("Отрицательное число сдвинет точку сортировки вниз. Настрой так, чтобы она была на линии фундамента.")]
    public float YOffset = -5f; // Попробуй поставить от -1.0 до -2.0

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"HouseSorting на {gameObject.name}: нет SpriteRenderer!");
            return;
        }

        spriteRenderer.sortingLayerName = SortingLayer;
    }

    void LateUpdate()
    {
        if (spriteRenderer != null)
        {
            // Добавляем отступ к текущей позиции Y
            float sortY = transform.position.y + YOffset;
            spriteRenderer.sortingOrder = BaseSortingOrder - Mathf.RoundToInt(sortY * 10);
        }
    }

    // Эта штука нарисует синюю точку в редакторе, чтобы ты видела, где сейчас точка сортировки
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 pos = transform.position;
        pos.y += YOffset;
        Gizmos.DrawSphere(pos, 0.2f);
    }
}