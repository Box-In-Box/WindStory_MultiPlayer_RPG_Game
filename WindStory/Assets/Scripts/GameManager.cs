using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [Header("---Setting---")]
    public GameObject LocalPlayer;
    public NpcManager npcManager;
    public TalkManager talkManager;
    public TypingEffect typingEffect;
    public GameObject gamePanel;
    public GameObject escapePanel;
    

    [Header("---Default---")]
    public GameObject scanObj;
    public string objectName;
    public int id;
    public bool isNpc;
    public bool isTransfer;

    [Header("---Npc---")]
    public string talkData;
    public int TalkIndex;
    public bool isTalking;

    [Header("---Transfer---")]
    public string currentMapName;
    public string targetMapName;
    public Collider2D currentChinemaCollider;
    public static CinemachineVirtualCamera chinemaCamera;
    public static CinemachineConfiner chinemaConfiner;

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);

        
        chinemaConfiner = FindObjectOfType<CinemachineConfiner>();
    }

    public void Setting(GameObject _LocalPlayer)
    {
        gamePanel.gameObject.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
        LocalPlayer = _LocalPlayer;

        chinemaCamera = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
        chinemaConfiner = FindObjectOfType<CinemachineConfiner>();
        chinemaCamera.Follow = LocalPlayer.transform;
        chinemaCamera.LookAt = LocalPlayer.transform;
    }

    public void SetMapField(string _currentMapName, Collider2D _currentChinemaCollider)
    {
        currentMapName = _currentMapName;
        currentChinemaCollider = _currentChinemaCollider;
        chinemaConfiner.m_BoundingShape2D = _currentChinemaCollider;
    }

    public void Action(GameObject _scanObj)
    {
        //같은 오브젝트이면 한번만 스캔
        if(scanObj == null || scanObj.GetComponent<ObjectData>().id != _scanObj.GetComponent<ObjectData>().id)
        {
            //기본 셋팅
            if(npcManager != null) npcManager.ChatDown(); //기존 챗 내림
            scanObj = _scanObj;
            objectName = scanObj.name;
            id = scanObj.GetComponent<ObjectData>().id;
            isNpc = scanObj.GetComponent<ObjectData>().isNpc;
            isTransfer = scanObj.GetComponent<ObjectData>().isTransfer;

            //Npc셋팅
            if (isNpc)
            {
                npcManager = scanObj.transform.parent.gameObject.GetComponent<NpcManager>();
                TalkIndex = 0;
                isTalking = false;
            } 
        }

        //Npc 대화액션
        if(isNpc)
        {
            if (!isTalking)
            {
                if (!npcManager.IsTalk(objectName)) npcManager.ChatUp(id);
                Talk(id);
            }
            else if (isTalking) Talk(id);
        }

        if(isTransfer)
        {
            //플레이어, 카메라 설정
            currentMapName = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.parent.parent.gameObject.name;
            LocalPlayer.transform.position = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.transform.position;
            currentChinemaCollider = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.transform.parent.parent.gameObject.transform.Find("CMRange_" + currentMapName).gameObject.GetComponent<Collider2D>();
            chinemaConfiner.m_BoundingShape2D = currentChinemaCollider;
        }
    }

    public void Talk(int _objectId)
    {
        if(typingEffect.isPlayingTypingEffect)
        { typingEffect.SetMsg(npcManager.curText.GetComponent<Text>(), ""); return; }
        else talkData = talkManager.GetTalk(_objectId, TalkIndex);
        
        if (talkData == null)
        {
            TalkIndex = 0;
            isTalking = false;
            npcManager.ChatDown();
            return;
        }
        //npcManager.curText.text = talkData;
        typingEffect.SetMsg(npcManager.curText.GetComponent<Text>(), talkData);

        isTalking = true;
        TalkIndex++;
    }
}
