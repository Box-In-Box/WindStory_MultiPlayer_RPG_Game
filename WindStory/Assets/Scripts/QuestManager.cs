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
        questList.Add(10, new QuestData("어르신의 시험", 1000, "마을 촌장의 테스트를 통과하기 위해 무른응가 1마리를 잡아오자.", "무른응가 0 / 1", "공중점프"));
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
        if (_questId != 0)  //퀘스트일 때만 실행
        {
            if (!npc.GetComponent<NPCData>().isCleared(_questId))   //퀘스트가 클리어 되어 있을 때는 실행 안함
            {
                if ( questList[_questId].condition == 2 ) //퀘스트 완료
                {
                    questList[_questId].condition = 0;
                    npc.GetComponent<NPCData>().QuestClear(_questId);

                    for (int i = 0; i < questLogListContent.childCount; i++)
                    {
                        if (questList[_questId].questID == questLogListContent.GetChild(i).gameObject.GetComponent<QuestData>().questID)
                            Destroy(questLogListContent.GetChild(i).gameObject);
                    }
                }
                else if (questList[_questId].condition == 1 )   //퀘스트 진행중...목표달성 확인   ***목표 설정하기 귀찮네 나중에
                {
                    /*
                     * goal 설정 혹은 다른 곳에서
                     */
                    questList[_questId].condition++;        //목표 만들면 해당 줄 삭제 필요 => 목표 달성 못하면 컨디션 1 무한루프 --- 다른곳에서 컨디션 변경 필요
                    for(int i = 0; i < questLogListContent.childCount; i++)
                    {
                        if (questList[_questId].questID == questLogListContent.GetChild(i).gameObject.GetComponent<QuestData>().questID)
                        {
                            questLogListContent.GetChild(i).gameObject.GetComponent<QuestData>().condition++;
                            quest.transform.GetChild(1).gameObject.SetActive(true);
                        }
                    }
                }
                else    //퀘스트 수락
                {
                    questList[_questId].condition++;
                    quest = Instantiate(questObj, questLogListContent.transform);   //퀘스트로그에 퀘스트 생성
                    quest.GetComponent<QuestData>().setting(questList[_questId]);
                    quest.transform.GetChild(0).GetComponent<Text>().text = questList[_questId].questName;  //퀘스트 이름 설정
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
