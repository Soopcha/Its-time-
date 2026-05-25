using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [Header("UI")]
    public GameObject LoadingPanel;
    public Image LoadingSprite;
    public TMPro.TMP_Text LoadingText;

    [Header("Настройки")]
    public float RotationSpeed = 100f;

    private bool isLoading = false;

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
        // Вращаем спрайт загрузки
        if (isLoading && LoadingSprite != null)
        {
            LoadingSprite.transform.Rotate(0, 0, -RotationSpeed * Time.deltaTime);
        }
    }

    public void Show(string text = "Генерация мира...")
    {
        isLoading = true;
        LoadingPanel.SetActive(true);

        if (LoadingText != null)
            LoadingText.text = text;

        // Блокируем управление
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
            player.enabled = false;

        MobileInputManager.Instance?.HideControls();
    }

    public void Hide()
    {
        isLoading = false;
        LoadingPanel.SetActive(false);

        // Включаем управление
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
            player.enabled = true;

        MobileInputManager.Instance?.ShowControls();
    }
}