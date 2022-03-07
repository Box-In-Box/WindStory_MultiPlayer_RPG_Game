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

    //�� ���� �ִ� NPC ��ũ��Ʈ���� ���� id�� ã������ �� id�� ä���� �ö��
    public void ChatUp(int _id)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<ObjectData>().id == _id)  //���� �ʿ��� _id�� ���� �ؼ� ���� id Ȯ��
            {   //id�� ������Ʈ�� ������ ���� �ؽ�Ʈ ������ ����
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
