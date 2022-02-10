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

    Vector3 curPos;

    int playerLayer, groundLayer;   //레이어통과

    void Awake()
    {
        if (PV.IsMine)
        {
            //2D 카메라
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }
        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("PassGround");
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
                PV.RPC("animationRPC", RpcTarget.AllBuffered, true);
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation; //키입력 받을때 프리즈 off
            }
            else
            {
                PV.RPC("animationRPC", RpcTarget.AllBuffered, false);
                rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; //키입력 안받을때 프리즈 on
            }

            //바닥체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position, 0.3f, 1 << LayerMask.NameToLayer("Ground"));
            isUpGround = Physics2D.OverlapCircle((Vector2)transform.position, 0.3f, 1 << LayerMask.NameToLayer("PassGround"));
            if (isUpGround) isGround = true;
            Debug.DrawRay(transform.position, new Vector2(0, 0.3f), Color.red);

            //↑ ↑ 더블 점프
            if (isGround) doubleJumpState = true;
            anim.SetBool("isJump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround) PV.RPC("JumpRPC", RpcTarget.All);
            else if(Input.GetKeyDown(KeyCode.UpArrow) && doubleJumpState) 
            {
                doubleJumpState = false;
                PV.RPC("JumpRPC", RpcTarget.All);
            }
            //레이어 위로통과
            if (rigid.velocity.y > 0)
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
            else
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);

            //레이어 아래로통과
            if (isUpGround && Input.GetKeyDown(KeyCode.DownArrow))
            {
                float curY = transform.position.y;
                StopAllCoroutines();
                StartCoroutine("PassGround", curY);
            }

        }
        //IsMine이 아닌 것들
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = curPos;   //부드러운 움직임 구현 필요 버그 발생!!!
    }

    [PunRPC]
    void FlipXRPC()
    {
        isFacingRight = !isFacingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x = playerScale.x * -1;
        transform.localScale = playerScale;
    }
    [PunRPC]
    void animationRPC(bool isRuning)
    {
        anim.SetBool("isRun", isRuning);
    }
    [PunRPC]
    void JumpRPC()
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * jumpForce);
    }

    IEnumerator PassGround(float curY)
    {
        gameObject.layer = LayerMask.NameToLayer("PassPlayer");
        yield return new WaitUntil(() => curY - transform.position.y > 0.2f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground") isGround = true;
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