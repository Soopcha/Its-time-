using UnityEngine;

public class DynamicSorting : MonoBehaviour
{
    [Header("Сортировка")]
    public int BaseSortingOrder = 10000;
    public string SortingLayer = "Player";

    [Header("Отступ")]
    [Tooltip("Смещение точки сортировки по Y. Отрицательное = ниже, положительное = выше")]
    public float YOffset = 0f;

    [Header("Режим")]
    [Tooltip("Если включено — сортируется относительно РОДИТЕЛЯ (для дочерних объектов типа перил)")]
    public bool UseParentPosition = false;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"DynamicSorting на {gameObject.name}: нет SpriteRenderer!");
            return;
        }

        spriteRenderer.sortingLayerName = SortingLayer;
    }

    void LateUpdate()
    {
        if (spriteRenderer != null)
        {
            float sortY;

            if (UseParentPosition && transform.parent != null)
            {
                // Сортировка по позиции родителя (для перил, окон и т.д.)
                sortY = transform.parent.position.y + YOffset;
            }
            else
            {
                // Сортировка по собственной позиции (для домов, деревьев и т.д.)
                sortY = transform.position.y + YOffset;
            }

            spriteRenderer.sortingOrder = BaseSortingOrder - Mathf.RoundToInt(sortY * 10);
        }
    }

    // Визуализация точки сортировки
    void OnDrawGizmosSelected()
    {
        Vector3 pos;

        if (UseParentPosition && transform.parent != null)
        {
            pos = transform.parent.position;
            Gizmos.color = Color.red;  // Красная точка для дочерних объектов
        }
        else
        {
            pos = transform.position;
            Gizmos.color = Color.blue;  // Синяя точка для основных объектов
        }

        pos.y += YOffset;
        Gizmos.DrawSphere(pos, 0.2f);
    }
}