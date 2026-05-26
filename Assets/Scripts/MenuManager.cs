using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Панели")]
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;
    public GameObject AboutPanel;
    public GameObject SaveSlotsPanel;

    [Header("Кнопки слотов")]
    public Button Slot1Button;
    public Button Slot2Button;
    public Button Slot3Button;
    public Button BackFromSlotsButton;

    [Header("Кнопки главного меню")]
    public Button NewGameButton;
    public Button ContinueButton;
    public Button SettingsButton;
    public Button AboutButton;
    public Button ExitButton;

    [Header("Кнопки возврата")]
    public Button BackFromSettingsButton;
    public Button BackFromAboutButton;

    [Header("Настройки (текст в About)")]
    public TMPro.TMP_Text AboutText;

    void Start()
    {
        // Привязываем кнопки
        NewGameButton.onClick.AddListener(OnNewGame);
        ContinueButton.onClick.AddListener(OnContinue);
        SettingsButton.onClick.AddListener(OnSettings);
        AboutButton.onClick.AddListener(OnAbout);
        ExitButton.onClick.AddListener(OnExit);

        BackFromSettingsButton.onClick.AddListener(OnBackToMenu);
        BackFromAboutButton.onClick.AddListener(OnBackToMenu);

        // Показываем главное меню
        ShowMainMenu();

        // Проверяем есть ли сохранение (пока отключено)
        ContinueButton.interactable = false;

        // Заполняем текст "Об игре"
        if (AboutText != null)
        {
            AboutText.text = "Разработчик: Малыхина Елена\n\n" +
                            "Игра с процедурной генерацией уровней\n" +
                            "Версия: 0.1 Alpha\n\n" +
                            "Unity 2D | WFC + Perlin Noise";
        }
        // Запускаем музыку меню
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }

        // Привязываем кнопки слотов
        Slot1Button.onClick.AddListener(() => LoadSlot(1));
        Slot2Button.onClick.AddListener(() => LoadSlot(2));
        Slot3Button.onClick.AddListener(() => LoadSlot(3));
        BackFromSlotsButton.onClick.AddListener(OnBackToMenu);

        UpdateSlotButtons();
        UpdateContinueButton();
    }

    void UpdateContinueButton()
    {
        if (SaveManager.Instance == null)
        {
            ContinueButton.interactable = false;
            return;
        }

        // Проверяем есть ли хотя бы одно сохранение
        bool hasSaves = SaveManager.Instance.SlotExists(1) ||
                        SaveManager.Instance.SlotExists(2) ||
                        SaveManager.Instance.SlotExists(3);

        ContinueButton.interactable = hasSaves;

        Debug.Log($"Кнопка 'Продолжить': {(hasSaves ? "активна" : "неактивна")}");
    }
    void ShowMainMenu()
    {
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        AboutPanel.SetActive(false);
        SaveSlotsPanel.SetActive(false);
    }

    void OnNewGame()
    {
        //  Сбрасываем флаг (чтобы сгенерировать новую карту)
        PlayerPrefs.SetInt("ShouldGenerate", 1);
        PlayerPrefs.DeleteKey("LoadSlotIndex"); // Удаляем слот загрузки
        PlayerPrefs.Save();

        Debug.Log(" Новая игра: карта будет сгенерирована");

        // Загружаем игровую сцену
        SceneManager.LoadScene("Game");
    }

    void OnSettings()
    {
        MainMenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    void OnAbout()
    {
        MainMenuPanel.SetActive(false);
        AboutPanel.SetActive(true);
    }

    void OnBackToMenu()
    {
        ShowMainMenu();
    }

    void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void OnContinue()
    {
        MainMenuPanel.SetActive(false);
        SaveSlotsPanel.SetActive(true);
        UpdateSlotButtons();
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

    void LoadSlot(int slotIndex)
    {
        //  Устанавливаем флаг "НЕ генерировать"
        PlayerPrefs.SetInt("ShouldGenerate", 0);
        PlayerPrefs.SetInt("LoadSlotIndex", slotIndex);
        PlayerPrefs.Save();

        Debug.Log($" Загрузка слота {slotIndex}: генерация отключена");

        //  Загружаем данные
        SaveManager.Instance.LoadGame(slotIndex);

        //  Загружаем сцену
        SceneManager.LoadScene("Game");
    }
}