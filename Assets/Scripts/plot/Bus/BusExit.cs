using UnityEngine;

public class BusExit : MonoBehaviour
{
    private bool playerInRange;

    void Update()
    {
        if (playerInRange)
        {
            InteractButtonManager.Instance?.ShowButton("Выйти из автобуса", ExitBus);
        }
    }

    void ExitBus()
    {
        InteractButtonManager.Instance?.HideButton();
        BusInterior.Instance?.ExitInterior();
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