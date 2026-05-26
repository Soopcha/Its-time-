using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Метаинформация
    public string SaveName;
    public string SaveDate;
    public float PlayTime;

    // Игрок
    public Vector2 PlayerPosition;
    public int PlayerHealth;

    // Сюжет (флаги из GameState)
    public bool HasReadNote;
    public bool HasKey;
    public bool PetrCutsceneDone;

    // Карта
    public int MapSeed;  // Seed для воспроизведения Perlin Noise
    public Vector2Int MapSize;
    public List<TileData> Tiles = new List<TileData>();

    // Враги
    public List<EnemyData> Enemies = new List<EnemyData>();

    // Интерактивные объекты
    public List<SaveableObjectData> SaveableObjects = new List<SaveableObjectData>();


    public List<SaveableNPCData> NPCs = new List<SaveableNPCData>();

    // Конструктор по умолчанию
    public SaveData()
    {
        SaveName = "Новое сохранение";
        SaveDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        PlayTime = 0f;
    }
}

// Данные одного тайла
[System.Serializable]
public class TileData
{
    public string TilePrefabName;
    public Vector2 Position;

    public TileData(string name, Vector2 pos)
    {
        TilePrefabName = name;
        Position = pos;
    }
}

// Данные одного врага
[System.Serializable]
public class EnemyData
{
    public Vector2 Position;
    public string State;  // Patrol, Alert, Chase
    public int PatrolIndex;
    public List<Vector2> PatrolPoints = new List<Vector2>();

    public EnemyData(Vector2 pos, string state, int patrolIndex, List<Vector2> points)
    {
        Position = pos;
        State = state;
        PatrolIndex = patrolIndex;
        PatrolPoints = points;
    }
}