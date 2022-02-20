using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LendingEffect : MonoBehaviour
{
    public PhotonView PV;

    void Start() => Destroy(gameObject, 0.25f);
}
