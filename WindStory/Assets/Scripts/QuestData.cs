using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    public string questName;
    public int questID;

    public int condition;    //0: ����Ʈ����   1: ����Ʈ��   2:����Ʈ�Ϸ�

    public QuestData(string name, int[] npcID) { }
}
