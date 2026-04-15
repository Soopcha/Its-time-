using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractButtonManager : MonoBehaviour
{
    public static InteractButtonManager Instance;

    [Header("UI")]
    public Button InteractButton;
    public TMP_Text ButtonText;

    private System.Action currentAction;

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

        if (InteractButton != null)
        {
            InteractButton.onClick.AddListener(OnButtonClick);
            HideButton();
        }
    }

    public void ShowButton(string text, System.Action action)
    {
        if (InteractButton == null) return;

        ButtonText.text = text;
        currentAction = action;
        InteractButton.gameObject.SetActive(true);
    }

    public void HideButton()
    {
        if (InteractButton == null) return;

        InteractButton.gameObject.SetActive(false);
        currentAction = null;
    }

    void OnButtonClick()
    {
        currentAction?.Invoke();
    }
}