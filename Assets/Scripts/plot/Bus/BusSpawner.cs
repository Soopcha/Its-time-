using UnityEngine;

public class BusSpawner : MonoBehaviour
{
    [Header("Префаб автобуса")]
    public GameObject BusPrefab;

    [Header("Настройки спавна")]
    public WFCGenerator2DAuto Generator;
    public int MaxAttempts = 50;

    [Header("Безопасная зона")]
    public int SafeZoneTiles = 5;

    [Header("Зона очистки от деревьев")]
    [Tooltip("Радиус зоны вокруг автобуса где не будут спавниться деревья и камни (в тайлах)")]
    public Vector2 ClearZoneSize = new Vector2(2, 4);  // 2x4 тайла (автобус длинный)

    [Header("Не спавнить если загрузка")]
    public bool SpawnOnlyOnNewGame = true;

    void Start()
    {
        bool shouldGenerate = PlayerPrefs.GetInt("ShouldGenerate", 1) == 1;

        if (SpawnOnlyOnNewGame && !shouldGenerate)
        {
            Debug.Log("Автобус НЕ спавнится - идёт загрузка из сохранения");
            return;
        }

        Invoke(nameof(SpawnBus), 0.6f);
    }

    void SpawnBus()
    {
        if (Generator == null || BusPrefab == null)
        {
            Debug.LogError("Generator или BusPrefab не назначен!");
            return;
        }

        Vector2Int mapSize = Generator.MapSize;
        float tileSize = Generator.TileSize;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            int x = Random.Range(SafeZoneTiles, mapSize.x - SafeZoneTiles);
            int y = Random.Range(SafeZoneTiles, mapSize.y - SafeZoneTiles);

            if (IsFieldTile(x, y))
            {
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                GameObject bus = Instantiate(BusPrefab, position, Quaternion.identity);

                SaveableObject saveable = bus.GetComponent<SaveableObject>();
                if (saveable == null)
                {
                    saveable = bus.AddComponent<SaveableObject>();
                }
                saveable.SavePosition = true;
                saveable.SaveActiveState = true;

                // ===== НОВОЕ: Добавляем запрещённую зону =====
                Vector2 clearSize = ClearZoneSize * tileSize;
                Generator.AddForbiddenZone(position, clearSize);

                Debug.Log($"Автобус заспавнен в ({x}, {y}) с зоной очистки {clearSize}");
                return;
            }
        }

        Debug.LogWarning("Не удалось найти подходящее место для автобуса!");
    }

    bool IsFieldTile(int x, int y)
    {
        if (Generator.PerlinNoise == null) return false;

        var zoneType = Generator.PerlinNoise.GetZoneType(x, y,
            Generator.MapSize.x, Generator.MapSize.y);

        return zoneType == PerlinNoiseMap.ZoneType.Field;
    }
}