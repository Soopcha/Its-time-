using UnityEngine;

public class HouseSpawner : MonoBehaviour
{
    [Header("Префаб дома")]
    public GameObject PetrsHousePrefab;

    [Header("Настройки спавна")]
    public WFCGenerator2DAuto Generator;
    public int MaxAttempts = 50;

    [Header("Безопасная зона")]
    [Tooltip("Отступ от края карты в тайлах")]
    public int SafeZoneTiles = 5;  // Не спавнится ближе 3 тайлов к краю

    [Header("Не спавнить если загрузка")]
    public bool SpawnOnlyOnNewGame = true;

    void Start()
    {
        bool shouldGenerate = PlayerPrefs.GetInt("ShouldGenerate", 1) == 1;

        if (SpawnOnlyOnNewGame && !shouldGenerate)
        {
            Debug.Log("Дом НЕ спавнится - идёт загрузка из сохранения");
            return;
        }

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
            // Спавним с отступом от краёв
            int x = Random.Range(SafeZoneTiles, mapSize.x - SafeZoneTiles);
            int y = Random.Range(SafeZoneTiles, mapSize.y - SafeZoneTiles);

            if (IsForestTile(x, y))
            {
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                GameObject house = Instantiate(PetrsHousePrefab, position, Quaternion.identity);

                SaveableObject saveable = house.GetComponent<SaveableObject>();
                if (saveable == null)
                {
                    saveable = house.AddComponent<SaveableObject>();
                }
                saveable.SavePosition = true;
                saveable.SaveActiveState = true;

                Debug.Log($"Дом Петра заспавнен в ({x}, {y}) - в безопасной зоне");
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