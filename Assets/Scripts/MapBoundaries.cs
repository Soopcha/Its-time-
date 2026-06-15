using UnityEngine;

public class MapBoundaries : MonoBehaviour
{
    [Header("Ссылки")]
    public WFCGenerator2DAuto Generator;

    [Header("Настройки стен")]
    public float WallThickness = 2f;
    [Tooltip("На сколько тайлов раньше края карты ставить стены")]
    public float SafeZoneTiles = 1f;  // Стены будут на 1 тайл раньше края
    public bool CreateVisibleWalls = false;  // Выключил чтобы были невидимы
    public Sprite WallSprite;
    public Color WallColor = new Color(1, 0, 0, 0.3f);

    void Start()
    {
        Invoke(nameof(CreateBoundaries), 1.5f);
    }

    void CreateBoundaries()
    {
        if (Generator == null)
        {
            Debug.LogError("Generator не назначен в MapBoundaries!");
            return;
        }

        Vector2Int mapSize = Generator.MapSize;
        float tileSize = Generator.TileSize;

        float mapWidth = mapSize.x * tileSize;
        float mapHeight = mapSize.y * tileSize;

        // Смещение на пол тайла
        float offsetX = -0.5f * tileSize;
        float offsetY = -0.5f * tileSize;

        // Отступ для стен (на 1 тайл раньше края)
        float safeZone = SafeZoneTiles * tileSize;

        Debug.Log($"Размер карты: {mapWidth} x {mapHeight}");
        Debug.Log($"Безопасная зона: {SafeZoneTiles} тайлов ({safeZone} единиц)");

        // ===== СЕВЕР (ВЕРХ) - на 1 тайл НИЖЕ верхнего края =====
        CreateWall("BoundaryNorth",
            new Vector2(mapWidth / 2f + offsetX, mapHeight - safeZone + offsetY),
            new Vector2(mapWidth + WallThickness * 2, WallThickness));

        // ===== ЮГ (НИЗ) - на 1 тайл ВЫШЕ нижнего края =====
        CreateWall("BoundarySouth",
            new Vector2(mapWidth / 2f + offsetX, safeZone + offsetY),
            new Vector2(mapWidth + WallThickness * 2, WallThickness));

        // ===== ЗАПАД (ЛЕВО) - на 1 тайл ПРАВЕЕ левого края =====
        CreateWall("BoundaryWest",
            new Vector2(safeZone + offsetX, mapHeight / 2f + offsetY),
            new Vector2(WallThickness, mapHeight + WallThickness * 2));

        // ===== ВОСТОК (ПРАВО) - на 1 тайл ЛЕВЕЕ правого края =====
        CreateWall("BoundaryEast",
            new Vector2(mapWidth - safeZone + offsetX, mapHeight / 2f + offsetY),
            new Vector2(WallThickness, mapHeight + WallThickness * 2));

        Debug.Log("Границы созданы!");
    }

    void CreateWall(string wallName, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.parent = transform;
        wall.transform.position = position;

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = false;

        Debug.Log($"{wallName}: pos={position}, size={size}");

        if (CreateVisibleWalls)
        {
            SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();

            if (WallSprite != null)
            {
                sr.sprite = WallSprite;
                sr.drawMode = SpriteDrawMode.Tiled;
                sr.size = size;
            }
            else
            {
                sr.sprite = CreateSimpleSquareSprite();
                sr.drawMode = SpriteDrawMode.Tiled;
                sr.size = size;
            }

            sr.color = WallColor;
            sr.sortingOrder = 1000;
        }
    }

    Sprite CreateSimpleSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    void OnDrawGizmos()
    {
        if (Generator == null) return;

        Vector2Int mapSize = Generator.MapSize;
        float tileSize = Generator.TileSize;
        float mapWidth = mapSize.x * tileSize;
        float mapHeight = mapSize.y * tileSize;

        float offsetX = -0.5f * tileSize;
        float offsetY = -0.5f * tileSize;
        float safeZone = SafeZoneTiles * tileSize;

        // Рисуем ПОЛНЫЕ границы карты (зелёный)
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3(mapWidth / 2f + offsetX, mapHeight / 2f + offsetY, 0),
            new Vector3(mapWidth, mapHeight, 0)
        );

        // Рисуем стены (красный) - они ВНУТРИ карты
        Gizmos.color = Color.red;

        // Верх
        Gizmos.DrawWireCube(
            new Vector3(mapWidth / 2f + offsetX, mapHeight - safeZone + offsetY, 0),
            new Vector3(mapWidth, WallThickness, 0)
        );

        // Низ
        Gizmos.DrawWireCube(
            new Vector3(mapWidth / 2f + offsetX, safeZone + offsetY, 0),
            new Vector3(mapWidth, WallThickness, 0)
        );

        // Лево
        Gizmos.DrawWireCube(
            new Vector3(safeZone + offsetX, mapHeight / 2f + offsetY, 0),
            new Vector3(WallThickness, mapHeight, 0)
        );

        // Право
        Gizmos.DrawWireCube(
            new Vector3(mapWidth - safeZone + offsetX, mapHeight / 2f + offsetY, 0),
            new Vector3(WallThickness, mapHeight, 0)
        );

        // Рисуем БЕЗОПАСНУЮ зону (жёлтый) - где могут спавниться объекты
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3(mapWidth / 2f + offsetX, mapHeight / 2f + offsetY, 0),
            new Vector3(mapWidth - safeZone * 2, mapHeight - safeZone * 2, 0)
        );
    }
}