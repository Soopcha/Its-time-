using System.Collections.Generic;
using UnityEngine;

public class Tile2DAuto : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    [Range(1, 100)]
    public int Weight = 50;

    [Header("Tile's zoneee")]
    public List<ZoneType> AllowedZones = new List<ZoneType>();  // ? СПИСОК!

    // Типы зон
    public enum ZoneType
    {
        Swamp,    // Болото
        Field,    // Поле
        Forest    // Лес
    }

    // Проверка: может ли тайл быть в этой зоне?
    public bool CanBeInZone(ZoneType zone)
    {
        // Если список пустой — может быть везде
        if (AllowedZones.Count == 0)
            return true;

        return AllowedZones.Contains(zone);
    }

    // Цвета краёв — заполняются автоматически
    [HideInInspector] public Color[] TopEdge;
    [HideInInspector] public Color[] BottomEdge;
    [HideInInspector] public Color[] LeftEdge;
    [HideInInspector] public Color[] RightEdge;

    // Вызвать один раз при старте
    public void CalculateEdges()
    {
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();

        Sprite sprite = SpriteRenderer.sprite;
        Texture2D tex = sprite.texture;

        // Проверяем что текстура читаемая
        if (!tex.isReadable)
        {
            Debug.LogError($"Текстура {tex.name} должна быть Read/Write Enabled!");
            return;
        }

        Rect rect = sprite.rect;
        int width = (int)rect.width;
        int height = (int)rect.height;
        int startX = (int)rect.x;
        int startY = (int)rect.y;

        TopEdge = new Color[width];
        BottomEdge = new Color[width];
        LeftEdge = new Color[height];
        RightEdge = new Color[height];

        // Верхний край (y = height - 1)
        for (int x = 0; x < width; x++)
        {
            TopEdge[x] = tex.GetPixel(startX + x, startY + height - 1);
        }

        // Нижний край (y = 0)
        for (int x = 0; x < width; x++)
        {
            BottomEdge[x] = tex.GetPixel(startX + x, startY);
        }

        // Левый край (x = 0)
        for (int y = 0; y < height; y++)
        {
            LeftEdge[y] = tex.GetPixel(startX, startY + y);
        }

        // Правый край (x = width - 1)
        for (int y = 0; y < height; y++)
        {
            RightEdge[y] = tex.GetPixel(startX + width - 1, startY + y);
        }
    }

    // Может ли этот тайл быть СПРАВА от other?
    public bool CanBeRightOf(Tile2DAuto other)
    {
        // Мой левый край должен совпадать с его правым
        return EdgesMatch(LeftEdge, other.RightEdge);
    }

    // Может ли этот тайл быть СЛЕВА от other?
    public bool CanBeLeftOf(Tile2DAuto other)
    {
        return EdgesMatch(RightEdge, other.LeftEdge);
    }

    // Может ли этот тайл быть СВЕРХУ от other?
    public bool CanBeAbove(Tile2DAuto other)
    {
        // Мой нижний край должен совпадать с его верхним
        return EdgesMatch(BottomEdge, other.TopEdge);
    }

    // Может ли этот тайл быть СНИЗУ от other?
    public bool CanBeBelow(Tile2DAuto other)
    {
        return EdgesMatch(TopEdge, other.BottomEdge);
    }

    private bool EdgesMatch(Color[] edge1, Color[] edge2)
    {
        if (edge1 == null || edge2 == null) return false;
        if (edge1.Length != edge2.Length) return false;

        for (int i = 0; i < edge1.Length; i++)
        {
            if (!ColorsSimilar(edge1[i], edge2[i]))
                return false;
        }
        return true;
    }

    private bool ColorsSimilar(Color a, Color b)
    {
        // Порог похожести (можно менять)
        float threshold = 0.05f;

        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold &&
               Mathf.Abs(a.a - b.a) < threshold;
    }
}