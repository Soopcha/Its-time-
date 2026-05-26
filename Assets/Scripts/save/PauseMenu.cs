using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject PausePanel;
    public GameObject SaveSlotsPanel; 

    [Header("Кнопки главного меню паузы")]
    public Button ResumeButton;
    public Button SaveButton;
    public Button SettingsButton;
    public Button MainMenuButton;

    [Header("Кнопки выбора слота")]
    public Button Slot1Button;
    public Button Slot2Button;
    public Button Slot3Button;
    public Button BackFromSlotsButton;

    private bool isPaused = false;

    void Start()
    {
        PausePanel.SetActive(false);
        SaveSlotsPanel.SetActive(false);

        // Главное меню паузы
        ResumeButton.onClick.AddListener(Resume);
        SaveButton.onClick.AddListener(ShowSaveSlots);
        SettingsButton.onClick.AddListener(OpenSettings);
        MainMenuButton.onClick.AddListener(ReturnToMenu);

        // Кнопки слотов
        Slot1Button.onClick.AddListener(() => SaveToSlot(1));
        Slot2Button.onClick.AddListener(() => SaveToSlot(2));
        Slot3Button.onClick.AddListener(() => SaveToSlot(3));
        BackFromSlotsButton.onClick.AddListener(HideSaveSlots);
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        PausePanel.SetActive(true);
        SaveSlotsPanel.SetActive(false);  // Скрываем слоты
        Time.timeScale = 0f;
    }

    void Resume()
    {
        isPaused = false;
        PausePanel.SetActive(false);
        SaveSlotsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void ShowSaveSlots()
    {
        PausePanel.SetActive(false);
        SaveSlotsPanel.SetActive(true);
        UpdateSlotButtons();
    }

    void HideSaveSlots()
    {
        SaveSlotsPanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    void UpdateSlotButtons()
    {
        UpdateSlotButton(Slot1Button, 1);
        UpdateSlotButton(Slot2Button, 2);
        UpdateSlotButton(Slot3Button, 3);
    }

    void UpdateSlotButton(Button button, int slotIndex)
    {
        bool exists = SaveManager.Instance.SlotExists(slotIndex);

        TMPro.TMP_Text buttonText = button.GetComponentInChildren<TMPro.TMP_Text>();
        if (buttonText != null)
        {
            if (exists)
            {
                SaveData data = SaveManager.Instance.GetSlotInfo(slotIndex);
                if (data != null)
                {
                    // Показываем дату и время игры
                    int hours = Mathf.FloorToInt(data.PlayTime / 3600f);
                    int minutes = Mathf.FloorToInt((data.PlayTime % 3600f) / 60f);

                    buttonText.text = $"Слот {slotIndex}\n" +
                                      $"{data.SaveDate}\n" +
                                      $"Игровое время: {hours}ч {minutes}м";
                }
            }
            else
            {
                buttonText.text = $"Слот {slotIndex}\n\nПусто";
            }
        }
    }

    void SaveToSlot(int slotIndex)
    {
        SaveManager.Instance.SaveGame(slotIndex);
        Debug.Log($" Сохранено в слот {slotIndex}");

        // Обновляем кнопки
        UpdateSlotButtons();

        // Показываем уведомление (опционально)
        // ShowNotification($"Игра сохранена в слот {slotIndex}");
    }

    void OpenSettings()
    {
        Debug.Log("Настройки (пока не реализовано)");
        // Здесь можно открыть панель настроек
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}