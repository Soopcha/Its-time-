using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EndingScreen : MonoBehaviour
{
  
    public static EndingScreen Instance;

    [Header("UI Панели")]
    public GameObject EndingPanel; // Сама панель
    public Image EndingImage;      // Картинка (CG)
    public TMPro.TMP_Text SpeakerText;
    public TMPro.TMP_Text DialogueText;
    public TMPro.TMP_Text HintText; // Текст "Нажмите для продолжения"

    [Header("CG концовок")]
    public Sprite BadEnding1CG;  // Петр + беременность
    public Sprite BadEnding2CG;
    public Sprite GoodEndingCG;
    public Sprite TrueEndingCG;

    private int currentLine = 0;
    private EndingDialogue currentDialogue;
    private bool isEndingActive = false;
    private bool justStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Не ставим DontDestroyOnLoad здесь, так как мы возвращаемся в меню
        }
        else
        {
            Destroy(gameObject);
        }

        if (EndingPanel != null)
            EndingPanel.SetActive(false);
    }

    void Update()
    {
        if (!isEndingActive) return;

        // Чтобы не пролистать первый кадр случайно
        if (justStarted)
        {
            justStarted = false;
            return;
        }

        // Проверка ввода точно как в твоем DialogueManager
        bool advanceInput = false;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) advanceInput = true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) advanceInput = true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) advanceInput = true;

        if (advanceInput)
        {
            AdvanceEnding();
        }
    }

    public void ShowBadEnding1()
    {
        var dialogue = new EndingDialogue
        {
            Speakers = new string[] { "Петр", "---", "Петр", "---", "Петр", "---", "", "" },
            Lines = new string[]
            {
                    "Доброе утро, дорогая. Как спалось?",
                    "...",
                    "Хочешь пить? Принести воды?",
                    "*молчание*",
                    "Ну что молчишь? Я же о тебе забочусь.",
                    "*взгляд в окно, на цепь у ноги*",
                    "Через месяц родится ребёнок.",
                    "Отсюда уже не выбраться."
            }
        };
        StartEndingSequence(BadEnding1CG, dialogue);
    }

    // Тут потом добавишь ShowBadEnding2, ShowGoodEnding и т.д. по образцу

    void StartEndingSequence(Sprite cg, EndingDialogue dialogue)
    {
        if (EndingPanel == null) return;

        EndingPanel.SetActive(true);
        isEndingActive = true;
        justStarted = true;

        if (EndingImage != null && cg != null)
            EndingImage.sprite = cg;

        currentDialogue = dialogue;
        currentLine = 0;

        if (HintText != null) HintText.text = "Нажмите, чтобы продолжить";

        ShowCurrentLine();

        Time.timeScale = 0f; // Останавливаем игру
    }

    void ShowCurrentLine()
    {
        if (currentDialogue == null) return;

        if (currentLine < currentDialogue.Lines.Length)
        {
            if (SpeakerText != null) SpeakerText.text = currentDialogue.Speakers[currentLine];
            if (DialogueText != null) DialogueText.text = currentDialogue.Lines[currentLine];
        }
        else
        {
            ReturnToMenu();
        }
    }

    void AdvanceEnding()
    {
        currentLine++;
        ShowCurrentLine();
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        isEndingActive = false;
        SceneManager.LoadScene("MainMenu");
    }

    [System.Serializable]
    public class EndingDialogue
    {
        public string[] Speakers;
        public string[] Lines;
    }
}
