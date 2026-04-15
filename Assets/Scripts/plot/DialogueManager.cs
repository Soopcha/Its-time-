using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject DialoguePanel;
    public TMP_Text SpeakerText;
    public TMP_Text DialogueText;
    public TMP_Text HintText;

    [Header("¬˚·Ó")]
    public GameObject ChoicePanel;
    public Button Choice1Button;
    public Button Choice2Button;
    public TMP_Text Choice1Text;
    public TMP_Text Choice2Text;

    private DialogueData currentData;
    private int currentNodeIndex;
    private int currentLineIndex;
    private bool isActive;
    private bool waitingForChoice;
    private bool justStarted;

    private Action onEnd;
    private Action<string> onAction;

    public bool IsActive => isActive;

    void Awake()
    {
        Instance = this;
        DialoguePanel.SetActive(false);
        ChoicePanel.SetActive(false);
    }

    //void Update()
    //{
    //    if (!isActive) return;

    //    if (justStarted)
    //    {
    //        justStarted = false;
    //        return;
    //    }

    //    if (waitingForChoice) return;

    //    if (Keyboard.current.eKey.wasPressedThisFrame)
    //    {
    //        Advance();
    //    }
    //}
    void Update()
    {
        if (!isActive) return;

        if (justStarted)
        {
            justStarted = false;
            return;
        }

        if (waitingForChoice) return;

        //  À¿¬»¿“”–¿ 
        bool advanceInput = false;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            advanceInput = true;
        }

        //  ¿—¿Õ»ﬂ 
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            advanceInput = true;
        }

        // Ã€ÿ‹ 
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            advanceInput = true;
        }

        if (advanceInput)
        {
            Advance();
        }
    }

    public void StartDialogue(DialogueData data, Action onEnd = null,
                               Action<string> onAction = null)
    {

        InteractButtonManager.Instance?.HideButton();
        currentData = data;
        currentNodeIndex = 0;
        currentLineIndex = 0;
        isActive = true;
        waitingForChoice = false;
        justStarted = true;
        this.onEnd = onEnd;
        this.onAction = onAction;

        DialoguePanel.SetActive(true);
        ChoicePanel.SetActive(false);

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (currentNodeIndex >= currentData.Nodes.Count)
        {
            EndDialogue();
            return;
        }

        var node = currentData.Nodes[currentNodeIndex];

        if (currentLineIndex >= node.Lines.Count)
        {
            if (node.HasChoices)
            {
                ShowChoices(node.Choices);
                return;
            }

            if (node.NextNodeIndex >= 0)
            {
                currentNodeIndex = node.NextNodeIndex;
                currentLineIndex = 0;
                ShowCurrentLine();
                return;
            }

            EndDialogue();
            return;
        }

        var line = node.Lines[currentLineIndex];
        SpeakerText.text = line.Speaker;
        DialogueText.text = line.Text;

        if (HintText != null)
            HintText.text = "[E]";
    }

    void Advance()
    {
        currentLineIndex++;
        ShowCurrentLine();
    }

    void ShowChoices(List<DialogueChoice> choices)
    {
        waitingForChoice = true;
        ChoicePanel.SetActive(true);

        if (HintText != null)
            HintText.text = "";

        if (choices.Count > 0)
        {
            Choice1Button.gameObject.SetActive(true);
            Choice1Text.text = choices[0].Text;
            Choice1Button.onClick.RemoveAllListeners();
            var c0 = choices[0];
            Choice1Button.onClick.AddListener(() => SelectChoice(c0));
        }

        if (choices.Count > 1)
        {
            Choice2Button.gameObject.SetActive(true);
            Choice2Text.text = choices[1].Text;
            Choice2Button.onClick.RemoveAllListeners();
            var c1 = choices[1];
            Choice2Button.onClick.AddListener(() => SelectChoice(c1));
        }
    }

    void SelectChoice(DialogueChoice choice)
    {
        waitingForChoice = false;
        ChoicePanel.SetActive(false);

        if (!string.IsNullOrEmpty(choice.Action))
        {
            onAction?.Invoke(choice.Action);
            EndDialogue();
            return;
        }

        if (choice.NextNodeIndex >= 0)
        {
            currentNodeIndex = choice.NextNodeIndex;
            currentLineIndex = 0;
            ShowCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isActive = false;
        DialoguePanel.SetActive(false);
        ChoicePanel.SetActive(false);
        onEnd?.Invoke();
    }
}