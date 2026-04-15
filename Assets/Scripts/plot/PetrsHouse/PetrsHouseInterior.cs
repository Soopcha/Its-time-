using UnityEngine;

public class PetrsHouseInterior : MonoBehaviour
{
    public static PetrsHouseInterior Instance;

    [Header("Настройки интерьера")]
    public GameObject InteriorRoot;
    public Transform PlayerSpawnPoint;
    public Transform ExitPoint; // Куда вернуться при выходе

    private Vector3 outsidePosition;
    private GameObject player;

    void Awake()
    {
        Instance = this;

        // Интерьер скрыт по умолчанию
        if (InteriorRoot != null)
            InteriorRoot.SetActive(false);
    }

    public void ActivateInterior(Vector3 outsidePos)
    {
        outsidePosition = outsidePos;
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player не найден!");
            return;
        }

        // Показываем интерьер
        InteriorRoot.SetActive(true);

        // Телепортируем игрока
        player.transform.position = PlayerSpawnPoint.position;

        // Запускаем диалог с Петром если не был
        if (!GameState.Instance.PetrCutsceneDone)
        {
            PetrDialogue petr = FindObjectOfType<PetrDialogue>();
            if (petr != null)
            {
                petr.StartInitialDialogue();
            }
        }
    }

    public void ExitInterior()
    {
        // Скрываем интерьер
        InteriorRoot.SetActive(false);

        // Возвращаем игрока наружу
        if (player != null)
        {
            player.transform.position = outsidePosition;
        }
    }
}