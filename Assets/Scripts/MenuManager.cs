using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Панели")]
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;
    public GameObject AboutPanel;

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
    }

    void ShowMainMenu()
    {
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        AboutPanel.SetActive(false);
    }

    void OnNewGame()
    {
        // Загружаем игровую сцену
        SceneManager.LoadScene("Game"); // ? Имя игровой сцены
    }

    void OnContinue()
    {
        Debug.Log("Продолжить игру (пока не реализовано)");
        // Здесь будет загрузка сохранения
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
}