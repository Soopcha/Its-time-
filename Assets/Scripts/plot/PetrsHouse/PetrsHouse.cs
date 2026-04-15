using UnityEngine;
using UnityEngine.InputSystem;

public class PetrsHouse : MonoBehaviour
{
    private bool playerInRange;

    [Header("Настройки")]
    public string houseSceneName = "PetrsHouseIn"; // Или используйте переход без сцены

    void Update()
    {
        if (!playerInRange) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryEnterHouse();
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

        DialogueManager.Instance.StartDialogue(data);
    }

    void EnterHouse()
    {
        // Переходим в интерьер дома
        PetrsHouseInterior.Instance?.ActivateInterior(transform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Кто-то вошёл в триггер: " + other.name + ", тег: " + other.tag);

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Игрок рядом с домом!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Кто-то вышел из триггера: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Игрок ушёл от дома");
        }
    }
}