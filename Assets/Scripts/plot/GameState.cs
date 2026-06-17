using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    // Флаги прогресса
    public bool HasReadNote = false;
    public bool HasKey = false;
    public bool PetrCutsceneDone = false;
    public bool NikaCutsceneDone = false;
    public bool VladCutsceneDone = false;

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

    // ===== НОВЫЕ МЕТОДЫ =====

    /// <summary>
    /// Можно ли войти к Нике? (Требует встречи с Петром)
    /// </summary>
    public bool CanEnterBus()
    {
        return PetrCutsceneDone;
    }

    /// <summary>
    /// Можно ли войти к Владу? (Требует встречи с Петром И Никой)
    /// </summary>
    public bool CanEnterVladHouse()
    {
        return PetrCutsceneDone && NikaCutsceneDone;
    }
}