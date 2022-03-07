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
        questList.Add(10, new QuestData("어르신의 시험", new int[] { 1000 }));
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
                }
                else if (questList[_questId].condition == 1 )   //퀘스트 진행중...목표달성 확인   ***목표 설정하기 귀찮네 나중에
                {
                    questList[_questId].condition++;        //목표 만들면 해당 줄 삭제 필요 => 목표 달성 못하면 컨디션 1 무한루프 --- 다른곳에서 컨디션 변경 필요
                }
                else    //퀘스트 수락
                {
                    questList[_questId].condition++;
                }
            }
        }
    }
}
