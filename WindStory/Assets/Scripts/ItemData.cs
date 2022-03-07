using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemData : MonoBehaviour
{
    private Image image;

    public int itemID;  //아이템 고유 ID. 중복 불가능
    public string itemName; //아이템 이름. 중복 가능 
    public string itemDescription;  //아이템 설명
    public ItemType itemType;
    public int itemCount;   //소지 개수
    public Sprite itemIcon; //아이템 아이디 값과 같은 이름
    public int attack; //공격력
    public int defense; //방어력

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
