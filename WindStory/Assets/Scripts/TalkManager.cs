using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    void Awake()
    {
        this.talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string[] { "여기는 윈드스토리입니다. \n자유롭게 즐겨주시기 바랍니다. \n저는 마을 장로 \'무루비시리\'라고 합니다.\n\n 제작자: dgf0000@naver.com" });
    }

    public string GetTalk(int id, int talkIndex)
    {
        return talkData[id][talkIndex];
    }
}
