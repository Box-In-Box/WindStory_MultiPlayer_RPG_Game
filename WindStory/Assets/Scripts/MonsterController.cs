using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MonsterController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D rigid;
    public Animator anim;
    public PhotonView PV;

    [SerializeField] [Range(0.5f, 5f)] float speed;
    public int moveFlag;    // -1 : left, 0 : idle, 1 : right

    private void Awake()
    {
        if(PV.IsMine)
        {
            Invoke("Think", 1f);
        }
    }

    private void FixedUpdate()
    {
        //flipX
        if (moveFlag != 0)
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = moveFlag == 1;

        //Transform
        if (PV.IsMine)
        {
            rigid.velocity = new Vector2(moveFlag * speed, rigid.velocity.y);
        }

        Vector2 frontVec = new Vector2(rigid.position.x + (moveFlag * 0.5f), rigid.position.y + 0.3f);

        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));

        RaycastHit2D raycast0 = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Ground"));
        RaycastHit2D raycast1 = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("PassGround"));
        if (raycast0.collider == null && raycast1.collider == null)
        {
            moveFlag *= -1;
            CancelInvoke();
            Invoke("Think", 2f);
        }

    }

    //AI
    void Think()
    {
        moveFlag = Random.Range(-1, 2);
        PV.RPC("flipX", RpcTarget.AllBuffered, moveFlag);

        if (moveFlag == 0)
            PV.RPC("animationRPC", RpcTarget.All, "isWalk", false);
        else
            PV.RPC("animationRPC", RpcTarget.All, "isWalk", true);

        Invoke("Think", Random.Range(2f, 5f));
    }

    [PunRPC]
    void flipX(int _moveFlag)
    {
        moveFlag = _moveFlag;
    }

    [PunRPC]
    void animationRPC(string param, bool isRuning)
    {
        anim.SetBool(param, isRuning);
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
