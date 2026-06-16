using UnityEngine;

public class VladDialogue : MonoBehaviour
{
    private bool hasFollowed = false;

    public void StartDialogue()
    {
        hasFollowed = false;

        var data = new DialogueData();

        // Узел 0: Начало
        var node0 = data.AddNode();
        node0.Lines.Add(new DialogueLine("Мужчина", "...!"));
        node0.Lines.Add(new DialogueLine("Мужчина", "Господи..."));
        node0.Lines.Add(new DialogueLine("Мужчина", "Я... думал ты умерла..."));
        node0.Lines.Add(new DialogueLine("Мужчина", "Прости! Если бы я знал, что так выйдет, я бы не отпустил тебя идти с той группой!"));
        node0.NextNodeIndex = 1;

        // Узел 1
        var node1 = data.AddNode();
        node1.Lines.Add(new DialogueLine("---", "Что?"));
        node1.Lines.Add(new DialogueLine("---", "Вы меня знаете?"));
        node1.NextNodeIndex = 2;

        // Узел 2
        var node2 = data.AddNode();
        node2.Lines.Add(new DialogueLine("Мужчина", "А?? Я... да. Ты не помнишь? Я Влад."));
        node2.NextNodeIndex = 3;

        // Узел 3
        var node3 = data.AddNode();
        node3.Lines.Add(new DialogueLine("---", "Простите, я уже некоторое время хожу... пытаюсь хоть что-то вспомнить, но всё печально."));
        node3.NextNodeIndex = 4;

        // Узел 4
        var node4 = data.AddNode();
        node4.Lines.Add(new DialogueLine("Влад", "Ах да, ты же такое пережила, конечно, конечно. Прости! Садись!"));
        node4.Lines.Add(new DialogueLine("Влад", "Я налью чаю! Присаживайся."));
        node4.NextNodeIndex = 5;

        // Узел 5
        var node5 = data.AddNode();
        node5.Lines.Add(new DialogueLine("---", "Спасибо... Но... Может объяснишь мне тогда, кто я? Я ничего не помню."));
        node5.NextNodeIndex = 6;

        // Узел 6
        var node6 = data.AddNode();
        node6.Lines.Add(new DialogueLine("Влад", "Ах да, господи. Ахаха, прости, я просто реально рад, что ты жива."));
        node6.Lines.Add(new DialogueLine("Влад", "Тебя зовут Женя. Мы с тобой не так давно познакомились."));
        node6.Lines.Add(new DialogueLine("Влад", "Помнишь?? Там был я, Виктор, Лера? Нет?"));
        node6.NextNodeIndex = 7;

        // Узел 7
        var node7 = data.AddNode();
        node7.Lines.Add(new DialogueLine("---", "..."));
        node7.NextNodeIndex = 8;

        // Узел 8
        var node8 = data.AddNode();
        node8.Lines.Add(new DialogueLine("Влад", "Недавно ты ушла с ними на добычу еды и не вернулась."));
        node8.Lines.Add(new DialogueLine("Влад", "Виктор и Лера сказали, что тебя унесли эти твари."));
        node8.Lines.Add(new DialogueLine("Влад", "Я был, честно сказать, в гневе и не верю им. Ну, может, они и сказали правду."));
        node8.NextNodeIndex = 9;

        // Узел 9
        var node9 = data.AddNode();
        node9.Lines.Add(new DialogueLine("Влад", "В любом случае они не вернутся, а ты не помнишь..."));
        node9.Lines.Add(new DialogueLine("Влад", "Ладно, всё равно эта парочка бесила меня, я их выгнал."));
        node9.Lines.Add(new DialogueLine("Влад", "Может я не очень хорошо поступил, чёрт..."));
        node9.Lines.Add(new DialogueLine("Влад", "Ну я был немного в гневе. Ладно."));
        node9.NextNodeIndex = 10;

        // Узел 10
        var node10 = data.AddNode();
        node10.Lines.Add(new DialogueLine("Влад", "В любом случае ты вернулась, это замечательно."));
        node10.NextNodeIndex = 11;

        // Узел 11
        var node11 = data.AddNode();
        node11.Lines.Add(new DialogueLine("---", "Я... Женя? Кажется, я вспоминаю... Или нет. Но это точно моё имя."));
        node11.NextNodeIndex = 12;

        // Узел 12
        var node12 = data.AddNode();
        node12.Lines.Add(new DialogueLine("Влад", "Ахахах да. Тяжело тебе. Ну слушай, я могу рассказать всё, что знаю о тебе!"));
        node12.NextNodeIndex = 13;

        // Узел 13
        var node13 = data.AddNode();
        node13.Lines.Add(new DialogueLine("---", "Давай."));
        node13.NextNodeIndex = 14;

        // Узел 14
        var node14 = data.AddNode();
        node14.Lines.Add(new DialogueLine("Влад", "Ну я правда мало знаю, но ты рассказывала, что до всего этого была инженером. Работала даже по профессии. Любила рисовать, шить."));
        node14.Lines.Add(new DialogueLine("Влад", "Но а так ты ужасно готовишь! И неплохо убиваешь чудищ."));
        node14.Lines.Add(new DialogueLine("Влад", "Вспоминается что-то?"));
        node14.NextNodeIndex = 15;

        // Узел 15
        var node15 = data.AddNode();
        node15.Lines.Add(new DialogueLine("---", "Пока нет... Ты мне можешь рассказать о себе?"));
        node15.NextNodeIndex = 16;

        // Узел 16
        var node16 = data.AddNode();
        node16.Lines.Add(new DialogueLine("Влад", "А, да... Я же для тебя как незнакомец."));
        node16.Lines.Add(new DialogueLine("Влад", "Вообщем, я тут живу с момента последней эвакуации. Сам-то я с другого континента, отвечал за эвакуацию людей."));
        node16.Lines.Add(new DialogueLine("Влад", "В итоге спасли только тех, кто отвалил огромную сумму."));
        node16.Lines.Add(new DialogueLine("Влад", "А один, как я понял, даже выкупил моё место — место персонала."));
        node16.Lines.Add(new DialogueLine("Влад", "Так что я теперь застрял тут."));
        node16.NextNodeIndex = 17;

        // Узел 17
        var node17 = data.AddNode();
        node17.Lines.Add(new DialogueLine("Влад", "*Глаза Влада на секунду задрожали*"));
        node17.Lines.Add(new DialogueLine("Влад", "Ладно. Мы ещё успеем поболтать. Пей чай, я щас вернусь."));
        node17.Lines.Add(new DialogueLine("Влад", "Нужно проверить, нет ли тут зомбаков этих рядом."));
        node17.NextNodeIndex = 18;

        // Узел 18: ВЫБОР - СРАЗУ ЗАПУСКАЕТ КОНЦОВКУ
        var node18 = data.AddNode();
        node18.Lines.Add(new DialogueLine("---", "..."));
        node18.Choices.Add(new DialogueChoice("Проследить за ним", -1, "FOLLOW"));
        node18.Choices.Add(new DialogueChoice("Не следить за ним", -1, "STAY"));

        DialogueManager.Instance.StartDialogue(data,
            onEnd: () =>
            {
                GameState.Instance.VladCutsceneDone = true;
            },
            onAction: (action) =>
            {
                if (action == "FOLLOW")
                {
                    hasFollowed = true;
                    Debug.Log("Проследила за Владом - запуск плохой концовки");
                    ShowBadEnding2();
                }
                else if (action == "STAY")
                {
                    hasFollowed = false;
                    Debug.Log("Не следила - запуск хорошей концовки");
                    ShowGoodEnding2();
                }
            }
        );
    }

    // ===== ПЛОХАЯ КОНЦОВКА 2 =====
    void ShowBadEnding2()
    {
        if (EndingScreen.Instance != null)
        {
            EndingScreen.Instance.ShowBadEnding2();
        }
    }

    // ===== ХОРОШАЯ КОНЦОВКА 2 =====
    void ShowGoodEnding2()
    {
        if (EndingScreen.Instance != null)
        {
            EndingScreen.Instance.ShowGoodEnding2();
        }
    }
}