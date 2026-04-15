using UnityEngine;
using UnityEngine.InputSystem;

public class HouseExit : MonoBehaviour
{
    private bool playerInRange;

    void Update()
    {
        if (!playerInRange) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            PetrsHouseInterior.Instance?.ExitInterior();
        }
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