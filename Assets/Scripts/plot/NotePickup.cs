using UnityEngine;
using UnityEngine.InputSystem;

public class NotePickup : MonoBehaviour
{
    private bool playerInRange;

    void Update()
    {
        if (!playerInRange) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickupNote();
        }
    }

    void PickupNote()
    {
        var data = new DialogueData();

        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("мятая записка",
            "идете на восток.. эвакуация... в четверх.. 10.00..."));
        node0.Lines.Add(new DialogueLine("мятая записка",
            "*записка смята, остальное не разобрать*"));
        node0.NextNodeIndex = 1;

        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("---", "Что.."));
        node1.Lines.Add(new DialogueLine("---", "Ладно. Надо поторопиться."));
        node1.Lines.Add(new DialogueLine("---", "А.. что тут рядом в земле?"));
        node1.Lines.Add(new DialogueLine("---", "Ключ?"));
        node1.Lines.Add(new DialogueLine("---", "*взяла ключ*"));

        DialogueManager.Instance.StartDialogue(data, onEnd: () =>
        {
            GameState.Instance.HasReadNote = true;
            GameState.Instance.HasKey = true;
            Debug.Log("Записка прочитана! Ключ получен!");
            Destroy(gameObject);
        });
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}