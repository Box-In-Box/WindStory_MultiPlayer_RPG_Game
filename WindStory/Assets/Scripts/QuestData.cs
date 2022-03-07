using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    public string questName;
    public int questID;

    public int condition;    //0: 퀘스트시작   1: 퀘스트중   2:퀘스트완료

    public QuestData(string name, int[] npcID) { }
}
