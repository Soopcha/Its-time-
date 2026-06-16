using System.Collections;
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
    public int MaxRetries = 20;
    public bool ShowDebugLogs = true;

    private List<Tile2DAuto>[,] possibleTiles;
    private Tile2DAuto.ZoneType[,] zoneMap;

    private int currentRetryCount = 0;
    private int totalDeadEnds = 0;

    [Header("Оверлеи для объектов")]
    public List<OverlayData> Overlays = new List<OverlayData>();

    [System.Serializable]
    public class OverlayData
    {
        public string TileNameContains = "";
        public Sprite OverlaySprite;
        public Vector2 OffsetInTiles = new Vector2(-0.5f, 1.5f);
    }

    [Header("Замена тайлов в запрещённых зонах")]
    [Tooltip("Префаб тайла который будет ставиться вместо удалённых деревьев/камней")]
    public Tile2DAuto ReplacementTile;  // ? НОВОЕ: назначь grass_0

    [Header("Загрузка")]
    public bool ShouldGenerate = true;

    private List<Bounds> forbiddenZones = new List<Bounds>();
    private List<GameObject> spawnedTiles = new List<GameObject>();

    public void AddForbiddenZone(Vector3 center, Vector2 size)
    {
        Bounds bounds = new Bounds(center, new Vector3(size.x, size.y, 0));
        forbiddenZones.Add(bounds);
        Debug.Log($" Запрещённая зона добавлена: центр={center}, размер={size}");

        ReplaceTilesInZone(bounds);  // ? ИЗМЕНЕНО: заменяем вместо удаления
    }

    void ReplaceTilesInZone(Bounds zone)
    {
        if (ReplacementTile == null)
        {
            Debug.LogWarning("?? ReplacementTile не назначен! Тайлы будут удалены.");
            RemoveTilesInZone(zone);
            return;
        }

        int replacedCount = 0;
        HashSet<GameObject> tilesToReplace = new HashSet<GameObject>();

        // Находим все тайлы в зоне
        for (int i = spawnedTiles.Count - 1; i >= 0; i--)
        {
            if (spawnedTiles[i] == null) continue;

            Vector3 tilePos = spawnedTiles[i].transform.position;

            if (zone.Contains(tilePos))
            {
                if (HasOverlay(spawnedTiles[i]))
                {
                    tilesToReplace.Add(spawnedTiles[i]);

                    // ===== НОВОЕ: Если это часть дерева — ищем соседние части =====
                    if (IsTreePart(spawnedTiles[i]))
                    {
                        FindAdjacentTreeParts(spawnedTiles[i], tilesToReplace);
                    }
                }
            }
        }

        // Заменяем все найденные тайлы
        foreach (var oldTile in tilesToReplace)
        {
            if (oldTile == null) continue;

            Vector3 tilePos = oldTile.transform.position;

            spawnedTiles.Remove(oldTile);
            Destroy(oldTile);

            // Спавним замену (траву)
            GameObject newTile = Instantiate(
                ReplacementTile.gameObject,
                tilePos,
                Quaternion.identity,
                transform
            );

            spawnedTiles.Add(newTile);
            replacedCount++;
        }

        Debug.Log($" Заменено {replacedCount} тайлов на {ReplacementTile.name}");
    }

    // ===== НОВОЕ: Проверка что тайл — это часть дерева =====
    bool IsTreePart(GameObject tile)
    {
        string tileName = tile.name.Replace("(Clone)", "");
        return tileName.Contains("Part") || tileName.Contains("part");
    }

    // ===== НОВОЕ: Рекурсивный поиск соседних частей дерева =====
    void FindAdjacentTreeParts(GameObject treePart, HashSet<GameObject> foundParts)
    {
        Vector3 pos = treePart.transform.position;
        float searchRadius = TileSize * 1.5f; // Ищем в радиусе 1.5 тайла

        foreach (var tile in spawnedTiles)
        {
            if (tile == null || foundParts.Contains(tile)) continue;

            if (IsTreePart(tile))
            {
                float distance = Vector3.Distance(pos, tile.transform.position);

                if (distance < searchRadius)
                {
                    foundParts.Add(tile);
                    // Рекурсивно ищем соседей этого тайла
                    FindAdjacentTreeParts(tile, foundParts);
                }
            }
        }
    }

    // ===== Старый метод (на случай если ReplacementTile не назначен) =====
    void RemoveTilesInZone(Bounds zone)
    {
        int removedCount = 0;

        for (int i = spawnedTiles.Count - 1; i >= 0; i--)
        {
            if (spawnedTiles[i] == null) continue;

            Vector3 tilePos = spawnedTiles[i].transform.position;

            if (zone.Contains(tilePos))
            {
                if (HasOverlay(spawnedTiles[i]))
                {
                    Debug.Log($" Удаляем тайл на позиции {tilePos}");
                    Destroy(spawnedTiles[i]);
                    spawnedTiles.RemoveAt(i);
                    removedCount++;
                }
            }
        }

        Debug.Log($" Удалено {removedCount} тайлов");
    }

    bool HasOverlay(GameObject tile)
    {
        string tileName = tile.name.Replace("(Clone)", "");

        // Проверяем по списку оверлеев
        foreach (var data in Overlays)
        {
            if (!string.IsNullOrEmpty(data.TileNameContains) &&
                tileName.Contains(data.TileNameContains))
            {
                return true;
            }
        }

        // Явно проверяем наличие tree_ и stone_ в имени
        if (tileName.Contains("tree_") || tileName.Contains("Tree") ||
            tileName.Contains("stone_") || tileName.Contains("Stone") ||
            tileName.Contains("rock_") || tileName.Contains("Rock"))
        {
            return true;
        }

        // Проверяем наличие компонента SpriteOverlay
        if (tile.GetComponent<SpriteOverlay>() != null)
        {
            return true;
        }

        return false;
    }

    void Start()
    {
        ShouldGenerate = PlayerPrefs.GetInt("ShouldGenerate", 1) == 1;
        PlayerPrefs.DeleteKey("ShouldGenerate");

        Debug.Log($"WFCGenerator.Start(): ShouldGenerate = {ShouldGenerate}");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        foreach (var tile in TilePrefabs)
        {
            tile.CalculateEdges();
        }

        if (!ShouldGenerate)
        {
            Debug.Log("Карта загружена из сохранения - генерация пропущена");

            if (LoadingScreen.Instance != null)
            {
                LoadingScreen.Instance.Hide();
            }

            return;
        }

        if (LoadingScreen.Instance != null)
        {
            LoadingScreen.Instance.Show("Генерация мира...");
        }

        StartCoroutine(GenerateAfterFrame());
    }

    IEnumerator GenerateAfterFrame()
    {
        yield return null;
        GenerateWithRetry();
        yield return null;

        if (LoadingScreen.Instance != null)
        {
            LoadingScreen.Instance.Hide();
        }
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ClearMap();
            GenerateWithRetry();
        }
    }

    public void GenerateWithRetry()
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
        }
    }

    bool TryGenerate()
    {
        if (UsePerlinZones && PerlinNoise != null)
        {
            PerlinNoise.RandomizeOffset();
            GenerateZoneMap();
        }

        possibleTiles = new List<Tile2DAuto>[MapSize.x, MapSize.y];

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                possibleTiles[x, y] = GetTilesForZone(x, y);

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

        int iterations = 0;
        int maxIterations = MapSize.x * MapSize.y * 10;

        while (iterations++ < maxIterations)
        {
            Vector2Int? minPos = FindMinEntropyCell();

            if (minPos == null)
                break;

            Vector2Int pos = minPos.Value;

            if (possibleTiles[pos.x, pos.y].Count == 0)
            {
                totalDeadEnds++;
                return false;
            }

            currentZone = GetZoneAtPosition(pos.x, pos.y);
            Tile2DAuto chosen = ChooseRandomTile(possibleTiles[pos.x, pos.y]);
            possibleTiles[pos.x, pos.y] = new List<Tile2DAuto> { chosen };

            Propagate(pos);
        }

        if (!IsMapFullyGenerated())
        {
            if (ShowDebugLogs)
            {
                int emptyCount = CountEmptyCells();
                Debug.LogWarning($"Карта не полностью заполнена! Пустых клеток: {emptyCount}");
            }
            return false;
        }

        PlaceTiles();

        return true;
    }

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

                    spawnedTiles.Add(placed);

                    SpriteRenderer sr = placed.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.sortingOrder = 0;
                    }

                    TryAddOverlay(placed, prefab.name, position);
                }
            }
        }
    }

    void TryAddOverlay(GameObject tile, string tileName, Vector3 tilePosition)
    {
        foreach (var data in Overlays)
        {
            if (string.IsNullOrEmpty(data.TileNameContains))
                continue;

            if (tileName.Contains(data.TileNameContains) && data.OverlaySprite != null)
            {
                SpriteOverlay overlay = tile.AddComponent<SpriteOverlay>();
                overlay.OverlaySprite = data.OverlaySprite;
                overlay.BaseOverlayOrder = 10000;
                overlay.OverlaySortingLayer = "Player";
                overlay.BaseY = tilePosition.y;
                overlay.PixelsPerUnit = 32f;
                overlay.OffsetInTiles = data.OffsetInTiles;

                return;
            }
        }
    }

    public void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        forbiddenZones.Clear();
        spawnedTiles.Clear();
    }
}