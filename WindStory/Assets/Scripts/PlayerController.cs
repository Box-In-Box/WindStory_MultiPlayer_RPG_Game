using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;

/*
 * 
 * 점프 후 땅에 떨어지면서 방향키 바꾸면 점프애니메이션 나감 /오류
 * 
 */
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D rigid;
    public Animator anim;
    public PhotonView PV;
    public GameManager gameManager;
    public ObjectData objectData;

    [Header ("---Setting---")]
    [SerializeField] [Range(1f, 10f)] float speed;
    [SerializeField] [Range(100f, 1000f)] float jumpForce;
    [SerializeField] [Range(0f, 1f)] float checkGroundDistance;

    [Header("---Check---")]
    public bool spawnCheck;
    public bool isFacingRight;
    public bool isGround;
    public bool isUpGround;
    public bool doubleJumpState;
    public bool isMove;
    public bool isJump;
    public bool canDoubleJump;

    [Header("---Sound---")]
    public string walkSound;
    private bool timeCheck = true;

    [Header("---MAP---")]
    public bool isInTransferObj;
    public GameObject transfer;

    [Header("---ETC---")]
    public GameObject scanObj;
    public bool isNpcTrigger;

    private AudioManager audioManager;

    private float axis;
    Vector3 curPos;
    int playerLayer, passPlayerLayer, groundLayer, passGroundLayer;

    void Awake()
    {
        if (PV.IsMine)
        {
            //레이어 통과
            playerLayer = LayerMask.NameToLayer("Player");
            passPlayerLayer = LayerMask.NameToLayer("PassPlayer");
            groundLayer = LayerMask.NameToLayer("Ground");
            passGroundLayer = LayerMask.NameToLayer("PassGround"); 
            Physics2D.IgnoreLayerCollision(playerLayer, playerLayer, true);
            Physics2D.IgnoreLayerCollision(playerLayer, passPlayerLayer, true);
            Physics2D.IgnoreLayerCollision(playerLayer, passPlayerLayer, true);
        }
        //닉네임
        transform.Find("Canvas").gameObject.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PV.IsMine ? PhotonNetwork.LocalPlayer.NickName : PV.Owner.NickName;
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        gameManager = FindObjectOfType<GameManager>();
        //게임메니저 셋팅
        if (PV.IsMine) gameManager.Setting(gameObject);
    }

    void Update()
    {
        if (PV.IsMine)
        {
            if (rigid.velocity != Vector2.zero) isMove = true;
            else isMove = false;

            axis = Input.GetAxisRaw("Horizontal");

            //달리기 anim
            if ((axis > 0 && isFacingRight == false) || (axis < 0 && isFacingRight == true)) PV.RPC("FlipXRPC", RpcTarget.AllBuffered);
            //경사면 에서 위치 고정(안미끄러짐)
            if (axis != 0)
            {
                PV.RPC("animationRPC", RpcTarget.AllBuffered, "isRun", true);
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation; //키입력 받을때 프리즈 off
            }
            else
            {
                PV.RPC("animationRPC", RpcTarget.AllBuffered, "isRun", false);
                rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; //키입력 안받을때 프리즈 on
            }

            //바닥체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position, checkGroundDistance, LayerMask.GetMask("Ground"));
            isUpGround = Physics2D.OverlapCircle((Vector2)transform.position, checkGroundDistance, LayerMask.GetMask("PassGround"));
            if (isUpGround) isGround = true;
            if (isGround) doubleJumpState = true;
            Debug.DrawRay(transform.position, Vector2.down * checkGroundDistance, Color.green);
        
            //점프 anim
            if (!isGround || isMove && gameObject.layer == passPlayerLayer) PV.RPC("animationRPC", RpcTarget.AllBuffered, "isJump", true);
            else PV.RPC("animationRPC", RpcTarget.AllBuffered, "isJump", false);

            //↑ 점프
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround) PV.RPC("jumpRPC", RpcTarget.All);
            //↑↑ 더블 점프
            else if (Input.GetKeyDown(KeyCode.UpArrow) && doubleJumpState && canDoubleJump) 
            { 
                doubleJumpState = false; 
                PV.RPC("jumpRPC", RpcTarget.All);
            }

            //↓ 레이어 아래로통과
            if (isUpGround && Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine("PassDownGroundCoroutine", transform.position.y);
            }
            //↓ 맵이동
            if (isInTransferObj)
            {
                gameManager.targetMapName = transfer.gameObject.GetComponent<ObjectData>().targetPoint.parent.parent.name;  //이동할 맵 표시
                if (Input.GetKeyDown(KeyCode.DownArrow)) gameManager.Action(transfer); //맵 이동
            }
            //사운드
            if (isMove)
            {
                if (!audioManager.IsPlaying(walkSound) && isGround && timeCheck)
                {
                    timeCheck = false;
                    audioManager.Play(walkSound);
                    StartCoroutine("CountTimeCoroutine", 0.5f);
                }
            }
            else audioManager.Stop(walkSound);
            //npc 대화
            if (isNpcTrigger)
            {
                if (Input.GetKeyDown(KeyCode.Space)) gameManager.Action(scanObj);
            }
        }
        /*IsMine이 아닌 것들*/
        //움직임
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(this.gameObject.transform.position, curPos, Time.deltaTime * 10);
    }

    void Jump()
    {
        isJump = true;
        //↑ 레이어 위로통과
        if (PV.IsMine) StartCoroutine("PassUpGroundCoroutine");
    }

    void FixedUpdate()
    {
        //← → 이동
        rigid.velocity = new Vector2(speed * axis, rigid.velocity.y);

        //↑점프 OR ↑↑ 더블 점프
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
    void jumpRPC() => Jump();

    [PunRPC]    //사운드
    void soundRPC(string _soundName)
    {
        audioManager.Play(_soundName);
    }

    //↓ 레이어 아래로통과 코루틴
    IEnumerator PassDownGroundCoroutine(float curY)
    {
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, true);
        gameObject.layer = passPlayerLayer;
        yield return new WaitUntil(() => curY - transform.position.y > 0.3f);
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, false);
        gameObject.layer = playerLayer;
    }
    //↑ 레이어 위로통과 코루틴
    IEnumerator PassUpGroundCoroutine()
    {
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, true);
        gameObject.layer = passPlayerLayer;
        //if(rigid.velocity.y > )
        yield return new WaitUntil(() => (rigid.velocity.y > 0));
        yield return new WaitUntil(() => (isGround && rigid.velocity.y < 0));
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, false);
        gameObject.layer = playerLayer;
    }

    //워크카운터 코루틴
    IEnumerator CountTimeCoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        timeCheck = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //대화창 온 트리거
        if (collision.gameObject.tag == "NPC")
        {
            scanObj = collision.gameObject;
            isNpcTrigger = true;
        }
        //맵이동 온 트리거
        else if (collision.gameObject.tag == "Transfer")
        {
            transfer = collision.gameObject;
            isInTransferObj = true;
        }
        //처음 맵 확인용
        else if (!spawnCheck && collision.gameObject.tag == "MapRange")
        {
            if(gameManager == null) gameManager = FindObjectOfType<GameManager>();
            gameManager.SetMapField(collision.gameObject.transform.parent.gameObject.name, collision.gameObject.GetComponent<PolygonCollider2D>());
            spawnCheck = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //대화창 오프 트리거
        if (collision.gameObject.tag == "NPC")
        {
            scanObj = null;
            isNpcTrigger = false;
        }
        //맵이동 오프 트리거
        else if (collision.gameObject.tag == "Transfer")
        {
            transfer = null;
            isInTransferObj = false;
            gameManager.targetMapName = "";
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