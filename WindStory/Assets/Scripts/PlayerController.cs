using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;


public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D rigid;
    public Animator anim;
    public PhotonView PV;

    [Header ("---Setting---")]
    [SerializeField] [Range(1f, 10f)] float speed = 5f;
    [SerializeField] [Range(100f, 500f)] float jumpForce = 100f;

    [Header ("---Check---")]
    public bool isFacingRight = true;
    public bool isUpGround; //체크
    public bool isGround;
    public bool doubleJumpState;
    public bool isMove;

    [Header("---Sound---")]
    public string walkSound;
    public bool timeCheck = true;

    [Header("---ETC---")]
    public string npcName;
    public int npcNum;
    public int talkIndex;
    public bool isNpcTrigger;

    private AudioManager theAudio;
    private NpcScript theNpc;

    Vector3 curPos;

    int playerLayer, passplayerLayer, passgroundLayer;   //레이어통과

    void Awake()
    {
        if (PV.IsMine)
        {
            //2D 카메라
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;

            //레이어 통과
            playerLayer = LayerMask.NameToLayer("Player");
            passgroundLayer = LayerMask.NameToLayer("PassGround");
            passplayerLayer = LayerMask.NameToLayer("PassPlayer");  
        }
        //닉네임
        transform.Find("Canvas").gameObject.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PV.IsMine ? PhotonNetwork.LocalPlayer.NickName : PV.Owner.NickName;

    }

    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
        theNpc = FindObjectOfType<NpcScript>();
    }

    void Update()
    {
        if (PV.IsMine)
        {
            //← → 이동
            float axis = Input.GetAxisRaw("Horizontal");
            rigid.velocity = new Vector2(speed * axis, rigid.velocity.y);

            if ((axis > 0 && isFacingRight == false) || (axis < 0 && isFacingRight == true))
            {
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered);
            }
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
            isGround = Physics2D.OverlapCircle((Vector2)transform.position, 0.3f, 1 << LayerMask.NameToLayer("Ground"));
            isUpGround = Physics2D.OverlapCircle((Vector2)transform.position, 0.3f, 1 << LayerMask.NameToLayer("PassGround"));
            if (isUpGround) isGround = true;
            Debug.DrawRay(transform.position, new Vector2(0, 0.3f), Color.red);

            //↑ 점프, ↑↑ 더블 점프
            if (isGround) doubleJumpState = true;
            PV.RPC("animationRPC", RpcTarget.AllBuffered, "isJump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround) PV.RPC("jumpRPC", RpcTarget.All);
            else if (Input.GetKeyDown(KeyCode.UpArrow) && doubleJumpState)
            {
                doubleJumpState = false;
                PV.RPC("jumpRPC", RpcTarget.All);
            }

            //레이어 위로통과
            if (rigid.velocity.y > 0)
                Physics2D.IgnoreLayerCollision(playerLayer, passgroundLayer, true);
            else
                Physics2D.IgnoreLayerCollision(playerLayer, passgroundLayer, false);

            //레이어 아래로통과
            if (isUpGround && Input.GetKeyDown(KeyCode.DownArrow))
            {
                float curY = transform.position.y;
                StartCoroutine("PassGround", curY);
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
        else transform.position = Vector3.Lerp(this.gameObject.transform.position, curPos, Time.deltaTime * 10);   //부드러운 움직임 구현 필요 버그 발생!!!
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
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * jumpForce);
    }
    [PunRPC]    //사운드
    void soundRPC(string _soundName)
    {
        theAudio.Play(_soundName);
    }

    //통과 코루틴
    IEnumerator PassGround(float curY)
    {
        Physics2D.IgnoreLayerCollision(passplayerLayer, passgroundLayer, true);
        gameObject.layer = LayerMask.NameToLayer("PassPlayer");
        yield return new WaitUntil(() => curY - transform.position.y > 0.2f);
        Physics2D.IgnoreLayerCollision(passplayerLayer, passgroundLayer, false);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    //워크카운터 코루틴
    IEnumerator CountTime(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        timeCheck = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //바닥체크
        if (col.gameObject.tag == "Ground") isGround = true;
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
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //대화창 오프 트리거
        if(col.gameObject.tag == "NPC")
        {
            npcName = "";
            npcNum = -1;
            isNpcTrigger = false;
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