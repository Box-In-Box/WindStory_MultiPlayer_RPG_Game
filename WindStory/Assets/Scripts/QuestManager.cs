using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private PlayerController playerController;
    public Dictionary<int, QuestData> questList;

    public GameObject questObj;
    public Transform questLogListContent;
    public Transform QuestContent;
    private GameObject quest;

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
        questList.Add(10, new QuestData("����� ����", 1000, "���� ������ �׽�Ʈ�� ����ϱ� ���� �������� 1������ ��ƿ���.", "�������� 0 / 1", "��������"));
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

                    for (int i = 0; i < questLogListContent.childCount; i++)
                    {
                        if (questList[_questId].questID == questLogListContent.GetChild(i).gameObject.GetComponent<QuestData>().questID)
                            Destroy(questLogListContent.GetChild(i).gameObject);
                    }
                }
                else if (questList[_questId].condition == 1 )   //����Ʈ ������...��ǥ�޼� Ȯ��   ***��ǥ �����ϱ� ������ ���߿�
                {
                    /*
                     * goal ���� Ȥ�� �ٸ� ������
                     */
                    questList[_questId].condition++;        //��ǥ ����� �ش� �� ���� �ʿ� => ��ǥ �޼� ���ϸ� ����� 1 ���ѷ��� --- �ٸ������� ����� ���� �ʿ�
                    for(int i = 0; i < questLogListContent.childCount; i++)
                    {
                        if (questList[_questId].questID == questLogListContent.GetChild(i).gameObject.GetComponent<QuestData>().questID)
                        {
                            questLogListContent.GetChild(i).gameObject.GetComponent<QuestData>().condition++;
                            quest.transform.GetChild(1).gameObject.SetActive(true);
                        }
                    }
                }
                else    //����Ʈ ����
                {
                    questList[_questId].condition++;
                    quest = Instantiate(questObj, questLogListContent.transform);   //����Ʈ�α׿� ����Ʈ ����
                    quest.GetComponent<QuestData>().setting(questList[_questId]);
                    quest.transform.GetChild(0).GetComponent<Text>().text = questList[_questId].questName;  //����Ʈ �̸� ����
                    quest.GetComponent<Button>().onClick.AddListener(QuestdetailBtn);
                }
            }
        }
    }

    public void QuestdetailBtn()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        QuestContent.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Text>().text = clickObject.GetComponent<QuestData>().questName;
        QuestContent.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = clickObject.GetComponent<QuestData>().discription;
        QuestContent.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Text>().text = clickObject.GetComponent<QuestData>().progress;
        QuestContent.GetChild(3).gameObject.transform.GetChild(0).GetComponent<Text>().text = clickObject.GetComponent<QuestData>().rewards;
    }
}
