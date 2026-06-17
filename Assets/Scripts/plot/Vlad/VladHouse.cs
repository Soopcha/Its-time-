using UnityEngine;

public class VladHouse : MonoBehaviour
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
            // НОВОЕ: Проверка доступности
            if (!GameState.Instance.CanEnterVladHouse())
            {
                // Показываем сообщение что закрыто (без функции клика)
                InteractButtonManager.Instance?.ShowButton("Закрыто", null);
                return;
            }

            InteractButtonManager.Instance?.ShowButton("Войти в дом", EnterHouse);
        }
    }

    void EnterHouse()
    {
        InteractButtonManager.Instance?.HideButton();
        VladHouseInterior.Instance?.ActivateInterior(transform.position);
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