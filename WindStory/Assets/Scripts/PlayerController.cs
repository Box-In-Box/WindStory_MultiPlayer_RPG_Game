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
    public bool isUpGround; //üũ
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

    int playerLayer, passplayerLayer, passgroundLayer;   //���̾����

    void Awake()
    {
        if (PV.IsMine)
        {
            //2D ī�޶�
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;

            //���̾� ���
            playerLayer = LayerMask.NameToLayer("Player");
            passgroundLayer = LayerMask.NameToLayer("PassGround");
            passplayerLayer = LayerMask.NameToLayer("PassPlayer");  
        }
        //�г���
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
            //�� �� �̵�
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
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation; //Ű�Է� ������ ������ off
            }
            else
            {
                isMove = false;
                PV.RPC("animationRPC", RpcTarget.AllBuffered, "isRun", false);
                rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; //Ű�Է� �ȹ����� ������ on
            }

            //�ٴ�üũ
            isGround = Physics2D.OverlapCircle((Vector2)transform.position, 0.3f, 1 << LayerMask.NameToLayer("Ground"));
            isUpGround = Physics2D.OverlapCircle((Vector2)transform.position, 0.3f, 1 << LayerMask.NameToLayer("PassGround"));
            if (isUpGround) isGround = true;
            Debug.DrawRay(transform.position, new Vector2(0, 0.3f), Color.red);

            //�� ����, ��� ���� ����
            if (isGround) doubleJumpState = true;
            PV.RPC("animationRPC", RpcTarget.AllBuffered, "isJump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround) PV.RPC("jumpRPC", RpcTarget.All);
            else if (Input.GetKeyDown(KeyCode.UpArrow) && doubleJumpState)
            {
                doubleJumpState = false;
                PV.RPC("jumpRPC", RpcTarget.All);
            }

            //���̾� �������
            if (rigid.velocity.y > 0)
                Physics2D.IgnoreLayerCollision(playerLayer, passgroundLayer, true);
            else
                Physics2D.IgnoreLayerCollision(playerLayer, passgroundLayer, false);

            //���̾� �Ʒ������
            if (isUpGround && Input.GetKeyDown(KeyCode.DownArrow))
            {
                float curY = transform.position.y;
                StartCoroutine("PassGround", curY);
            }

            //����
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
            //npc ��ȭ
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
        //IsMine�� �ƴ� �͵�
        //������
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(this.gameObject.transform.position, curPos, Time.deltaTime * 10);   //�ε巯�� ������ ���� �ʿ� ���� �߻�!!!
    }

    [PunRPC]    //�¿����
    void FlipXRPC()
    {
        isFacingRight = !isFacingRight;
        Vector3 playerScale = transform.GetChild(0).transform.localScale;
        playerScale.x = playerScale.x * -1;
        transform.GetChild(0).transform.localScale = playerScale;
    }
    [PunRPC]    //�ִϸ��̼�
    void animationRPC(string param, bool isRuning)
    {
        anim.SetBool(param, isRuning);
    }
    [PunRPC]    //����
    void jumpRPC()
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * jumpForce);
    }
    [PunRPC]    //����
    void soundRPC(string _soundName)
    {
        theAudio.Play(_soundName);
    }

    //��� �ڷ�ƾ
    IEnumerator PassGround(float curY)
    {
        Physics2D.IgnoreLayerCollision(passplayerLayer, passgroundLayer, true);
        gameObject.layer = LayerMask.NameToLayer("PassPlayer");
        yield return new WaitUntil(() => curY - transform.position.y > 0.2f);
        Physics2D.IgnoreLayerCollision(passplayerLayer, passgroundLayer, false);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    //��ũī���� �ڷ�ƾ
    IEnumerator CountTime(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        timeCheck = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //�ٴ�üũ
        if (col.gameObject.tag == "Ground") isGround = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //��ȭâ �� Ʈ����
        if (col.gameObject.tag == "NPC")
        {
            npcName = col.gameObject.name;
            npcNum = col.gameObject.GetComponent<ObjectData>().id;
            isNpcTrigger = true;
        } 
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //��ȭâ ���� Ʈ����
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