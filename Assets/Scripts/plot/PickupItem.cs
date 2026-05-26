using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class PickupItem : MonoBehaviour
{
    [Header("Настройки")]
    public string ItemName = "Предмет";
    public string PickupMessage = "взяла предмет";

    [Header("Диалог (опционально)")]
    public bool ShowDialogue = false;
    public DialogueData CustomDialogue;

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
            InteractButtonManager.Instance?.ShowButton($"Взять {ItemName}", Pickup);
        }
    }

    void Pickup()
    {
        if (ShowDialogue && CustomDialogue != null)
        {
            // Показываем кастомный диалог
            DialogueManager.Instance.StartDialogue(CustomDialogue, onEnd: () =>
            {
                FinishPickup();
            });
        }
        else
        {
            // Простое взятие
            Debug.Log($" {PickupMessage}");
            FinishPickup();
        }
    }

    void FinishPickup()
    {
        InteractButtonManager.Instance?.HideButton();
        Destroy(gameObject); // SaveableObject сохранит что объект удалён
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