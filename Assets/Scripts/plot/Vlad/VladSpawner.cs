using UnityEngine;

public class VladSpawner : MonoBehaviour
{
    [Header("Префаб дома Влада")]
    public GameObject VladHousePrefab;

    [Header("Настройки спавна")]
    public WFCGenerator2DAuto Generator;
    public int MaxAttempts = 50;

    [Header("Безопасная зона")]
    public int SafeZoneTiles = 5;

    [Header("Зона очистки")]
    public Vector2 ClearZoneSize = new Vector2(3, 3);

    [Header("Не спавнить если загрузка")]
    public bool SpawnOnlyOnNewGame = true;

    void Start()
    {
        bool shouldGenerate = PlayerPrefs.GetInt("ShouldGenerate", 1) == 1;

        if (SpawnOnlyOnNewGame && !shouldGenerate)
        {
            Debug.Log("Дом Влада НЕ спавнится - идёт загрузка");
            return;
        }

        Invoke(nameof(SpawnHouse), 0.7f);
    }

    void SpawnHouse()
    {
        if (Generator == null || VladHousePrefab == null)
        {
            Debug.LogError("Generator или VladHousePrefab не назначен!");
            return;
        }

        Vector2Int mapSize = Generator.MapSize;
        float tileSize = Generator.TileSize;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            int x = Random.Range(SafeZoneTiles, mapSize.x - SafeZoneTiles);
            int y = Random.Range(SafeZoneTiles, mapSize.y - SafeZoneTiles);

            //  ТОЛЬКО БОЛОТО
            if (IsSwampTile(x, y))
            {
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                GameObject house = Instantiate(VladHousePrefab, position, Quaternion.identity);

                SaveableObject saveable = house.GetComponent<SaveableObject>();
                if (saveable == null)
                {
                    saveable = house.AddComponent<SaveableObject>();
                }
                saveable.SavePosition = true;
                saveable.SaveActiveState = true;

                Vector2 clearSize = ClearZoneSize * tileSize;
                Generator.AddForbiddenZone(position, clearSize);

                Debug.Log($" Дом Влада заспавнен НА БОЛОТЕ в ({x}, {y})");
                return;
            }
        }

        Debug.LogWarning(" Не удалось найти подходящее место на болоте для дома Влада!");
    }

    bool IsSwampTile(int x, int y)
    {
        if (Generator.PerlinNoise == null) return false;

        var zoneType = Generator.PerlinNoise.GetZoneType(x, y,
            Generator.MapSize.x, Generator.MapSize.y);

        // ВОЗВРАЩАЕТ TRUE ТОЛЬКО ДЛЯ БОЛОТА
        return zoneType == PerlinNoiseMap.ZoneType.Swamp;
    }
}