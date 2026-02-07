using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WFCGenerator2DAuto : MonoBehaviour
{
    [Header("Тайлы")]
    public List<Tile2DAuto> TilePrefabs;

    private Tile2DAuto.ZoneType currentZone;

    [Header("Размер карты")]
    public Vector2Int MapSize = new Vector2Int(20, 20);
    public float TileSize = 1f;

    [Header("Шум Перлина")]
    public PerlinNoiseMap PerlinNoise;
    public bool UsePerlinZones = true;

    [Header("Настройки генерации")]
    public int MaxRetries = 20;              // Максимум попыток
    public bool ShowDebugLogs = true;        // Показывать логи

    private List<Tile2DAuto>[,] possibleTiles;
    private Tile2DAuto.ZoneType[,] zoneMap;

    // Счётчики для статистики
    private int currentRetryCount = 0;
    private int totalDeadEnds = 0;

    [Header("Оверлеи для деревьев")]
    public List<TreeOverlayData> TreeOverlays = new List<TreeOverlayData>();

    [System.Serializable]
    public class TreeOverlayData
    {
        public string TileNameContains = "poplarPart";  // Часть имени тайла
        public Sprite OverlaySprite;                     // Прозрачный спрайт
    }

    void Start()
    {
        foreach (var tile in TilePrefabs)
        {
            tile.CalculateEdges();
        }

        GenerateWithRetry();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ClearMap();
            GenerateWithRetry();
        }
    }

    // НОВЫЙ МЕТОД: Генерация с повторными попытками
    void GenerateWithRetry()
    {
        currentRetryCount = 0;
        totalDeadEnds = 0;

        bool success = false;

        while (!success && currentRetryCount < MaxRetries)
        {
            currentRetryCount++;
            success = TryGenerate();

            if (!success)
            {
                ClearMap();

                if (ShowDebugLogs)
                {
                    Debug.Log($"Попытка {currentRetryCount}/{MaxRetries} не удалась. Тупиков: {totalDeadEnds}. Пробуем снова...");
                }
            }
        }

        if (success)
        {
            if (ShowDebugLogs && currentRetryCount > 1)
            {
                Debug.Log($"? Карта сгенерирована с {currentRetryCount} попытки! Всего тупиков: {totalDeadEnds}");
            }
            else if (ShowDebugLogs)
            {
                Debug.Log($"? Карта сгенерирована с первой попытки!");
            }
        }
        else
        {
            Debug.LogError($"? НЕ УДАЛОСЬ сгенерировать карту за {MaxRetries} попыток! Тупиков: {totalDeadEnds}");
            Debug.LogWarning("Возможные причины:");
            Debug.LogWarning("  - Тайлы плохо стыкуются между собой");
            Debug.LogWarning("  - Слишком мало тайлов для какой-то зоны");
            Debug.LogWarning("  - Края тайлов не совпадают по цветам");
        }
    }

    // НОВЫЙ МЕТОД: Попытка генерации (возвращает true если успех)
    bool TryGenerate()
    {
        // ШАГ 1: Генерируем карту зон
        if (UsePerlinZones && PerlinNoise != null)
        {
            PerlinNoise.RandomizeOffset();
            GenerateZoneMap();
        }

        // ШАГ 2: Инициализируем WFC
        possibleTiles = new List<Tile2DAuto>[MapSize.x, MapSize.y];

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                possibleTiles[x, y] = GetTilesForZone(x, y);

                // Проверяем что есть хотя бы один тайл для этой клетки
                if (possibleTiles[x, y].Count == 0)
                {
                    if (ShowDebugLogs)
                    {
                        Tile2DAuto.ZoneType zone = GetZoneAtPosition(x, y);
                        Debug.LogWarning($"Нет тайлов для зоны {zone} в позиции ({x}, {y})!");
                    }
                    return false;
                }
            }
        }

        // ШАГ 3: Основной цикл WFC
        int iterations = 0;
        int maxIterations = MapSize.x * MapSize.y * 10;
        int deadEndsThisRun = 0;

        while (iterations++ < maxIterations)
        {
            Vector2Int? minPos = FindMinEntropyCell();

            if (minPos == null)
                break;

            Vector2Int pos = minPos.Value;

            // Проверка на тупик
            if (possibleTiles[pos.x, pos.y].Count == 0)
            {
                deadEndsThisRun++;
                totalDeadEnds++;
                return false;  // Тупик — возвращаем неудачу
            }

            currentZone = GetZoneAtPosition(pos.x, pos.y);
            Tile2DAuto chosen = ChooseRandomTile(possibleTiles[pos.x, pos.y]);
            possibleTiles[pos.x, pos.y] = new List<Tile2DAuto> { chosen };

            Propagate(pos);
        }

        // ШАГ 4: Проверяем что ВСЕ клетки заполнены
        if (!IsMapFullyGenerated())
        {
            if (ShowDebugLogs)
            {
                int emptyCount = CountEmptyCells();
                Debug.LogWarning($"Карта не полностью заполнена! Пустых клеток: {emptyCount}");
            }
            return false;
        }

        // ШАГ 5: Размещаем тайлы
        PlaceTiles();

        return true;
    }

    // НОВЫЙ МЕТОД: Проверка что все клетки заполнены
    bool IsMapFullyGenerated()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                if (possibleTiles[x, y].Count != 1)
                    return false;
            }
        }
        return true;
    }

    // НОВЫЙ МЕТОД: Подсчёт пустых клеток
    int CountEmptyCells()
    {
        int count = 0;
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                if (possibleTiles[x, y].Count != 1)
                    count++;
            }
        }
        return count;
    }

    Tile2DAuto.ZoneType GetZoneAtPosition(int x, int y)
    {
        if (!UsePerlinZones || zoneMap == null)
            return Tile2DAuto.ZoneType.Field;

        return zoneMap[x, y];
    }

    void GenerateZoneMap()
    {
        zoneMap = new Tile2DAuto.ZoneType[MapSize.x, MapSize.y];

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                PerlinNoiseMap.ZoneType perlinZone = PerlinNoise.GetZoneType(x, y, MapSize.x, MapSize.y);

                switch (perlinZone)
                {
                    case PerlinNoiseMap.ZoneType.Forest:
                        zoneMap[x, y] = Tile2DAuto.ZoneType.Forest;
                        break;
                    case PerlinNoiseMap.ZoneType.Field:
                        zoneMap[x, y] = Tile2DAuto.ZoneType.Field;
                        break;
                    case PerlinNoiseMap.ZoneType.Swamp:
                        zoneMap[x, y] = Tile2DAuto.ZoneType.Swamp;
                        break;
                }
            }
        }
    }

    List<Tile2DAuto> GetTilesForZone(int x, int y)
    {
        if (!UsePerlinZones || zoneMap == null)
        {
            return new List<Tile2DAuto>(TilePrefabs);
        }

        Tile2DAuto.ZoneType zone = zoneMap[x, y];

        return TilePrefabs
            .Where(t => t.CanBeInZone(zone))
            .ToList();
    }

    Vector2Int? FindMinEntropyCell()
    {
        int minCount = int.MaxValue;
        List<Vector2Int> candidates = new List<Vector2Int>();

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                int count = possibleTiles[x, y].Count;

                if (count > 1)
                {
                    if (count < minCount)
                    {
                        minCount = count;
                        candidates.Clear();
                        candidates.Add(new Vector2Int(x, y));
                    }
                    else if (count == minCount)
                    {
                        candidates.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    Tile2DAuto ChooseRandomTile(List<Tile2DAuto> tiles)
    {
        float totalWeight = 0f;
        foreach (var tile in tiles)
        {
            totalWeight += tile.GetWeightForZone(currentZone);
        }

        if (totalWeight <= 0)
        {
            return tiles[Random.Range(0, tiles.Count)];
        }

        float random = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var tile in tiles)
        {
            current += tile.GetWeightForZone(currentZone);
            if (random <= current)
                return tile;
        }

        return tiles[tiles.Count - 1];
    }

    void Propagate(Vector2Int startPos)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPos);

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        while (queue.Count > 0)
        {
            Vector2Int pos = queue.Dequeue();

            if (visited.Contains(pos))
                continue;
            visited.Add(pos);

            List<Tile2DAuto> here = possibleTiles[pos.x, pos.y];

            TryConstrain(pos, new Vector2Int(1, 0), here, queue);
            TryConstrain(pos, new Vector2Int(-1, 0), here, queue);
            TryConstrain(pos, new Vector2Int(0, 1), here, queue);
            TryConstrain(pos, new Vector2Int(0, -1), here, queue);
        }
    }

    void TryConstrain(Vector2Int pos, Vector2Int offset, List<Tile2DAuto> here, Queue<Vector2Int> queue)
    {
        Vector2Int neighborPos = pos + offset;

        if (neighborPos.x < 0 || neighborPos.x >= MapSize.x ||
            neighborPos.y < 0 || neighborPos.y >= MapSize.y)
            return;

        List<Tile2DAuto> neighborPossible = possibleTiles[neighborPos.x, neighborPos.y];

        int before = neighborPossible.Count;

        neighborPossible.RemoveAll(neighborTile =>
        {
            foreach (var hereTile in here)
            {
                if (IsCompatible(hereTile, neighborTile, offset))
                    return false;
            }
            return true;
        });

        if (neighborPossible.Count < before)
        {
            queue.Enqueue(neighborPos);
        }
    }

    bool IsCompatible(Tile2DAuto hereTile, Tile2DAuto neighborTile, Vector2Int offset)
    {
        if (offset == new Vector2Int(1, 0))
            return neighborTile.CanBeRightOf(hereTile);
        else if (offset == new Vector2Int(-1, 0))
            return neighborTile.CanBeLeftOf(hereTile);
        else if (offset == new Vector2Int(0, 1))
            return neighborTile.CanBeAbove(hereTile);
        else if (offset == new Vector2Int(0, -1))
            return neighborTile.CanBeBelow(hereTile);

        return false;
    }

    //void PlaceTiles()
    //{
    //    for (int x = 0; x < MapSize.x; x++)
    //    {
    //        for (int y = 0; y < MapSize.y; y++)
    //        {
    //            if (possibleTiles[x, y].Count == 1)
    //            {
    //                Tile2DAuto prefab = possibleTiles[x, y][0];
    //                Vector3 position = new Vector3(x * TileSize, y * TileSize, 0);
    //                GameObject placed = Instantiate(prefab.gameObject, position, Quaternion.identity, transform);

    //                // Проверяем нужен ли оверлей
    //                TryAddOverlay(placed, prefab.name);
    //            }
    //        }
    //    }
    //}

    void PlaceTiles()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                if (possibleTiles[x, y].Count == 1)
                {
                    Tile2DAuto prefab = possibleTiles[x, y][0];
                    Vector3 position = new Vector3(x * TileSize, y * TileSize, 0);
                    GameObject placed = Instantiate(prefab.gameObject, position, Quaternion.identity, transform);

                    SpriteRenderer sr = placed.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.sortingOrder = 0;
                    }

                    // Передаём Y позицию!
                    TryAddOverlay(placed, prefab.name, y * TileSize);
                }
            }
        }
    }
    //void TryAddOverlay(GameObject tile, string tileName)
    //{
    //    foreach (var data in TreeOverlays)
    //    {
    //        if (tileName.Contains(data.TileNameContains) && data.OverlaySprite != null)
    //        {
    //            TreeOverlay overlay = tile.AddComponent<TreeOverlay>();
    //            overlay.OverlaySprite = data.OverlaySprite;
    //            overlay.UseYForSorting = true;
    //            overlay.BaseOverlayOrder = 100;
    //            return;
    //        }
    //    }
    //}

    // В TryAddOverlay добавь вычисление базовой Y:

    void TryAddOverlay(GameObject tile, string tileName, float tileY)
    {
        if (!tileName.Contains("poplarPart1_0"))
            return;

        foreach (var data in TreeOverlays)
        {
            if (data.OverlaySprite != null)
            {
                TreeOverlay overlay = tile.AddComponent<TreeOverlay>();
                overlay.OverlaySprite = data.OverlaySprite;
                overlay.BaseOverlayOrder = 10000;
                overlay.OverlaySortingLayer = "Player";
                overlay.TreeBaseY = tileY;
                overlay.PixelsPerUnit = 32f;
                // УБРАЛ: overlay.UseYForSorting = true;
                // УБРАЛ: overlay.SpriteOffset = ...
                // УБРАЛ: overlay.AutoAlignToTile = ...

                return;
            }
        }
    }

    float CalculateTreeBaseY(string tileName, float currentY)
    {
        // Части 1-2 = перед (ближе к камере) - это ОСНОВАНИЕ для сортировки
        // Части 3-4 = выше на 1
        // Части 5-6 = выше на 2
        // Части 7-8 = верх (самые дальние)

        // Нам нужно вычислить Y частей 1-2 (основания) для ВСЕХ частей

        if (tileName.Contains("poplarPart1") || tileName.Contains("poplarPart2"))
            return currentY;  // Это и есть основание
        if (tileName.Contains("poplarPart3") || tileName.Contains("poplarPart4"))
            return currentY - TileSize;  // На 1 тайл ниже = Y основания
        if (tileName.Contains("poplarPart5") || tileName.Contains("poplarPart6"))
            return currentY - TileSize * 2;  // На 2 тайла ниже
        if (tileName.Contains("poplarPart7") || tileName.Contains("poplarPart8"))
            return currentY - TileSize * 3;  // На 3 тайла ниже

        return currentY;
    }
    void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}