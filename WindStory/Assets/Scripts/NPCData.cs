using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData : ObjectData
{
    [Header("---NPC---")]
    public int[] NPCQuest;
    public bool[] isNPCQuestClear;

    private void Start()
    {
        setting();
    }

    void setting()
    {
        isNPCQuestClear = new bool[NPCQuest.Length];
    }

    public bool isCleared(int _questID)
    {
        for (int i = 0; i < NPCQuest.Length; i++)
        {
            if (NPCQuest[i] == _questID)
            {
                return isNPCQuestClear[i];
            }
        }
        return false;
    }

    public void QuestClear(int _questID)
    {
        for (int i = 0; i < NPCQuest.Length; i++)
        {
            if (NPCQuest[i] == _questID)
            {
                isNPCQuestClear[i] = true;
                return;
            }
        }
    }
}
