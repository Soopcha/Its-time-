using UnityEngine;

public class BusInterior : MonoBehaviour
{
    public static BusInterior Instance;

    [Header("Настройки интерьера")]
    public GameObject InteriorRoot;
    public Transform PlayerSpawnPoint;

    private Vector3 outsidePosition;
    private GameObject player;

    void Awake()
    {
        Instance = this;

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

        InteriorRoot.SetActive(true);
        player.transform.position = PlayerSpawnPoint.position;

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.SnapToTarget();
        }

        // Запускаем диалог с Никой если ещё не был
        if (!GameState.Instance.NikaCutsceneDone)
        {
            BusDialogue busDialogue = FindObjectOfType<BusDialogue>();
            if (busDialogue != null)
            {
                busDialogue.StartDialogue();
            }
        }
    }

    public void ExitInterior()
    {
        InteriorRoot.SetActive(false);

        if (player != null)
        {
            player.transform.position = outsidePosition;
        }

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.SnapToTarget();
        }
    }
}