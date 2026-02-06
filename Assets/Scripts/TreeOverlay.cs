using UnityEngine;

public class TreeOverlay : MonoBehaviour
{
    [Header("Оверлейный спрайт")]
    public Sprite OverlaySprite;

    [Header("Sorting")]
    public string OverlaySortingLayer = "Player";
    public int BaseOverlayOrder = 10000;

    [Header("Настройки")]
    public bool UseYForSorting = true;
    public float TreeBaseY = 0f;

    [Header("Смещение спрайта (подстрой под свой спрайт!)")]
    public Vector2 SpriteOffset = new Vector2(0f, 2f);

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
        overlayObj.transform.localPosition = new Vector3(SpriteOffset.x, SpriteOffset.y, 0);
        overlayObj.transform.localRotation = Quaternion.identity;
        overlayObj.transform.localScale = Vector3.one;

        overlayRenderer = overlayObj.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = OverlaySprite;
        overlayRenderer.sortingLayerName = OverlaySortingLayer;

        UpdateSorting();

        // ДЕБАГ
        Debug.Log($"TreeOverlay создан: TreeBaseY={TreeBaseY}, Order={overlayRenderer.sortingOrder}");
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