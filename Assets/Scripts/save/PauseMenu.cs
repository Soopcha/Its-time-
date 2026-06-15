using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Панели")]
    public GameObject PausePanel;
    public GameObject SaveSlotsPanel;
    public GameObject SettingsPanel;

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

    [Header("Настройки")]
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public Button BackFromSettingsButton;

    private bool isPaused = false;

    void Start()
    {
        PausePanel.SetActive(false);
        SaveSlotsPanel.SetActive(false);
        SettingsPanel.SetActive(false);

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

        // Настройки
        BackFromSettingsButton.onClick.AddListener(HideSettings);

        // Загружаем сохранённые настройки
        LoadSettings();

        // Привязываем события
        MusicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        SFXVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
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
        SaveSlotsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    void Resume()
    {
        isPaused = false;
        PausePanel.SetActive(false);
        SaveSlotsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
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
        UpdateSlotButtons();
    }

    void OpenSettings()
    {
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    void HideSettings()
    {
        SettingsPanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        MusicVolumeSlider.value = musicVolume;
        SFXVolumeSlider.value = sfxVolume;

        // Применяем настройки
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVolume);
            AudioManager.Instance.SetSFXVolume(sfxVolume);
        }
    }

    void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}