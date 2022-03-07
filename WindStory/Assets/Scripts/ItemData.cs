using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemData : MonoBehaviour
{
    private Image image;

    public int itemID;  //������ ���� ID. �ߺ� �Ұ���
    public string itemName; //������ �̸�. �ߺ� ���� 
    public string itemDescription;  //������ ����
    public ItemType itemType;
    public int itemCount;   //���� ����
    public Sprite itemIcon; //������ ���̵� ���� ���� �̸�
    public int attack; //���ݷ�
    public int defense; //����

    public enum ItemType
    {
        Equipment,
        Consumable,
        Quest,
        ETC
    }

    public void setting(Item _item)
    {
        itemID = _item.itemID;
        itemName = _item.itemName;
        itemDescription = _item.itemDescription;
        itemType = (ItemType)_item.itemType;
        itemCount = _item.itemCount;
        itemIcon = Resources.Load("Image/ItemIcon/" + _item.itemID.ToString(), typeof(Sprite)) as Sprite;
        attack = _item.attack;
        defense = _item.defense;

        image = GetComponent<Image>();
        image.sprite = itemIcon;
    }
}
