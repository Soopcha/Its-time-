using UnityEngine;

public class PetrDialogue : MonoBehaviour
{
    private SaveableNPC saveableNPC;

    void Awake()
    {
        saveableNPC = GetComponent<SaveableNPC>();
    }

    public void StartInitialDialogue()
    {
        if (saveableNPC != null && saveableNPC.HasTalked)
        {
            Debug.Log("Петр уже разговаривал с игроком");
            return;
        }

        var data = new DialogueData();

        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("Петр", "Что за…"));
        node0.Lines.Add(new DialogueLine("Петр", "Давно никого не было в этих краях."));
        node0.Lines.Add(new DialogueLine("Петр", "Чем могу помочь, девушка?"));
        node0.NextNodeIndex = 1;

        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("---", "Здравствуйте…"));
        node1.Lines.Add(new DialogueLine("---", "Вы не собираетесь эвакуироваться?"));
        node1.NextNodeIndex = 2;

        var node2 = data.AddNode();
        node2.Lines.Add(new DialogueLine("Петр", "Эвакуация?"));
        node2.Lines.Add(new DialogueLine("Петр", "*перелистывает журнал, не поднимая глаз*"));
        node2.NextNodeIndex = 3;

        var node3 = data.AddNode();
        node3.Lines.Add(new DialogueLine("---", "Ну да… В четверг. На востоке, возле телебашни."));
        node3.Lines.Add(new DialogueLine("---", "Я не знаю точно когда, но может быть стоит проверить?"));
        node3.NextNodeIndex = 4;

        var node4 = data.AddNode();
        node4.Lines.Add(new DialogueLine("Петр", "А, эта байка…"));
        node4.Lines.Add(new DialogueLine("Петр", "Никто никогда не приезжал. Да и не приедет."));
        node4.NextNodeIndex = 5;

        var node5 = data.AddNode();
        node5.Lines.Add(new DialogueLine("---", "Но вдруг? Вы хотя бы проверяли?"));
        node5.NextNodeIndex = 6;

        var node6 = data.AddNode();
        node6.Lines.Add(new DialogueLine("Петр", "Нет. У меня здесь свои дела."));
        node6.NextNodeIndex = 7;

        var node7 = data.AddNode();
        node7.Lines.Add(new DialogueLine("---", "Понятно… Тогда до свидания."));
        node7.NextNodeIndex = 8;

        var node8 = data.AddNode();
        node8.Lines.Add(new DialogueLine("Петр", "Постой-ка."));
        node8.Lines.Add(new DialogueLine("Петр", "А может, останешься? Вдвоём спокойнее."));
        node8.NextNodeIndex = 9;

        var node9 = data.AddNode();
        node9.Lines.Add(new DialogueLine("---", "Нет, спасибо."));
        node9.NextNodeIndex = 10;

        var node10 = data.AddNode();
        node10.Lines.Add(new DialogueLine("Петр", "Эх, зря отказываешься."));
        node10.Lines.Add(new DialogueLine("Петр", "Тут опасно одной бродить. Да и… приятная компания не помешала бы."));
        node10.Lines.Add(new DialogueLine("Петр", "Молодая девушка вроде тебя…"));
        node10.NextNodeIndex = 11;

        var node11 = data.AddNode();
        node11.Lines.Add(new DialogueLine("---", "Мне пора."));
        node11.NextNodeIndex = 12;

        var node12 = data.AddNode();
        node12.Lines.Add(new DialogueLine("Петр", "*встаёт со стула*"));
        node12.Lines.Add(new DialogueLine("Петр", "Не торопись уходить. Я серьёзно говорю."));
        node12.NextNodeIndex = 13;

        var node13 = data.AddNode();
        node13.Choices.Add(new DialogueChoice(
            "Извините, правда не могу. *быстро уйти*",
            14
        ));
        node13.Choices.Add(new DialogueChoice(
            "Отойдите от двери! Немедленно!",
            15,
            "BAD_END"
        ));

        var node14 = data.AddNode();
        node14.Lines.Add(new DialogueLine("---", "*Вы быстро вышли из дома, закрыв за собой дверь*"));
        node14.Lines.Add(new DialogueLine("---", "*Сердце колотится…*"));
        node14.Lines.Add(new DialogueLine("---", "Нужно уходить. Немедленно."));
        node14.NextNodeIndex = -1;

        var node15 = data.AddNode();
        node15.Lines.Add(new DialogueLine("Петр", "Отойти?"));
        node15.Lines.Add(new DialogueLine("Петр", "*лицо темнеет*"));
        node15.Lines.Add(new DialogueLine("Петр", "Не надо было так со мной разговаривать."));
        node15.Lines.Add(new DialogueLine("---", "Что вы…"));
        node15.Lines.Add(new DialogueLine("Петр", "*резко хватает ружьё*"));
        node15.Lines.Add(new DialogueLine("---", "НЕТ! СТОЙТЕ!"));
        node15.Lines.Add(new DialogueLine("", "*удар прикладом*"));
        node15.Lines.Add(new DialogueLine("", "*хруст костей*"));
        node15.Lines.Add(new DialogueLine("---", "АААА!!!"));
        node15.Lines.Add(new DialogueLine("", "…"));
        node15.Lines.Add(new DialogueLine("", "*несколько месяцев спустя*"));
        node15.NextNodeIndex = -1;

        DialogueManager.Instance.StartDialogue(data,
            onEnd: () => {
                GameState.Instance.PetrCutsceneDone = true;

                if (saveableNPC != null)
                {
                    saveableNPC.HasTalked = true;
                }
            },
            onAction: (action) => {
                if (action == "BAD_END")
                {
                    ShowBadEnding();
                }
            }
        );
    }

    void ShowBadEnding()
    {
        GameState.Instance.PetrCutsceneDone = true;

        if (EndingScreen.Instance != null)
        {
            EndingScreen.Instance.ShowBadEnding1();
        }
        else
        {
            Debug.LogError("EndingScreen не найден!");
        }
    }
}