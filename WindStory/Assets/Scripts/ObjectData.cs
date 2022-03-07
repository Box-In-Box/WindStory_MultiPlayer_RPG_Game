using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    /*
     * transfer : 0 ~ 999
     * Npc : 1000 ~ 2999 +100씩
     * Monster : 3000 ~ 4999
     * item : 5000 ~ 5999 : 소비
     *        6000 ~ 6999 : 장비
     *        7000 ~ 7999 : 기타
     *        8000 ~ 8999 : 퀘스트
     *       
     * 퀘스트 : 10000 ~
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
