using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    // Флаги прогресса
    public bool HasReadNote = false;
    public bool HasKey = false;
    public bool PetrCutsceneDone = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}