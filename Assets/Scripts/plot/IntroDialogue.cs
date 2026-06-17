using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    void Start()
    {
        // Проверяем что это новая игра
        bool shouldGenerate = PlayerPrefs.GetInt("ShouldGenerate", 1) == 1;

        if (!shouldGenerate)
        {
            Debug.Log("Интро пропущен - это загрузка");
            return;
        }

        // Запускаем через 1 секунду после старта
        Invoke(nameof(ShowIntro), 1f);
    }

    void ShowIntro()
    {
        var data = new DialogueData();

        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("---", "Чёрт... Так голова болит..."));
        node0.Lines.Add(new DialogueLine("---", "Где я?"));
        node0.NextNodeIndex = 1;

        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("", "*Ты найдёшь подсказки, исследуя зоны последовательно*"));
        node1.Lines.Add(new DialogueLine("", "*Если окружение поменялось - ты уже в другой зоне*"));
        node1.NextNodeIndex = -1;

        DialogueManager.Instance.StartDialogue(data, onEnd: () => {
            Debug.Log("Интро завершён");
        });
    }
}