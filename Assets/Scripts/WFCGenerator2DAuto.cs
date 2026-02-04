using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WFCGenerator2DAuto : MonoBehaviour
{
    [Header("Тайлы")]
    public List<Tile2DAuto> TilePrefabs;

    [Header("Размер карты")]
    public Vector2Int MapSize = new Vector2Int(20, 20);
    public float TileSize = 1f;

    [Header("Шум Перлина")]
    public PerlinNoiseMap PerlinNoise;   // ? ДОБАВЬ ЭТО!
    public bool UsePerlinZones = true;    // ? Включить/выключить зоны

    private List<Tile2DAuto>[,] possibleTiles;
    private Tile2DAuto.ZoneType[,] zoneMap;  // ? Карта зон

    void Start()
    {
        // Считаем края всех тайлов
        foreach (var tile in TilePrefabs)
        {
            tile.CalculateEdges();
        }

        Generate();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ClearMap();
            Generate();
        }
    }

    void Generate()
    {
        // ШАГ 1: Генерируем карту зон через Perlin Noise
        if (UsePerlinZones && PerlinNoise != null)
        {
            PerlinNoise.RandomizeOffset();  // Новый рандом каждый раз
            GenerateZoneMap();
        }

        // ШАГ 2: Инициализируем WFC
        possibleTiles = new List<Tile2DAuto>[MapSize.x, MapSize.y];

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                // Фильтруем тайлы по зоне!
                possibleTiles[x, y] = GetTilesForZone(x, y);
            }
        }

        // ШАГ 3: Основной цикл WFC
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
                Debug.LogWarning($"Тупик в позиции {pos}! Перегенерация...");
                Generate();
                return;
            }

            Tile2DAuto chosen = ChooseRandomTile(possibleTiles[pos.x, pos.y]);
            possibleTiles[pos.x, pos.y] = new List<Tile2DAuto> { chosen };

            Propagate(pos);
        }

        PlaceTiles();
    }

    void GenerateZoneMap()
    {
        zoneMap = new Tile2DAuto.ZoneType[MapSize.x, MapSize.y];

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                // Получаем тип зоны из PerlinNoiseMap
                PerlinNoiseMap.ZoneType perlinZone = PerlinNoise.GetZoneType(x, y, MapSize.x, MapSize.y);

                // Конвертируем в тип из Tile2DAuto
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

    // НОВЫЙ МЕТОД: Получить тайлы подходящие для зоны
    List<Tile2DAuto> GetTilesForZone(int x, int y)
    {
        if (!UsePerlinZones || zoneMap == null)
        {
            return new List<Tile2DAuto>(TilePrefabs);
        }

        Tile2DAuto.ZoneType zone = zoneMap[x, y];

        // Фильтруем: тайлы у которых эта зона в списке AllowedZones
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
        float totalWeight = tiles.Sum(t => t.Weight);
        float random = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var tile in tiles)
        {
            current += tile.Weight;
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
                    Instantiate(prefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}