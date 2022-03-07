using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private PlayerController playerController;
    public Dictionary<int, QuestData> questList;

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);

        questList = new Dictionary<int, QuestData>();
    }

    public void Setting(GameObject _LocalPlayer)
    {
        playerController = _LocalPlayer.GetComponent<PlayerController>();
        GenerateData();
    }

    void GenerateData()
    {
        questList.Add(10, new QuestData("����� ����", new int[] { 1000 }));
    }

    public int GetQuest(int _questId)
    {
        if (questList.ContainsKey(_questId)) return _questId;
        else return 0;
    }

    public int GetQuestCondition(int _questId)
    {
        if (questList.ContainsKey(_questId)) return questList[_questId].condition;
        else return 0;
    }

    public void CheckQuest(int _questId, GameObject npc)
    {
        if (_questId != 0)  //����Ʈ�� ���� ����
        {
            if (!npc.GetComponent<NPCData>().isCleared(_questId))   //����Ʈ�� Ŭ���� �Ǿ� ���� ���� ���� ����
            {
                if ( questList[_questId].condition == 2 ) //����Ʈ �Ϸ�
                {
                    questList[_questId].condition = 0;
                    npc.GetComponent<NPCData>().QuestClear(_questId);
                }
                else if (questList[_questId].condition == 1 )   //����Ʈ ������...��ǥ�޼� Ȯ��   ***��ǥ �����ϱ� ������ ���߿�
                {
                    questList[_questId].condition++;        //��ǥ ����� �ش� �� ���� �ʿ� => ��ǥ �޼� ���ϸ� ����� 1 ���ѷ��� --- �ٸ������� ����� ���� �ʿ�
                }
                else    //����Ʈ ����
                {
                    questList[_questId].condition++;
                }
            }
        }
    }
}
