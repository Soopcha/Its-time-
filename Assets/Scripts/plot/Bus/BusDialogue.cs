using UnityEngine;

public class BusDialogue : MonoBehaviour
{
    private int points = 0;

    public void StartDialogue()
    {
        points = 0;

        var data = new DialogueData();

        // Узел 0: Начало
        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("Ника", "(громко) Кто там!?"));
        node0.Lines.Add(new DialogueLine("Ника", "А.. Фух. Я уж думала пришёл снова тот больной.."));
        node0.Lines.Add(new DialogueLine("Ника", "А ты кто?"));
        node0.NextNodeIndex = 1;

        // Узел 1
        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("---", "Я.. Честно сказать, не помню."));
        node1.NextNodeIndex = 2;

        // Узел 2
        var node2 = data.AddNode();
        node2.Lines.Add(new DialogueLine("Ника", "*ухмыльнулась*"));
        node2.Lines.Add(new DialogueLine("Ника", "Аахахх, ну ладно. А зовут как?"));
        node2.NextNodeIndex = 3;

        // Узел 3
        var node3 = data.AddNode();
        node3.Lines.Add(new DialogueLine("---", "..."));
        node3.NextNodeIndex = 4;

        // Узел 4
        var node4 = data.AddNode();
        node4.Lines.Add(new DialogueLine("Ника", "А я Ника, приятно познакомиться. И вот мой брат Рома."));
        node4.NextNodeIndex = 5;

        // Узел 5
        var node5 = data.AddNode();
        node5.Lines.Add(new DialogueLine("Рома", "Приветик."));
        node5.NextNodeIndex = 6;

        // Узел 6
        var node6 = data.AddNode();
        node6.Lines.Add(new DialogueLine("---", "Приветик."));
        node6.NextNodeIndex = 7;

        // Узел 7: ВЫБОР ПРО РОМУ
        var node7 = data.AddNode();
        node7.Choices.Add(new DialogueChoice("Спросить что с ним", 8, "ASK_ROMA"));
        node7.Choices.Add(new DialogueChoice("Промолчать", 9, "SKIP_ROMA"));

        // Узел 8: Спросили про Рому (0 очков)
        var node8 = data.AddNode();
        node8.Lines.Add(new DialogueLine("Ника", "Господи, ну каждый ткнёт в это, сколько можно."));
        node8.NextNodeIndex = 9;

        // Узел 9: Продолжение (общий для обоих выборов)
        var node9 = data.AddNode();
        node9.Lines.Add(new DialogueLine("Ника", "Ладно, как ты тут оказалась? Что тебе нужно?"));
        node9.NextNodeIndex = 10;

        // Узел 10
        var node10 = data.AddNode();
        node10.Lines.Add(new DialogueLine("---", "(немного растерянно) Я… правда не помню. Как сюда попала — тоже. Просто шла по полю и увидела ваш автобус."));
        node10.NextNodeIndex = 11;

        // Узел 11
        var node11 = data.AddNode();
        node11.Lines.Add(new DialogueLine("Ника", "(прищурилась, но всё ещё улыбается) Странная ты. Но ладно. Бывает. Мы тут тоже не по своей воле."));
        node11.NextNodeIndex = 12;

        // Узел 12: ВЫБОР ПРО ИСТОРИЮ
        var node12 = data.AddNode();
        node12.Choices.Add(new DialogueChoice("А как вы тут оказались?", 13, "ASK_STORY"));
        node12.Choices.Add(new DialogueChoice("Да.. наверное тяжело вам..", 14, "SKIP_STORY"));

        // Узел 13: История Ники (+1 очко)
        var node13 = data.AddNode();
        node13.Lines.Add(new DialogueLine("Ника", "О, это долгая история..."));
        node13.Lines.Add(new DialogueLine("Ника", "Мы с Ромой ехали на восток, когда всё началось."));
        node13.Lines.Add(new DialogueLine("Ника", "Автобус сломался прямо здесь. Водитель ушёл за помощью и не вернулся."));
        node13.Lines.Add(new DialogueLine("Ника", "Мы решили остаться — внутри хотя бы безопасно."));
        node13.Lines.Add(new DialogueLine("Рома", "В других местах рядом не так уютно."));
        node13.Lines.Add(new DialogueLine("Ника", "Зато научились охотиться! Я теперь вообще крутая охотница."));
        node13.Lines.Add(new DialogueLine("Ника", "*смеётся*"));
        node13.NextNodeIndex = 15;

        // Узел 14: Промолчали (0 очков)
        var node14 = data.AddNode();
        node14.Lines.Add(new DialogueLine("Ника", "Ну да. Но мы справляемся."));
        node14.NextNodeIndex = 15;

        // Узел 15: Пустой узел для завершения первого диалога
        var node15 = data.AddNode();
        node15.Lines.Add(new DialogueLine("", ""));
        node15.NextNodeIndex = -1;

        DialogueManager.Instance.StartDialogue(data,
            onEnd: () =>
            {
                GameState.Instance.NikaCutsceneDone = true;
                ShowEndingByPoints();
            },
            onAction: (action) =>
            {
                if (action == "ASK_ROMA")
                {
                    // Спросила про Рому — 0 очков
                    Debug.Log("Спросила про Рому: 0 очков");
                }
                else if (action == "SKIP_ROMA")
                {
                    // Промолчала — +1 очко
                    points += 1;
                    Debug.Log($"Промолчала про Рому: +1 очко. Итого: {points}");
                }
                else if (action == "ASK_STORY")
                {
                    // Спросила историю — +1 очко
                    points += 1;
                    Debug.Log($"Спросила историю: +1 очко. Итого: {points}");
                }
                else if (action == "SKIP_STORY")
                {
                    // Промолчала — 0 очков
                    Debug.Log("Промолчала про историю: 0 очков");
                }
            }
        );
    }

    void ShowEndingByPoints()
    {
        Debug.Log($"Диалог с Никой завершён. Очков: {points}");

        if (points >= 2)
        {
            StartGoodNikaDialogue();
        }
        else
        {
            StartBadNikaDialogue();
        }
    }

    void StartGoodNikaDialogue()
    {
        var data = new DialogueData();

        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("Ника", "Слушай, а ты милая. Давай назовём тебя.. хм... Соня? Круто?"));
        node0.NextNodeIndex = 1;

        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("---", "Эмм. Вроде мне идёт?"));
        node1.NextNodeIndex = 2;

        var node2 = data.AddNode();
        node2.Lines.Add(new DialogueLine("Ника", "Да конечно! Ахахах."));
        node2.Lines.Add(new DialogueLine("Ника", "Ну ладно, поздно уже. Не хочешь остаться? Переночуем! А если у тебя есть консервы — то вообще ты желанный гость! Хихихи."));
        node2.NextNodeIndex = 3;

        var node3 = data.AddNode();
        node3.Choices.Add(new DialogueChoice("Остаться", 4, "STAY"));
        node3.Choices.Add(new DialogueChoice("Извиниться и уйти", 5, "LEAVE"));

        // Узел 4: Остались
        var node4 = data.AddNode();
        node4.Lines.Add(new DialogueLine("---", "Я бы осталась, да."));
        node4.Lines.Add(new DialogueLine("Ника", "Ура! Рома, подвинься!"));
        node4.Lines.Add(new DialogueLine("Рома", "*улыбается*"));
        node4.NextNodeIndex = -1;

        // Узел 5: Ушли
        var node5 = data.AddNode();
        node5.Lines.Add(new DialogueLine("---", "Спасибо.. но мне правда надо идти."));
        node5.Lines.Add(new DialogueLine("Ника", "Ну ладно. Удачи тебе, Соня!"));
        node5.NextNodeIndex = -1;

        DialogueManager.Instance.StartDialogue(data,
            onEnd: null,
            onAction: (action) =>
            {
                if (action == "STAY")
                {
                    if (EndingScreen.Instance != null)
                    {
                        EndingScreen.Instance.ShowGoodEnding1();
                    }
                }
                else if (action == "LEAVE")
                {
                    BusInterior.Instance?.ExitInterior();
                }
            }
        );
    }

    void StartBadNikaDialogue()
    {
        var data = new DialogueData();

        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("Ника", "Ладно, мы тут спать уже хотели, устали после вылазки."));
        node0.NextNodeIndex = 1;

        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("---", "А.. хорошо."));
        node1.NextNodeIndex = 2;

        var node2 = data.AddNode();
        node2.Lines.Add(new DialogueLine("Ника", "..."));
        node2.NextNodeIndex = 3;

        var node3 = data.AddNode();
        node3.Lines.Add(new DialogueLine("Ника", "Ну ты иди, а то как-то при чужих людях не очень — мы не можем расслабиться."));
        node3.NextNodeIndex = -1;

        DialogueManager.Instance.StartDialogue(data,
            onEnd: () =>
            {
                BusInterior.Instance?.ExitInterior();
            }
        );
    }
}