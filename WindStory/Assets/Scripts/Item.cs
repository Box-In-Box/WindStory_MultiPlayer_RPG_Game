using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{ 
    /*
     * ����� : 10000 ~ 19999
     * ���� : 20000 ~ 29999
     * ��Ÿ�� : 30000 ~ 39999
     * ����Ʈ�� : 40000 ~ 49999
     */
    public int itemID;  //������ ���� ID. �ߺ� �Ұ���
    public string itemName; //������ �̸�. �ߺ� ���� 
    public string itemDescription;  //������ ����
    public ItemType itemType;
    public int itemCount;   //���� ����
    public int attack; //���ݷ�
    public int defense; //����

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
