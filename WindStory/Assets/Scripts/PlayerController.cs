using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;

/*
 * 
 * fixed 속도가 내려갈때 가속되면 콜라이더 뚫고 내려가버림
 * 내 방향 앞에 타일이 있고 앞에밑에 타일이 있으면 rigid kinematic
 * 점프중에는 레이어 변경하면 안됨 콜라이더트리거 하나 더만들어서 (몸(상체))이안들어가있고 isground이면 착지
 * 
 */
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D rigid;
    public Animator anim;
    public PhotonView PV;
    public TransferMap theTransfer;
    public Transform groundCheckCollider;

    [Header ("---Setting---")]
    [SerializeField] [Range(1f, 10f)] float speed;
    [SerializeField] [Range(100f, 1000f)] float jumpForce;
    [SerializeField] [Range(0f, 1f)] float checkGroundDistance = 0.3f;

    [Header ("---Check---")]
    public bool isFacingRight;
    public bool isGround;
    public bool isUpGround;
    public bool doubleJumpState;
    public bool isMove;
    private bool isJump;
    
    [Header("---Sound---")]
    public string walkSound;
    public bool timeCheck = true;

    [Header("---MAP---")]
    public Collider2D currentChinemaCollider;
    public GameObject transfer;
    public string currentMapName;
    public string targetMapName;
    public bool isInTransferObj;
    public static CinemachineVirtualCamera chinemaCamera;
    public static CinemachineConfiner chinemaConfiner;

    [Header("---ETC---")]
    public string npcName;
    public int npcNum;
    public int talkIndex;
    public bool isNpcTrigger;

    private AudioManager theAudio;
    private NpcScript theNpc;

    private float axis;
    Vector3 curPos;
    int playerLayer, groundPlayerLayer, passPlayerLayer, groundLayer, passGroundLayer;

    void Awake()
    {
        if (PV.IsMine)
        {
            //2D 카메라
            chinemaCamera = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            chinemaCamera.Follow = transform;
            chinemaCamera.LookAt = transform;
            chinemaConfiner = FindObjectOfType<CinemachineConfiner>();
            chinemaConfiner.m_BoundingShape2D = currentChinemaCollider;

            //레이어 통과
            playerLayer = LayerMask.NameToLayer("Player");
            groundPlayerLayer = LayerMask.NameToLayer("GroundPlayer");
            passPlayerLayer = LayerMask.NameToLayer("PassPlayer");
            groundLayer = LayerMask.NameToLayer("Ground");
            passGroundLayer = LayerMask.NameToLayer("PassGround"); 
            Physics2D.IgnoreLayerCollision(playerLayer, playerLayer, true);
            Physics2D.IgnoreLayerCollision(playerLayer, passPlayerLayer, true);
        }
        //닉네임
        transform.Find("Canvas").gameObject.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PV.IsMine ? PhotonNetwork.LocalPlayer.NickName : PV.Owner.NickName;
    }

    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
        theNpc = FindObjectOfType<NpcScript>();
        theTransfer = FindObjectOfType<TransferMap>();
    }

    void Update()
    {
        if (PV.IsMine)
        {
            axis = Input.GetAxisRaw("Horizontal");
            if ((axis > 0 && isFacingRight == false) || (axis < 0 && isFacingRight == true))
            {
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered);
            }
            //경사면 에서 위치 고정(안미끄러짐)
            if (axis != 0)
            {
                isMove = true;
                PV.RPC("animationRPC", RpcTarget.AllBuffered, "isRun", true);
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation; //키입력 받을때 프리즈 off
            }
            else
            {
                isMove = false;
                PV.RPC("animationRPC", RpcTarget.AllBuffered, "isRun", false);
                rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; //키입력 안받을때 프리즈 on
            }

            //바닥체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position, checkGroundDistance, 1 << groundLayer);
            isUpGround = Physics2D.OverlapCircle((Vector2)transform.position, checkGroundDistance, 1 << passGroundLayer);
            Debug.DrawRay(transform.position, Vector2.down * checkGroundDistance, Color.green);

            //↑ 점프, ↑↑ 더블 점프
            if (isUpGround) isGround = true;
            if (isGround) doubleJumpState = true;

            PV.RPC("animationRPC", RpcTarget.AllBuffered, "isJump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround)
            {
                PV.RPC("jumpRPC", RpcTarget.All);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && doubleJumpState)
            {
                doubleJumpState = false;
                PV.RPC("jumpRPC", RpcTarget.All);
            }

            //↓ 레이어 아래로통과
            if (isUpGround && Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine("PassDownGround", transform.position.y);
            }
            //↓ 맵이동
            if (isInTransferObj)
            {
                targetMapName = transfer.gameObject.GetComponent<TransferMap>().targetPoint.parent.name;  //이동할 맵 표시
                if (Input.GetKeyDown(KeyCode.DownArrow)) //맵 이동
                {
                    //카메라 설정
                    chinemaConfiner.m_BoundingShape2D = currentChinemaCollider;
                    currentMapName = transfer.gameObject.GetComponent<TransferMap>().targetPoint.parent.name;

                    //플레이어 카메라 설정
                    transform.position = transfer.gameObject.GetComponent<TransferMap>().targetPoint.transform.position;
                    currentChinemaCollider = transfer.gameObject.GetComponent<TransferMap>().targetPoint.transform.parent.gameObject.transform.Find("CMRange_" + currentMapName).gameObject.GetComponent<Collider2D>();
                    chinemaConfiner.m_BoundingShape2D = currentChinemaCollider;
                }
            }

            //사운드
            if (isMove)
            {
                if (!theAudio.IsPlaying(walkSound) && isGround && timeCheck)
                {
                    timeCheck = false;
                    theAudio.Play(walkSound);
                    StartCoroutine("CountTime", 0.5f);
                }
            }
            else theAudio.Stop(walkSound);
            //npc 대화
            if (isNpcTrigger)
            {
                if (!theNpc.IsTalk(npcName) && Input.GetKeyDown(KeyCode.Space))
                {
                    theNpc.Talk(npcNum, talkIndex);
                }

                else if (theNpc.IsTalk(npcName) && Input.GetKeyDown(KeyCode.Space))
                {
                    theNpc.TalkCancel(npcName);
                }
            }
        }
        //IsMine이 아닌 것들
        //움직임
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(this.gameObject.transform.position, curPos, Time.deltaTime * 10);
    }

    void Jump()
    {
        isJump = true;
        if (PV.IsMine)
        {
            //↑ 레이어 위로통과
            StopAllCoroutines();
            StartCoroutine("PassUpGround");
        }
    }

    void FixedUpdate()
    {
        //← → 이동
        rigid.velocity = new Vector2(speed * axis, rigid.velocity.y);
        if (isJump)
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpForce);
            isJump = false;
        }

    }

    [PunRPC]    //좌우반전
    void FlipXRPC()
    {
        isFacingRight = !isFacingRight;
        Vector3 playerScale = transform.GetChild(0).transform.localScale;
        playerScale.x = playerScale.x * -1;
        transform.GetChild(0).transform.localScale = playerScale;
    }
    [PunRPC]    //애니메이션
    void animationRPC(string param, bool isRuning)
    {
        anim.SetBool(param, isRuning);
    }
    [PunRPC]    //점프
    void jumpRPC()
    {
        Jump();
    }
    [PunRPC]    //사운드
    void soundRPC(string _soundName)
    {
        theAudio.Play(_soundName);
    }

    //↓ 레이어 아래로통과 코루틴
    IEnumerator PassDownGround(float curY)
    {
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, true);
        gameObject.layer = passPlayerLayer;
        yield return new WaitUntil(() => curY - transform.position.y > 0.3f);
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, false);
        gameObject.layer = playerLayer;
    }
    //↑ 레이어 위로통과 코루틴
    IEnumerator PassUpGround()
    {
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, true);
        gameObject.layer = passPlayerLayer;
        yield return new WaitUntil(() => (rigid.velocity.y < 0 && !isGround));
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, false);
        gameObject.layer = playerLayer;
    }

    //워크카운터 코루틴
    IEnumerator CountTime(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        timeCheck = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //대화창 온 트리거
        if (col.gameObject.tag == "NPC")
        {
            npcName = col.gameObject.name;
            npcNum = col.gameObject.GetComponent<ObjectData>().id;
            isNpcTrigger = true;
        }
        //맵이동 온 트리거
        else if (col.gameObject.tag == "Transfer")
        {
            transfer = col.gameObject;
            isInTransferObj = true;
        }
        //처음 맵 확인용
        else if (col.gameObject.tag == "MapRange")
        {
            currentMapName = col.gameObject.transform.parent.name;
            currentChinemaCollider = col.gameObject.GetComponent<PolygonCollider2D>();
            chinemaConfiner.m_BoundingShape2D = currentChinemaCollider;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //대화창 오프 트리거
        if (col.gameObject.tag == "NPC")
        {
            npcName = "";
            npcNum = -1;
            isNpcTrigger = false;
        }
        //맵이동 오프 트리거
        else if (col.gameObject.tag == "Transfer")
        {
            transfer = null;
            isInTransferObj = false;
            targetMapName = "";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }
}