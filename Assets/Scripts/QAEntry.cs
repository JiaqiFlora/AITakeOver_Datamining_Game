
[System.Serializable]
public class QAEntry
{
    public string question;
    public string answer;
    public bool is_human;
    public bool predicted_is_human;
}

[System.Serializable]
public class QAList
{
    public QAEntry[] entries;
}
