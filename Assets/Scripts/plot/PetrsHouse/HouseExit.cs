using UnityEngine;

public class HouseExit : MonoBehaviour
{
    private bool playerInRange;

    void Update()
    {
        if (playerInRange)
        {
            InteractButtonManager.Instance?.ShowButton("Выйти из дома", ExitHouse);
        }
    }

    void ExitHouse()
    {
        InteractButtonManager.Instance?.HideButton();
        PetrsHouseInterior.Instance?.ExitInterior();
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