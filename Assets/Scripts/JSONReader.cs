using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset jsonFile;

    // Start is called before the first frame update
    void Start()
    {
        ReadJsonData();
    }

    void ReadJsonData()
    {
        QAList qaList = JsonUtility.FromJson<QAList>("{\"entries\":" + jsonFile.text + "}");

        //foreach(QAEntry entry in qaList.entries)
        //{
        //    Debug.Log("=========");
        //    Debug.Log("Questions: " + entry.question);
        //    Debug.Log("Answer: " + entry.answer);
        //    Debug.Log("Is huma: " + entry.is_human);
        //    Debug.Log("Predicted is human: " + entry.predicted_is_human);
        //}

        GameManagement.instance.SetupQAList(qaList);
    }
}
