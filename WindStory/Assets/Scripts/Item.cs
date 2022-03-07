using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{ 
    /*
     * 물약류 : 10000 ~ 19999
     * 장비류 : 20000 ~ 29999
     * 기타류 : 30000 ~ 39999
     * 퀘스트류 : 40000 ~ 49999
     */
    public int itemID;  //아이템 고유 ID. 중복 불가능
    public string itemName; //아이템 이름. 중복 가능 
    public string itemDescription;  //아이템 설명
    public ItemType itemType;
    public int itemCount;   //소지 개수
    public int attack; //공격력
    public int defense; //방어력

    public enum ItemType
    {
        Equipment,
        Consumable,
        Quest,
        ETC
    }

    public Item(int _itemID, string _itemName, string _itemDescription, ItemType _itemType, int _itemCount = 1, int _attack = 0, int _defense = 0)
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDescription;
        itemType = _itemType;
        itemCount = _itemCount;
        attack = _attack;
        defense = _defense;
    }
}
