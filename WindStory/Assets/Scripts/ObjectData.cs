using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    //0~999 : object 
    /*
     * transfer - 0
     */
    //1000 ~ 2000 : npc
    public int id;
    public bool isNpc;
    public bool isTransfer;

    private void Start()
    {
        if (id >= 0 && id < 100) isTransfer = true;
        else if (id >= 1000 && id < 2000) isNpc = true;
    }

    public Transform targetPoint;
    public Transform startPoint;
}
