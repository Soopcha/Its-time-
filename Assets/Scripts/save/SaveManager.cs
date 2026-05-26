using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string saveFolder;
    private SaveData pendingLoadData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        saveFolder = Application.persistentDataPath + "/Saves/";

        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game" && pendingLoadData != null)
        {
            Debug.Log("Применяем сохранённые данные после загрузки сцены...");
            StartCoroutine(ApplySaveDataDelayed(pendingLoadData));
            pendingLoadData = null;
        }
    }

    string GetSlotPath(int slotIndex)
    {
        return saveFolder + $"save_slot_{slotIndex}.json";
    }

    public void SaveGame(int slotIndex)
    {
        SaveData data = CollectSaveData();
        data.SaveName = $"Слот {slotIndex}";
        data.SaveDate = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSlotPath(slotIndex), json);

        Debug.Log($"Игра сохранена в слот {slotIndex}");
        Debug.Log($"Путь: {GetSlotPath(slotIndex)}");
        Debug.Log($"Дата: {data.SaveDate}");
    }

    public void LoadGame(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Слот {slotIndex} пуст!");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        pendingLoadData = data;

        Debug.Log($"Данные слота {slotIndex} готовы к загрузке");
    }

    public bool SlotExists(int slotIndex)
    {
        return File.Exists(GetSlotPath(slotIndex));
    }

    public void DeleteSlot(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Слот {slotIndex} удалён");
        }
    }

    public SaveData GetSlotInfo(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    SaveData CollectSaveData()
    {
        SaveData data = new SaveData();

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            data.PlayerPosition = player.transform.position;
            data.PlayerHealth = player.CurrentHealth;
        }

        if (GameState.Instance != null)
        {
            data.HasReadNote = GameState.Instance.HasReadNote;
            data.HasKey = GameState.Instance.HasKey;
            data.PetrCutsceneDone = GameState.Instance.PetrCutsceneDone;
            data.PlayTime = GameState.Instance.PlayTime;
        }

        WFCGenerator2DAuto generator = FindObjectOfType<WFCGenerator2DAuto>();
        if (generator != null)
        {
            data.MapSeed = Random.Range(0, int.MaxValue);
            data.MapSize = generator.MapSize;

            foreach (Transform child in generator.transform)
            {
                Tile2DAuto tile = child.GetComponent<Tile2DAuto>();
                if (tile != null)
                {
                    data.Tiles.Add(new TileData(tile.name.Replace("(Clone)", ""), child.position));
                }
            }
        }

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (var enemy in enemies)
        {
            List<Vector2> points = new List<Vector2>();
            foreach (var pt in enemy.PatrolPoints)
            {
                if (pt != null)
                    points.Add(pt.position);
            }

            data.Enemies.Add(new EnemyData(
                enemy.transform.position,
                enemy.CurrentState.ToString(),
                0,
                points
            ));
        }

        SaveableObject[] saveableObjects = FindObjectsOfType<SaveableObject>(true);
        Debug.Log($"Найдено SaveableObject на сцене: {saveableObjects.Length}");

        foreach (var obj in saveableObjects)
        {
            SaveableObjectData objData = obj.GetSaveData();
            data.SaveableObjects.Add(objData);

            // НОВОЕ: Логируем каждый объект
            Debug.Log($"Сохраняем: {obj.name}, ID={objData.ObjectID}, Active={objData.IsActive}, Pos={objData.Position}");
        }

        Debug.Log($"Сохранено объектов: {data.SaveableObjects.Count}");

        SaveableNPC[] npcs = FindObjectsOfType<SaveableNPC>();
        foreach (var npc in npcs)
        {
            data.NPCs.Add(npc.GetSaveData());
        }

        return data;
    }

    IEnumerator ApplySaveDataDelayed(SaveData data)
    {
        yield return null;
        yield return null;

        ApplySaveData(data);

        Debug.Log("Игра загружена!");
    }

    void ApplySaveData(SaveData data)
    {
        Debug.Log("Применяем сохранённые данные...");

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.transform.position = data.PlayerPosition;
            player.CurrentHealth = data.PlayerHealth;
            Debug.Log($"Игрок: позиция {data.PlayerPosition}, здоровье {data.PlayerHealth}");
        }
        else
        {
            Debug.LogWarning("PlayerController не найден!");
        }

        if (GameState.Instance != null)
        {
            GameState.Instance.HasReadNote = data.HasReadNote;
            GameState.Instance.HasKey = data.HasKey;
            GameState.Instance.PetrCutsceneDone = data.PetrCutsceneDone;
            GameState.Instance.PlayTime = data.PlayTime;
            Debug.Log($"Сюжет: записка={data.HasReadNote}, ключ={data.HasKey}");
        }
        else
        {
            Debug.LogWarning("GameState не найден!");
        }

        WFCGenerator2DAuto generator = FindObjectOfType<WFCGenerator2DAuto>();
        if (generator != null)
        {
            generator.ShouldGenerate = false;
            generator.ClearMap();

            Debug.Log($"Восстанавливаем {data.Tiles.Count} тайлов...");

            foreach (var tileData in data.Tiles)
            {
                Tile2DAuto prefab = generator.TilePrefabs.Find(t => t.name == tileData.TilePrefabName);
                if (prefab != null)
                {
                    Instantiate(prefab.gameObject, tileData.Position, Quaternion.identity, generator.transform);
                }
            }

            Debug.Log("Карта восстановлена!");
        }
        else
        {
            Debug.LogWarning("WFCGenerator не найден!");
        }

        StartCoroutine(RestoreObjectsAfterDelay(data));
    }

    IEnumerator RestoreObjectsAfterDelay(SaveData data)
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"Применяем состояние {data.SaveableObjects.Count} объектов...");

        SaveableObject[] currentObjects = FindObjectsOfType<SaveableObject>(true);

        foreach (var objData in data.SaveableObjects)
        {
            SaveableObject obj = System.Array.Find(currentObjects, o => o.ObjectID == objData.ObjectID);

            if (obj != null)
            {
                obj.LoadData(objData);
                Debug.Log($"Объект {obj.name}: активен={objData.IsActive}");
            }
            else
            {
                Debug.LogWarning($"Объект с ID {objData.ObjectID} не найден - возможно уже удалён");
            }
        }

        Debug.Log($"Применяем состояние {data.NPCs.Count} NPC...");

        SaveableNPC[] currentNPCs = FindObjectsOfType<SaveableNPC>(true);
        foreach (var npcData in data.NPCs)
        {
            SaveableNPC npc = System.Array.Find(currentNPCs, n => n.GetComponent<SaveableObject>().ObjectID == npcData.ObjectID);

            if (npc != null)
            {
                npc.LoadData(npcData);
                Debug.Log($"NPC {npc.name} загружен");
            }
            else
            {
                Debug.LogWarning($"NPC с ID {npcData.ObjectID} не найден");
            }
        }

        Debug.Log("Все данные применены!");
    }
}