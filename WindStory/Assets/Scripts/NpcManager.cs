using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcManager : MonoBehaviour
{
    public List<int> npcNum;

    public GameObject curChat;
    public Text curText;

    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
            npcNum.Add(transform.GetChild(i).gameObject.GetComponent<ObjectData>().id);
    }

    //맵 마다 있는 NPC 스크립트에서 같은 id가 찾아지면 그 id만 채팅이 올라옴
    public void ChatUp(int _id)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<ObjectData>().id == _id)  //현재 맵에서 _id를 조사 해서 같은 id 확인
            {   //id의 컴포넌트를 가져와 현재 텍스트 정보를 저장
                curChat = transform.GetChild(i).gameObject.transform.GetChild(0).gameObject;
                curChat.SetActive(true);
                curText = transform.GetChild(i).gameObject.transform.Find("Chat").gameObject.transform.Find("Canvas").gameObject.transform.Find("ChatText").gameObject.GetComponent<Text>();
            }
        }
    }

    public void ChatDown()
    {
        curChat.SetActive(false);
    }

    public bool IsTalk(string _npcName)
    {
        return GameObject.Find(_npcName).transform.GetChild(0).gameObject.activeSelf == true ? true : false;
    }
}
