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
    public BGMManager bgmManager;
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

    [Header("---Monster---")]
    public GameObject[] spawnPoint;

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);

        
        chinemaConfiner = FindObjectOfType<CinemachineConfiner>();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Monster"), LayerMask.NameToLayer("Monster"), true);
    }

    public void Setting(GameObject _LocalPlayer)
    {
        gamePanel.gameObject.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
        LocalPlayer = _LocalPlayer;

        chinemaCamera = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
        chinemaConfiner = FindObjectOfType<CinemachineConfiner>();
        chinemaCamera.Follow = LocalPlayer.transform;
        chinemaCamera.LookAt = LocalPlayer.transform;
        bgmManager = FindObjectOfType<BGMManager>();
        bgmManager.Play(1);
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

        //맵이동
        if(isTransfer)
        {
            //플레이어, 카메라 설정
            currentMapName = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.parent.parent.gameObject.name;
            LocalPlayer.transform.position = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.transform.position;
            currentChinemaCollider = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.transform.parent.parent.gameObject.transform.Find("CMRange_" + currentMapName).gameObject.GetComponent<Collider2D>();
            chinemaConfiner.m_BoundingShape2D = currentChinemaCollider;

            //몬스터 필드일경우
            if(currentMapName == "MapA_1")
            {
                spawnMonster(currentMapName);
            }
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

    public void spawnMonster(string _MapName)
    {
        GameObject spawnPointObject = GameObject.Find(_MapName).gameObject.transform.Find("MonsterSpawnField").gameObject;
        GameObject Monsters = GameObject.Find("Monsters").gameObject;
        spawnPoint = new GameObject[spawnPointObject.transform.childCount];
        BoxCollider2D spawnCollider;
        Vector2 originPosition;
        Vector2 randomPosition;
        Vector2 respawnPosition;

        for (int i = 0; i < spawnPointObject.transform.childCount; i++)
        {
            spawnPoint[i] = spawnPointObject.transform.GetChild(i).gameObject;
            spawnCollider = spawnPoint[i].GetComponent<BoxCollider2D>();

            originPosition = spawnPoint[i].transform.position;

            float spawnPosition_x = spawnCollider.bounds.size.x;
            float spawnPosition_y = spawnCollider.bounds.size.y / 2 * -1;
            spawnPosition_x = Random.Range((spawnPosition_x / 2) * -1, (spawnPosition_x / 2));

            randomPosition = new Vector2(spawnPosition_x, spawnPosition_y);
            respawnPosition = originPosition + randomPosition;
            GameObject monster = PhotonNetwork.Instantiate("Monster0", respawnPosition, Quaternion.identity);
            monster.transform.SetParent(Monsters.transform);
        }
    }
}
