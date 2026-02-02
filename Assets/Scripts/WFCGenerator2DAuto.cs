using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WFCGenerator2DAuto : MonoBehaviour
{
    public List<Tile2DAuto> TilePrefabs;
    public Vector2Int MapSize = new Vector2Int(10, 10);
    public float TileSize = 1f;

    private List<Tile2DAuto>[,] possibleTiles;

    void Start()
    {
        // Сначала посчитать края всех тайлов
        foreach (var tile in TilePrefabs)
        {
            tile.CalculateEdges();
        }

        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearMap();
            Generate();
        }
    }

    void Generate()
    {
        possibleTiles = new List<Tile2DAuto>[MapSize.x, MapSize.y];

        // Инициализация — везде возможны все тайлы
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                possibleTiles[x, y] = new List<Tile2DAuto>(TilePrefabs);
            }
        }

        // Основной цикл WFC
        int iterations = 0;
        int maxIterations = MapSize.x * MapSize.y * 10;

        while (iterations++ < maxIterations)
        {
            // Найти клетку с минимальной энтропией
            Vector2Int? minPos = FindMinEntropyCell();

            if (minPos == null)
                break;

            Vector2Int pos = minPos.Value;

            // Коллапс
            if (possibleTiles[pos.x, pos.y].Count == 0)
            {
                Debug.LogWarning($"Тупик в позиции {pos}! Перегенерация...");
                Generate();
                return;
            }

            Tile2DAuto chosen = ChooseRandomTile(possibleTiles[pos.x, pos.y]);
            possibleTiles[pos.x, pos.y] = new List<Tile2DAuto> { chosen };

            // Распространение
            Propagate(pos);
        }

        PlaceTiles();
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

            // Проверяем 4 соседа
            TryConstrain(pos, new Vector2Int(1, 0), here, queue);  // справа
            TryConstrain(pos, new Vector2Int(-1, 0), here, queue); // слева
            TryConstrain(pos, new Vector2Int(0, 1), here, queue);  // сверху
            TryConstrain(pos, new Vector2Int(0, -1), here, queue); // снизу
        }
    }

    void TryConstrain(Vector2Int pos, Vector2Int offset, List<Tile2DAuto> here, Queue<Vector2Int> queue)
    {
        Vector2Int neighborPos = pos + offset;

        // Проверка границ
        if (neighborPos.x < 0 || neighborPos.x >= MapSize.x ||
            neighborPos.y < 0 || neighborPos.y >= MapSize.y)
            return;

        List<Tile2DAuto> neighborPossible = possibleTiles[neighborPos.x, neighborPos.y];

        int before = neighborPossible.Count;

        // Убираем тайлы которые не совместимы ни с одним из текущих
        neighborPossible.RemoveAll(neighborTile =>
        {
            // Сосед должен быть совместим хотя бы с одним тайлом тут
            foreach (var hereTile in here)
            {
                if (IsCompatible(hereTile, neighborTile, offset))
                    return false; // НЕ удалять — совместим
            }
            return true; // Удалить — ни с кем не совместим
        });

        if (neighborPossible.Count < before)
        {
            queue.Enqueue(neighborPos);
        }
    }

    bool IsCompatible(Tile2DAuto hereTile, Tile2DAuto neighborTile, Vector2Int offset)
    {
        // offset показывает где сосед относительно нас
        if (offset == new Vector2Int(1, 0))
        {
            // Сосед СПРАВА от нас
            // Наш правый край должен совпадать с его левым
            return neighborTile.CanBeRightOf(hereTile);
        }
        else if (offset == new Vector2Int(-1, 0))
        {
            // Сосед СЛЕВА от нас
            return neighborTile.CanBeLeftOf(hereTile);
        }
        else if (offset == new Vector2Int(0, 1))
        {
            // Сосед СВЕРХУ от нас
            return neighborTile.CanBeAbove(hereTile);
        }
        else if (offset == new Vector2Int(0, -1))
        {
            // Сосед СНИЗУ от нас
            return neighborTile.CanBeBelow(hereTile);
        }

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
