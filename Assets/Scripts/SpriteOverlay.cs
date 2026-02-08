using UnityEngine;

public class SpriteOverlay : MonoBehaviour
{
    [Header("Оверлейный спрайт")]
    public Sprite OverlaySprite;

    [Header("Sorting")]
    public string OverlaySortingLayer = "Player";
    public int BaseOverlayOrder = 10000;

    [Header("Настройки")]
    public float BaseY = 0f;

    [Header("Pixels Per Unit")]
    public float PixelsPerUnit = 32f;

    [Header("Смещение (в тайлах)")]
    public Vector2 OffsetInTiles = new Vector2(-0.5f, 1.5f);  // По умолчанию как для дерева

    private SpriteRenderer overlayRenderer;

    void Start()
    {
        CreateOverlay();
    }

    void CreateOverlay()
    {
        if (OverlaySprite == null) return;

        GameObject overlayObj = new GameObject("Overlay");
        overlayObj.transform.SetParent(transform);
        overlayObj.transform.localRotation = Quaternion.identity;
        overlayObj.transform.localScale = Vector3.one;

        overlayRenderer = overlayObj.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = OverlaySprite;
        overlayRenderer.sortingLayerName = OverlaySortingLayer;

        // Вычисляем смещение как раньше
        Vector3 offset = CalculateOffset();
        overlayObj.transform.localPosition = offset;

        UpdateSorting();
    }

    Vector3 CalculateOffset()
    {
        float tileSize = 32f / PixelsPerUnit;  // = 1 юнит при PPU=32

        float offsetX = OffsetInTiles.x * tileSize;
        float offsetY = OffsetInTiles.y * tileSize;

        return new Vector3(offsetX, offsetY, 0);
    }

    void UpdateSorting()
    {
        if (overlayRenderer == null) return;
        overlayRenderer.sortingOrder = BaseOverlayOrder - Mathf.RoundToInt(BaseY * 10);
    }

    void LateUpdate()
    {
        UpdateSorting();
    }
}