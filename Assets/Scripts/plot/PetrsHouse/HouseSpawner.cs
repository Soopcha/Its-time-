using UnityEngine;

public class HouseSpawner : MonoBehaviour
{
    [Header("Префаб дома")]
    public GameObject PetrsHousePrefab;

    [Header("Настройки спавна")]
    public WFCGenerator2DAuto Generator;
    public int MaxAttempts = 50;

    void Start()
    {
        // Подождём пока карта сгенерируется
        Invoke(nameof(SpawnHouse), 0.5f);
    }

    void SpawnHouse()
    {
        if (Generator == null || PetrsHousePrefab == null)
        {
            Debug.LogError("Generator или PetrsHousePrefab не назначен!");
            return;
        }

        Vector2Int mapSize = Generator.MapSize;
        float tileSize = Generator.TileSize;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            int x = Random.Range(0, mapSize.x);
            int y = Random.Range(0, mapSize.y);

            // Проверяем что это лес
            if (IsForestTile(x, y))
            {
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                Instantiate(PetrsHousePrefab, position, Quaternion.identity);

                Debug.Log($"Дом Петра заспавнен в ({x}, {y})");
                return;
            }
        }

        Debug.LogWarning("Не удалось найти подходящее место для дома!");
    }

    bool IsForestTile(int x, int y)
    {
        if (Generator.PerlinNoise == null) return false;

        var zoneType = Generator.PerlinNoise.GetZoneType(x, y,
            Generator.MapSize.x, Generator.MapSize.y);

        return zoneType == PerlinNoiseMap.ZoneType.Forest;
    }
}