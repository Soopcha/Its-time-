using UnityEngine;

public class TreeOverlay : MonoBehaviour
{
    [Header("Оверлейный спрайт")]
    public Sprite OverlaySprite;



    [Header("Sorting")]
    public string OverlaySortingLayer = "Player";  // Такой же как у персонажа!
    public int BaseOverlayOrder = 10000;

    [Header("Настройки")]
    public bool UseYForSorting = true;
    public float TreeBaseY = 0f;  // Y основания дерева (вычисляется автоматически)

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
        overlayObj.transform.localPosition = Vector3.zero;
        overlayObj.transform.localRotation = Quaternion.identity;
        overlayObj.transform.localScale = Vector3.one;

        overlayRenderer = overlayObj.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = OverlaySprite;
        overlayRenderer.sortingLayerName = OverlaySortingLayer;

        UpdateSorting();
    }

    void UpdateSorting()
    {
        if (overlayRenderer == null) return;

        // Используем TreeBaseY вместо transform.position.y!
        float yForSorting = TreeBaseY;
        overlayRenderer.sortingOrder = BaseOverlayOrder - Mathf.RoundToInt(yForSorting * 10);
    }

    void LateUpdate()
    {
        UpdateSorting();
    }
}