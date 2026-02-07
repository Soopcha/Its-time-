using UnityEngine;

public class TreeOverlay : MonoBehaviour
{
    [Header("Оверлейный спрайт")]
    public Sprite OverlaySprite;

    [Header("Sorting")]
    public string OverlaySortingLayer = "Player";
    public int BaseOverlayOrder = 10000;

    [Header("Настройки")]
    public float TreeBaseY = 0f;

    [Header("Pixels Per Unit")]
    public float PixelsPerUnit = 32f;

    private SpriteRenderer overlayRenderer;

    void Start()
    {
        CreateOverlay();
    }

    void CreateOverlay()
    {
        if (OverlaySprite == null) return;

        GameObject overlayObj = new GameObject("TreeOverlay");
        overlayObj.transform.SetParent(transform);
        overlayObj.transform.localRotation = Quaternion.identity;
        overlayObj.transform.localScale = Vector3.one;

        overlayRenderer = overlayObj.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = OverlaySprite;
        overlayRenderer.sortingLayerName = OverlaySortingLayer;

        Vector3 offset = CalculateOffset();
        overlayObj.transform.localPosition = offset;

        UpdateSorting();
    }

    Vector3 CalculateOffset()
    {
        float tileSize = 32f / PixelsPerUnit;  // = 1 юнит

        // 32 пикселя влево = -1 юнит при PPU=32
        float offsetX = -0.5f * tileSize;  // ВЛЕВО на 1 тайл
        float offsetY = 1.5f * tileSize; // Вверх на 1.5 тайла

        return new Vector3(offsetX, offsetY, 0);
    }

    void UpdateSorting()
    {
        if (overlayRenderer == null) return;
        overlayRenderer.sortingOrder = BaseOverlayOrder - Mathf.RoundToInt(TreeBaseY * 10);
    }

    void LateUpdate()
    {
        UpdateSorting();
    }
}