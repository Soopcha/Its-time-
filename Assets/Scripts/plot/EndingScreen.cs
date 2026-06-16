using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EndingScreen : MonoBehaviour
{
    public static EndingScreen Instance;

    [Header("UI Панели")]
    public GameObject EndingPanel;
    public Image EndingImage;
    public TMPro.TMP_Text SpeakerText;
    public TMPro.TMP_Text DialogueText;
    public TMPro.TMP_Text HintText;

    [Header("CG концовок")]
    public Sprite BadEnding1CG;
    public Sprite GoodEnding1CG;
    public Sprite GoodEnding2CG;
    public Sprite TrueEndingCG;  // ? Используется для истинной концовки

    private enum EndingPhase
    {
        PreText,
        Image,
        PostText
    }

    private EndingPhase currentPhase;
    private int currentLine = 0;
    private bool isEndingActive = false;
    private bool justStarted = false;

    private string[] preTextSpeakers;
    private string[] preTextLines;
    private Sprite currentCG;
    private string[] imageSpeakers;
    private string[] imageLines;
    private string[] postTextSpeakers;
    private string[] postTextLines;

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

        if (EndingPanel != null)
            EndingPanel.SetActive(false);
    }

    void Update()
    {
        if (!isEndingActive) return;

        if (justStarted)
        {
            justStarted = false;
            return;
        }

        bool advanceInput = false;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) advanceInput = true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) advanceInput = true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) advanceInput = true;

        if (advanceInput)
        {
            AdvanceEnding();
        }
    }

    // ===== ПЛОХАЯ КОНЦОВКА 1 (с Петром) =====
    public void ShowBadEnding1()
    {
        preTextSpeakers = new string[] { "", "" };
        preTextLines = new string[]
        {
            "....",
            "...."
        };

        currentCG = BadEnding1CG;
        imageSpeakers = new string[] { "Петр", "---", "Петр", "---", "Петр", "---", "", "" };
        imageLines = new string[]
        {
            "Доброе утро, дорогая. Как спалось?",
            "...",
            "Хочешь пить? Принести воды?",
            "*молчание*",
            "Ну что молчишь? Я же о тебе забочусь.",
            "Давай, спасибо...",
            "Убил сегодня одного из этих тварей.",
            "Ещё полон сил я, ахахаха."
        };

        postTextSpeakers = new string[] { "" };
        postTextLines = new string[]
        {
            "ПЛОХАЯ КОНЦОВКА"
        };

        StartEnding();
    }

    // ===== ХОРОШАЯ КОНЦОВКА 1 (с Никой) =====
    public void ShowGoodEnding1()
    {
        preTextSpeakers = new string[] { "", "" };
        preTextLines = new string[]
        {
            "*Несколько недель спустя*",
            "..."
        };

        currentCG = GoodEnding1CG;
        imageSpeakers = new string[] { "---", "---", "---", "Ника", "---", "Рома", "Ника", "---", "Ника", "Ника", "---", "---", "---" };
        imageLines = new string[]
        {
            "С ними было весело.",
            "Я не заметила как осталась у них на неделю, а потом еще немного и еще...",
            "Может мои воспоминания не важны, мне с ними так хорошо...",
            "Соня, смотри! Я сделала чучело из головы того зомбака, что тут шлялся!",
            "Ахаха, Ника, это отвратительно, выбрось это!",
            "*улыбается*",
            "Да подожди ты! Давай у входа повесим? Вдруг других отпугнет!",
            "Боже...",
            "*Громко смеётся*",
            "Вручаю тебе честь его прикрепить",
            "*Сунула тебе в руки немного сгнившую голову*",
            "*Тебя немного корёжит, но глядя на весёлое лицо Ники, ты спокойно выдыхаешь*",
            "..."
        };

        postTextSpeakers = new string[] { "" };
        postTextLines = new string[]
        {
            "ХОРОШАЯ КОНЦОВКА: Новая семья"
        };

        StartEnding();
    }

    // ===== ХОРОШАЯ КОНЦОВКА 2 (с Владом - не узнала правду) =====
    public void ShowGoodEnding2()
    {
        preTextSpeakers = new string[] { "", "" };
        preTextLines = new string[]
        {
            "*Ты сидишь и пьёшь чай*",
            "*Через некоторое время Влад возвращается*"
        };

        currentCG = GoodEnding2CG;
        imageSpeakers = new string[] { "Влад", "---", "---", "Влад", "---", "Влад", "Влад", "Влад", "---", "---", "Влад", "", "" };
        imageLines = new string[]
        {
            "Я принёс сегодня вкусных яблок.",
            "Спасибо...",
            "На самом деле я рада, что набрела на тебя.",
            "И я рад, что ты жива...",
            "Давай! Расскажи ещё обо мне! Я всё ещё ничего не помню...",
            "Ахаха, ладно, ладно.",
            "Помню, ты рассказывала, что любишь смотреть на звёзды...",
            "И однажды ты сшила мне заплатку на куртке. Очень аккуратно.",
            "*улыбается*",
            "Звучит... мило. Может быть, я вспомню когда-нибудь.",
            "Не спеши. Главное, что ты здесь. И в безопасности.",
            "*Вы продолжаете говорить до глубокой ночи*",
            "*Может, прошлое и важно... но сейчас у тебя есть настоящее*"
        };

        postTextSpeakers = new string[] { "" };
        postTextLines = new string[]
        {
            "ХОРОШАЯ КОНЦОВКА: Новый рассвет"
        };

        StartEnding();
    }

    // ===== ИСТИННАЯ КОНЦОВКА (с Владом - узнала правду о ребёнке) =====
    public void ShowTrueEnding()
    {
        preTextSpeakers = new string[] { "", "" };
        preTextLines = new string[]
        {
            "*Ты тихо выходишь за ним*",
            "*Влад стоит рядом с мояком и выкапывает крест из чьей-то могилы*"
        };

        currentCG = TrueEndingCG;
        imageSpeakers = new string[] { "Влад", "---", "", "", "---", "---", "---", "Влад", "", "Влад", "", "---" };
        imageLines = new string[]
        {
            "*резко оборачивается*",
            "Это... просто декор. Вернись в дом.",
            "*смотришь внимательно на крест*",
            "*Голову пронзает острая боль*",
            "...",
            "*В голову приходят воспоминания о... Максиме...*",
            "ГДЕ МОЙ РЕБЁНОК!",
            "ГДЕ МОЙ РЕБЁНОК!",
            "Успокойся, успокойся...",
            "*Он со всей силы обнимает тебя*",
            "Прости. Прости. Прости. Я хотел убрать это до того, как ты вспомнишь...",
            "*Воспоминания нахлынули волной*"
        };

        postTextSpeakers = new string[] { "", "", "", "" };
        postTextLines = new string[]
        {
            "*Муж погиб в самом начале эпидемии*",
            "*Сын Максим... пропал на одной из вылазок за едой*",
            "*Ты всё вспомнила. Но знание этого не принесло облегчения*",
            "ИСТИННАЯ КОНЦОВКА: Груз памяти"
        };

        StartEnding();
    }

    void StartEnding()
    {
        EndingPanel.SetActive(true);
        isEndingActive = true;
        justStarted = true;
        Time.timeScale = 0f;

        currentPhase = EndingPhase.PreText;
        currentLine = 0;

        if (EndingImage != null)
            EndingImage.gameObject.SetActive(false);

        if (HintText != null)
            HintText.text = "Нажмите, чтобы продолжить";

        ShowCurrentLine();
    }

    void AdvanceEnding()
    {
        currentLine++;

        string[] lines = GetCurrentLines();

        if (currentLine < lines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            GoToNextPhase();
        }
    }

    void GoToNextPhase()
    {
        if (currentPhase == EndingPhase.PreText)
        {
            currentPhase = EndingPhase.Image;
            currentLine = 0;

            if (EndingImage != null)
            {
                EndingImage.gameObject.SetActive(true);
                if (currentCG != null)
                    EndingImage.sprite = currentCG;
            }

            justStarted = true;
            ShowCurrentLine();
        }
        else if (currentPhase == EndingPhase.Image)
        {
            currentPhase = EndingPhase.PostText;
            currentLine = 0;

            if (EndingImage != null)
                EndingImage.gameObject.SetActive(false);

            justStarted = true;
            ShowCurrentLine();
        }
        else if (currentPhase == EndingPhase.PostText)
        {
            ReturnToMenu();
        }
    }

    void ShowCurrentLine()
    {
        string[] speakers = GetCurrentSpeakers();
        string[] lines = GetCurrentLines();

        if (speakers == null || lines == null) return;
        if (currentLine >= lines.Length) return;

        if (SpeakerText != null)
            SpeakerText.text = speakers[currentLine];

        if (DialogueText != null)
            DialogueText.text = lines[currentLine];
    }

    string[] GetCurrentSpeakers()
    {
        switch (currentPhase)
        {
            case EndingPhase.PreText: return preTextSpeakers;
            case EndingPhase.Image: return imageSpeakers;
            case EndingPhase.PostText: return postTextSpeakers;
            default: return null;
        }
    }

    string[] GetCurrentLines()
    {
        switch (currentPhase)
        {
            case EndingPhase.PreText: return preTextLines;
            case EndingPhase.Image: return imageLines;
            case EndingPhase.PostText: return postTextLines;
            default: return null;
        }
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        isEndingActive = false;

        SceneManager.LoadScene("MainMenu");
    }
}