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


    public void ChatUp(int _id)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<ObjectData>().id == _id)
            {
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
