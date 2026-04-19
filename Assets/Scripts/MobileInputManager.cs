using UnityEngine;

public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance;

    [Header("Кнопки направлений")]
    public MobileInputButton UpButton;
    public MobileInputButton DownButton;
    public MobileInputButton LeftButton;
    public MobileInputButton RightButton;

    [Header("Видимость")]
    public GameObject MobileControlsRoot;

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

    void Start()
    {
        // Показываем кнопки только на мобилке
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        ShowControls();
#else
            HideControls();
#endif
    }

    public Vector2 GetMoveInput()
    {
        Vector2 input = Vector2.zero;

        if (UpButton != null && UpButton.IsPressed)
            input.y = 1;
        if (DownButton != null && DownButton.IsPressed)
            input.y = -1;
        if (LeftButton != null && LeftButton.IsPressed)
            input.x = -1;
        if (RightButton != null && RightButton.IsPressed)
            input.x = 1;

        if (input.magnitude > 1f)
            input.Normalize();

        return input;
    }

    // НОВЫЙ МЕТОД: Проверка бега через двойное нажатие
    public bool IsRunning()
    {
        // Бег активен если ЛЮБАЯ кнопка в режиме двойного нажатия
        if (UpButton != null && UpButton.IsDoubleTapRunning)
            return true;
        if (DownButton != null && DownButton.IsDoubleTapRunning)
            return true;
        if (LeftButton != null && LeftButton.IsDoubleTapRunning)
            return true;
        if (RightButton != null && RightButton.IsDoubleTapRunning)
            return true;

        return false;
    }

    public void ShowControls()
    {
        if (MobileControlsRoot != null)
            MobileControlsRoot.SetActive(true);
    }

    public void HideControls()
    {
        if (MobileControlsRoot != null)
            MobileControlsRoot.SetActive(false);
    }
}