using UnityEngine;

public class Bus : MonoBehaviour
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
            InteractButtonManager.Instance?.ShowButton("Войти в автобус", EnterBus);
        }
    }

    void EnterBus()
    {
        InteractButtonManager.Instance?.HideButton();
        BusInterior.Instance?.ActivateInterior(transform.position);
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