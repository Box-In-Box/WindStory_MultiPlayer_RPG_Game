using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance;
    Dictionary<int, string[]> talkData;

    void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);

        this.talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string[] { "안녕하세요 무르비시리라고 합니다.\n\n\n 제작자 : dgf0000@naver.com" });

        talkData.Add(1000 + 10, new string[] { "넓은 세상으로 모험을 떠나는 것도 좋지만 우선 기본이 확실해야 하지 않겠는가?\n\n게다가 그렇게 맨손으로 여행을 다니다가 몬스터라도 만나면 얻어 맞기 십상이라네.\n허허허~"
            , "내가 여기 간단한 무기를 줄테니 우선 마을 앞마당의 몬스터를 한마리만 사냥하고 오게나.\n\n 그럼 나도 더이상 자네를 말리지 않겠네.\n 어디 한 번 해 보겠나?" });
        talkData.Add(1000 + 11, new string[] { "마을 앞마당의 \"무른응가\"를 한마리 잡아보게..\n 실력이 있어야 모험을 떠나지 않겠나?" });
        talkData.Add(1000 + 12, new string[] { "벌써 몬스터를 사냥하고 왔구만\n\n이제 나도 말라지 않겠네 모험을 즐기게나" });

        talkData.Add(1100, new string[] { "으윽...산을 오르다가 미끄러져 그만 다리를 다치고 말았습니다.\n\n집에 거의 다 와서 이게 무슨 일이람..."
            , "여행자님...여기서 조금만 더 가면 저의 집이 있는데 수고스럽겠지만 저의 아내에게서 빨간 약을 받아서 제게 가져다 주시겠습니까?\n\n제발 부탁드립니다. 아니구 내다리! ㅜ.ㅜ" });
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }
}
