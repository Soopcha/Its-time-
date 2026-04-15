using UnityEngine;

public class PetrDialogue : MonoBehaviour
{
    public void StartInitialDialogue()
    {
        var data = new DialogueData();

        // Узел 0: Начало
        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("странный мужик", "Какого черта..."));
        node0.Lines.Add(new DialogueLine("странный мужик", "Давно тут крысы не пробегали"));
        node0.Lines.Add(new DialogueLine("странный мужик", "Чего надо?"));
        node0.NextNodeIndex = 1;

        // Узел 1
        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("---", "Здравствуйте"));
        node1.Lines.Add(new DialogueLine("---", "Вы не торопитесь эвакуироваться?"));
        node1.NextNodeIndex = 2;

        // Узел 2
        var node2 = data.AddNode();
        node2.Lines.Add(new DialogueLine("странный мужик", "Че?"));
        node2.Lines.Add(new DialogueLine("странный мужик", "*звук страниц*"));
        node2.NextNodeIndex = 3;

        // Узел 3
        var node3 = data.AddNode();
        node3.Lines.Add(new DialogueLine("---", "Ну в четверг... где-то на востоке в высоком здании. Рядом с телебашней."));
        node3.Lines.Add(new DialogueLine("---", "Я правда не знаю когда четверг, может уже и прошла давно эвакуация, но раз она там была, может еще раз будет."));
        node3.NextNodeIndex = 4;

        // Узел 4
        var node4 = data.AddNode();
        node4.Lines.Add(new DialogueLine("странный мужик", "Аа.. забей."));
        node4.Lines.Add(new DialogueLine("странный мужик", "Никто не приезжал никогда да и не приедет"));
        node4.NextNodeIndex = 5;

        // Узел 5
        var node5 = data.AddNode();
        node5.Lines.Add(new DialogueLine("---", "А вдруг, лучше проверить! Вы ходили туда?"));
        node5.NextNodeIndex = 6;

        // Узел 6
        var node6 = data.AddNode();
        node6.Lines.Add(new DialogueLine("странный мужик", "Не, мне и так забот хватает."));
        node6.NextNodeIndex = 7;

        // Узел 7
        var node7 = data.AddNode();
        node7.Lines.Add(new DialogueLine("---", "Хорошо... До свидания."));
        node7.NextNodeIndex = 8;

        // Узел 8
        var node8 = data.AddNode();
        node8.Lines.Add(new DialogueLine("странный мужик", "Стой."));
        node8.Lines.Add(new DialogueLine("странный мужик", "Не хочешь остаться?"));
        node8.NextNodeIndex = 9;

        // Узел 9
        var node9 = data.AddNode();
        node9.Lines.Add(new DialogueLine("---", "Нет."));
        node9.NextNodeIndex = 10;

        // Узел 10
        var node10 = data.AddNode();
        node10.Lines.Add(new DialogueLine("странный мужик", "Ахахах. Зачем так грубо. Будем вместе быт вести. Ты молодая, красивая."));
        node10.NextNodeIndex = 11;

        // Узел 11
        var node11 = data.AddNode();
        node11.Lines.Add(new DialogueLine("---", "А вы нет."));
        node11.NextNodeIndex = 12;

        // Узел 12
        var node12 = data.AddNode();
        node12.Lines.Add(new DialogueLine("странный мужик", "Эй, дефка, не провоцируй меня."));
        node12.NextNodeIndex = 13;

        // Узел 13: ВЫБОР
        var node13 = data.AddNode();
        node13.Choices.Add(new DialogueChoice(
            "Я шучу так простите, до свидания! *убежать*",
            14 // Узел побега
        ));
        node13.Choices.Add(new DialogueChoice(
            "Вы такой мерзкий",
            15, // Узел плохой концовки
            "BAD_END"
        ));

        // Узел 14: Побег
        var node14 = data.AddNode();
        node14.Lines.Add(new DialogueLine("---", "*Вы быстро выбежали из дома*"));

        // Узел 15: Плохая концовка
        var node15 = data.AddNode();
        node15.Lines.Add(new DialogueLine("странный мужик", "..."));
        node15.Lines.Add(new DialogueLine("", "*выстрел*"));
        node15.Lines.Add(new DialogueLine("", "..."));

        DialogueManager.Instance.StartDialogue(data,
            onEnd: () => {
                GameState.Instance.PetrCutsceneDone = true;
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
        // Здесь можно показать CG или экран концовки
        Debug.Log("ПЛОХАЯ КОНЦОВКА: Петр пристрелил героиню");
        GameState.Instance.PetrCutsceneDone = true;

        // Можно загрузить сцену Game Over или показать UI
    }
}