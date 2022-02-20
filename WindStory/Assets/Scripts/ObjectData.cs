using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    /*
     * transfer : 0 ~ 999
     * Npc : 1000 ~ 2999
     * Monster : 3000 ~ 4999
     */

    [Header("---ID---")]
    public int id;

    [Header("---Type---")]
    public bool isNpc;
    public bool isTransfer;
    public bool isMonster;

    [Header("---Transfer---")]
    public Transform targetPoint;
    public Transform startPoint;
}
