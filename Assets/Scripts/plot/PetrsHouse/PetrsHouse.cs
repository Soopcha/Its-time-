using UnityEngine;

public class PetrsHouse : MonoBehaviour
{
    private bool playerInRange;

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive)
        {
            InteractButtonManager.Instance?.HideButton();
            return;
        }

        if (playerInRange)
        {
            InteractButtonManager.Instance?.ShowButton("Войти в дом", TryEnterHouse);
        }
    }

    void TryEnterHouse()
    {
        // Проверяем есть ли ключ
        if (!GameState.Instance.HasKey)
        {
            ShowLockedMessage();
            return;
        }

        // Входим в дом
        EnterHouse();
    }

    void ShowLockedMessage()
    {
        var data = new DialogueData();
        var node = data.AddNode();
        node.Lines.Add(new DialogueLine("", "Дверь заперта."));
        node.NextNodeIndex = -1;

        DialogueManager.Instance.StartDialogue(data);
    }

    void EnterHouse()
    {
        InteractButtonManager.Instance?.HideButton();
        PetrsHouseInterior.Instance?.ActivateInterior(transform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractButtonManager.Instance?.HideButton();
        }
    }
}