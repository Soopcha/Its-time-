using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public Direction InputDirection;

    [Header("Двойное нажатие")]
    public float DoubleTapTime = 0.3f; // Время между нажатиями

    private bool isPressed = false;
    private float lastTapTime = 0f;
    private bool isDoubleTapActive = false;

    public bool IsPressed => isPressed;
    public bool IsDoubleTapRunning => isDoubleTapActive && isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;

        // Проверяем двойное нажатие
        float timeSinceLastTap = Time.time - lastTapTime;

        if (timeSinceLastTap <= DoubleTapTime)
        {
            // Двойное нажатие обнаружено!
            isDoubleTapActive = true;
            Debug.Log($"Двойное нажатие на {InputDirection}! БЕГ!");
        }
        else
        {
            // Первое нажатие
            isDoubleTapActive = false;
        }

        lastTapTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        // Сбрасываем бег только если отпустили кнопку
        if (!isPressed)
        {
            isDoubleTapActive = false;
        }
    }

    void OnDisable()
    {
        isPressed = false;
        isDoubleTapActive = false;
    }
}