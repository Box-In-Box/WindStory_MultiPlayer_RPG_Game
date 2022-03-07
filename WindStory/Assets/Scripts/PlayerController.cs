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
    private Rigidbody2D rigid;
    private Animator anim;
    private PhotonView PV;
    private GameManager gameManager;

    [Header ("---Setting---")]
    [SerializeField] [Range(1f, 10f)] float speed;
    [SerializeField] [Range(100f, 1000f)] float jumpForce;
    [SerializeField] [Range(0f, 1f)] float checkGroundDistance;
    [SerializeField] public int level = 1;
    [SerializeField] public int hp = 100;
    [SerializeField] public int mp = 100;
    [SerializeField] public int exp = 0;

    [Header("---Check---")]
    public bool canDoubleJump;
    private bool spawnCheck;
    private bool isFacingRight = true;
    private bool isGround;
    private bool isUpGround;
    private bool doubleJumpState;
    private bool isMove;
    private bool isJump;

    [Header("---Sound---")]
    public string walkSound;
    private bool timeCheck = true;

    [Header("---ETC---")]
    public GameObject scanObj;
    public bool isNpcTrigger;
    public bool isInTransferObj;
    private float PreviouslandPositionY;

    private AudioManager audioManager;

    private float axis;
    int playerLayer, passPlayerLayer, passGroundLayer;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        gameManager = FindObjectOfType<GameManager>();

        if (PV.IsMine)
        {
            //레이어 통과
            playerLayer = LayerMask.NameToLayer("Player");
            passPlayerLayer = LayerMask.NameToLayer("PassPlayer");
            passGroundLayer = LayerMask.NameToLayer("PassGround"); 
            Physics2D.IgnoreLayerCollision(playerLayer, playerLayer, true);
            Physics2D.IgnoreLayerCollision(playerLayer, passPlayerLayer, true);
            Physics2D.IgnoreLayerCollision(playerLayer, passPlayerLayer, true);
        }
        else
        {
            gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
            gameObject.GetComponent<Rigidbody2D>().simulated = false;
        }
            
        //닉네임
        transform.Find("PlayerCanvas").gameObject.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PV.IsMine ? PhotonNetwork.LocalPlayer.NickName : PV.Owner.NickName;
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        gameManager = FindObjectOfType<GameManager>();
        if (PV.IsMine)
        {
            //게임메니저 셋팅
            gameManager.Setting(gameObject);
            //초기값 셋팅
            PreviouslandPositionY = transform.position.y;
        }
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
            if (isGround)
            {
                doubleJumpState = true;
            } 
                
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
            if (isUpGround && Input.GetKeyDown(KeyCode.DownArrow) && !isInTransferObj)
            {
                audioManager.Play("Jump");
                StartCoroutine("PassDownGroundCoroutine", transform.position.y);
            }
            //↓ 맵이동
            if (isInTransferObj)
            {
                gameManager.targetMapName = scanObj.gameObject.GetComponent<ObjectData>().targetPoint.parent.parent.name;  //이동할 맵 표시
                if (Input.GetKeyDown(KeyCode.DownArrow)) gameManager.Action(scanObj); //맵 이동
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
            PreviouslandPositionY = transform.position.y > PreviouslandPositionY ? transform.position.y : PreviouslandPositionY;
        }
    }

    void Jump()
    {
        isJump = true;
        audioManager.Play("Jump");
        //↑ 레이어 위로통과
        if (PV.IsMine) StartCoroutine("PassUpGroundCoroutine");
    }

    void land()
    {
        //점프했을 때 최고 위치 - 내려와 땅에 닿았을 때 위치 가 0.3이상이면 랜드이펙트 발생
        if(PreviouslandPositionY - transform.position.y > 0.3f)
        {
            audioManager.Play("Land");
            GameObject effect = PhotonNetwork.Instantiate("lendingEffect", transform.position, Quaternion.identity);
            effect.transform.SetParent(GameObject.Find("Effect").transform);
        }
    }

    void FixedUpdate()
    {
        if(PV.IsMine)
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
        yield return new WaitUntil(() => (rigid.velocity.y < 0));
        Physics2D.IgnoreLayerCollision(passPlayerLayer, passGroundLayer, false);
        gameObject.layer = playerLayer;
    }

    //워크카운터 코루틴
    IEnumerator CountTimeCoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        timeCheck = true;
    }

    IEnumerator WaitTimeCoroutine(float delayTime)
    {
        rigid.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(delayTime);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //landEffect
        if (collision.gameObject.tag == "Ground")
        {
            land();
            PreviouslandPositionY = transform.position.y;
        }
        if (collision.gameObject.tag == "BorderLine")
        {
            land();
            PreviouslandPositionY = transform.position.y;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //landEffect 크로스(내리막, 바닥) 되는 부분 수정값
        if (collision.gameObject.tag == "Ground") PreviouslandPositionY = transform.position.y;
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
            scanObj = collision.gameObject;
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
            scanObj = null;
            isInTransferObj = false;
            gameManager.targetMapName = "";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            
        }
        else
        {
            
        }
    }
}