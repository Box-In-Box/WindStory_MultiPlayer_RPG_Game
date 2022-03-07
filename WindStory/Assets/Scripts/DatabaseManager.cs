using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    public List<Item> itemList = new List<Item>();

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        itemList.Add(new Item(5000, "���� ����", "ü���� 100 ä���ش�.", Item.ItemType.Consumable));
        itemList.Add(new Item(5001, "�Ķ� ����", "������ 100 ä���ش�.", Item.ItemType.Consumable));
        itemList.Add(new Item(6000, "�⺻ ��", "���� �㸧�� ����", Item.ItemType.Equipment, 1, 5, 0));
    }
}
