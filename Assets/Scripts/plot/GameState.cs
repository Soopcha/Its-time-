using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    // Флаги прогресса
    public bool HasReadNote = false;
    public bool HasKey = false;
    public bool PetrCutsceneDone = false;

    // Время игры
    public float PlayTime = 0f;

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

    void Update()
    {
        PlayTime += Time.deltaTime;
    }
}