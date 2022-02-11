using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcScript : MonoBehaviour
{
    public TalkManager theTalk;
    public List<int> npcNum;

    string talkData;

    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
            npcNum.Add(transform.GetChild(i).gameObject.GetComponent<ObjectData>().id);
    }

    public void TalkCancel(string _npcName)
    {
        GameObject.Find(_npcName).transform.GetChild(0).gameObject.SetActive(false);
    }

    public bool IsTalk(string _npcName)
    {
        return GameObject.Find(_npcName).transform.GetChild(0).gameObject.activeSelf == true ? true : false;
    }

    public void Talk(int _id, int _talkIndex)
    {
        talkData = theTalk.GetTalk(_id, _talkIndex);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<ObjectData>().id == _id)
            {
                transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(i).gameObject.transform.Find("Chat").gameObject.transform.Find("Canvas").gameObject.transform.Find("ChatText").gameObject.GetComponent<Text>().text = talkData;
            }
        }
        
    }
}
