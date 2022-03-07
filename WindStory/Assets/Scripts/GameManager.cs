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
    public QuestManager questManager;
    public TypingEffect typingEffect;

    public GameObject gamePanel;
    private GameObject inventoryCase;
    private GameObject questLog;

    [Header("---Default---")]
    private GameObject scanObj;
    private string objectName;
    private int id;
    private bool isNpc;
    private bool isTransfer;

    [Header("---Npc---")]
    private string talkData;
    private int TalkIndex;
    private int questIndex;
    private int questCondition;
    private bool isTalking;

    [Header("---Transfer---")]
    public string currentMapName;
    public string targetMapName;
    private Collider2D currentChinemaCollider;
    private static CinemachineVirtualCamera chinemaCamera;
    private static CinemachineConfiner chinemaConfiner;

    [Header("---Monster---")]
    public GameObject Monsters; //���Ͱ� ��ȯ�Ǵ� ���� �ֻ��� �θ� ������Ʈ
    private GameObject monsters;    //�ʺ��� ���Ͱ� ��ȯ�Ǵ� ���� ������Ʈ
    public GameObject[] spawnPoint;
    public bool[] isSpawnCoroutineRunning;

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);

        chinemaConfiner = FindObjectOfType<CinemachineConfiner>();
        inventoryCase = GameObject.Find("Canvas").transform.Find("Inventory").transform.Find("InventoryCase").gameObject;
        questLog = GameObject.Find("Canvas").transform.Find("Quest").transform.Find("QuestLog").gameObject;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Monster"), LayerMask.NameToLayer("Monster"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monster"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PassPlayer"), LayerMask.NameToLayer("Monster"), true);
    }

    public void Setting(GameObject _LocalPlayer)
    {
        gamePanel.gameObject.transform.Find("StatusPanel").transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
        LocalPlayer = _LocalPlayer;
        questManager.Setting(_LocalPlayer);

        isSpawnCoroutineRunning = new bool[Monsters.transform.childCount];

        //���� ó�� ���۽� �ó׸��� �� ����
        chinemaCamera = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
        chinemaConfiner = FindObjectOfType<CinemachineConfiner>();
        chinemaCamera.Follow = _LocalPlayer.transform;
        chinemaCamera.LookAt = _LocalPlayer.transform;

        //bgm ���� �� ���� *** DB ������ ���� �ʿ�
        bgmManager = FindObjectOfType<BGMManager>();
        bgmManager.Play(1);
    }

    //�� �̵��� ���� �� �� ����
    public void SetMapField(string _currentMapName, Collider2D _currentChinemaCollider)
    {
        currentMapName = _currentMapName;
        currentChinemaCollider = _currentChinemaCollider;
        chinemaConfiner.m_BoundingShape2D = _currentChinemaCollider;
    }

    public void Action(GameObject _scanObj)
    {
        //���� ������Ʈ�̸� �ѹ��� ��ĵ
        if(scanObj == null || scanObj.GetComponent<ObjectData>().id != _scanObj.GetComponent<ObjectData>().id)
        {
            //���� ������Ʈ �� ����
            if(npcManager != null) npcManager.ChatDown(); //���� ê ����
            scanObj = _scanObj;
            objectName = scanObj.name;
            id = scanObj.GetComponent<ObjectData>().id;
            isNpc = scanObj.GetComponent<ObjectData>().isNpc;
            isTransfer = scanObj.GetComponent<ObjectData>().isTransfer;

            //Npc����
            if (isNpc)
            {
                npcManager = scanObj.transform.parent.gameObject.GetComponent<NpcManager>();
                TalkIndex = 0;
                isTalking = false;
            } 
        }

        //Npc ��ȭ�׼�
        if(isNpc)
        {
            //Npc����Ʈ�ε��� ���� ***����� ������ ���ʴ�� �ִ� ���� �ҷ��� ����Ʈ ���� ���߿� npc�� ����Ʈ�� �������ٸ� ���� �ʿ�***
            if (scanObj.GetComponent<NPCData>().NPCQuest.Length != 0)
            {
                for (int i = 0; i < scanObj.GetComponent<NPCData>().NPCQuest.Length; i++)
                {
                    if (scanObj.GetComponent<NPCData>().isNPCQuestClear[i] == false)    //����Ÿ�� ���� ��
                    {
                        questIndex = questManager.GetQuest(scanObj.GetComponent<NPCData>().NPCQuest[i]);
                        questCondition = questManager.GetQuestCondition(scanObj.GetComponent<NPCData>().NPCQuest[i]);
                    }
                    else    //����Ʈ�� ������ Ŭ���� �Ǿ��� ��
                    {
                        questIndex = 0;
                        questCondition = 0;
                    }
                }
            }
            else    //����Ʈ�� ���� ��
            {
                questIndex = 0;
                questCondition = 0;
            } 

            if (!isTalking)
            {
                if (!npcManager.IsTalk(objectName)) npcManager.ChatUp(id);
                Talk(id);
            }
            else if (isTalking) Talk(id);
        }

        //���̵�
        if(isTransfer)
        {
            //�÷��̾�, ī�޶� ����
            currentMapName = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.parent.parent.gameObject.name;
            LocalPlayer.transform.position = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.transform.position;
            currentChinemaCollider = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.transform.parent.parent.gameObject.transform.Find("CMRange_" + currentMapName).gameObject.GetComponent<Collider2D>();
            chinemaConfiner.m_BoundingShape2D = currentChinemaCollider;

            //���� �ʵ��ϰ��
            if(currentMapName == "MapA_1")
            {
                for(int i = 0; i < Monsters.transform.childCount; i++)
                    if(isSpawnCoroutineRunning[i] == false && Monsters.transform.GetChild(i).name == "MapA_1") StartCoroutine("spawnMonsterCoroutine"); 
            }
        }
    }

    public void Talk(int _npcId)
    {
        if (typingEffect.isPlayingTypingEffect) { typingEffect.SetMsg(npcManager.curText.GetComponent<Text>(), ""); return; }
        else talkData = talkManager.GetTalk(_npcId + questIndex + questCondition, TalkIndex);
        
        if (talkData == null)
        {
            TalkIndex = 0;
            isTalking = false;
            npcManager.ChatDown();
            questManager.CheckQuest(questIndex, scanObj);
            return;
        }
        //npcManager.curText.text = talkData;
        typingEffect.SetMsg(npcManager.curText.GetComponent<Text>(), talkData);

        isTalking = true;
        TalkIndex++;
    }

    public void spawnMonster(string _MapName)
    {
        for (int i = 0; i < Monsters.transform.childCount; i++)
        {
            //���� ���� ���Ͱ� �����Ǵ� ������ Ȯ��
            if (Monsters.transform.GetChild(i).gameObject.name == _MapName)
            {
                monsters = Monsters.transform.GetChild(i).gameObject;   //���Ͱ� ��ȯ�� �� ������Ʈ ����
                isSpawnCoroutineRunning[i] = true;                      //�� ���� ���� ���� �ڷ�ƾ 1���� ������ ���� bool
            }
        }
        GameObject spawnPointObject = monsters.transform.GetChild(0).gameObject;    //�� ���� �����Ǵ� ��ġ��

        spawnPoint = new GameObject[spawnPointObject.transform.childCount];
        BoxCollider2D spawnCollider;
        Vector2 originPosition;
        Vector2 randomPosition;
        Vector2 respawnPosition;

        //������ġ�� �ݶ��̴� �ȿ��� �������� ������ġ ����
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
            monster.transform.SetParent(monsters.transform);
        }
    }

    IEnumerator spawnMonsterCoroutine()
    {
        while(true)
        {  
            //���� ���� �ֱ� ���� �� �ִ� ���� �� ����
            spawnMonster("MapA_1");
            yield return new WaitUntil(() => monsters.transform.childCount < 12);
            yield return new WaitForSeconds(15f);
        }
    }

    private void Update()
    {
        //�κ��丮
        if (Input.GetKeyDown(KeyCode.I)) ObjectActive(KeyCode.I);
        
        if (Input.GetKeyDown(KeyCode.Q)) ObjectActive(KeyCode.Q);
    }

    public void ObjectActive(KeyCode _key)
    {
        GameObject go = null;
        switch(_key)
        {
            case KeyCode.I:
                go = inventoryCase.gameObject;
                break;
            case KeyCode.Q:
                go = questLog.gameObject;
                break;
        }
        if (!go.activeSelf) go.SetActive(true);    //������Ʈ�� �� ���� ������ ����
        else if (go.activeSelf) go.SetActive(false);   //������Ʈ�� ���������� �ݱ�
    }
}
