using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string Speaker;
    public string Text;

    public DialogueLine(string speaker, string text)
    {
        Speaker = speaker;
        Text = text;
    }
}

[System.Serializable]
public class DialogueChoice
{
    public string Text;
    public int NextNodeIndex;
    public string Action;

    public DialogueChoice(string text, int nextNode, string action = "")
    {
        Text = text;
        NextNodeIndex = nextNode;
        Action = action;
    }
}

[System.Serializable]
public class DialogueNode
{
    public List<DialogueLine> Lines = new List<DialogueLine>();
    public List<DialogueChoice> Choices = new List<DialogueChoice>();
    public int NextNodeIndex = -1;

    public bool HasChoices => Choices != null && Choices.Count > 0;
}

public class DialogueData
{
    public List<DialogueNode> Nodes = new List<DialogueNode>();

    public DialogueNode AddNode()
    {
        var node = new DialogueNode();
        Nodes.Add(node);
        return node;
    }
}