using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    public string questName;
    public int questID;
    public string discription;
    public string progress;
    public string rewards;
    public int condition;    //0: ����Ʈ����   1: ����Ʈ��   2:����Ʈ�Ϸ�

    public QuestData(string _name, int _npcID, string _discription = "Test Discription", string _progress = "Test Progress", string _rewards = "Test Rewards")
    {
        questName = _name;
        questID = _npcID;
        discription = _discription;
        progress = _progress;
        rewards = _rewards;
    }

    public void setting(QuestData _questData)
    {
        questName = _questData.questName;
        questID = _questData.questID;
        condition = _questData.condition;

        discription = _questData.discription;
        progress = _questData.progress;
        rewards = _questData.rewards;
    }
}
