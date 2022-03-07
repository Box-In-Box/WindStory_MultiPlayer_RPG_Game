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
        itemList.Add(new Item(5000, "빨간 물약", "체력을 100 채워준다.", Item.ItemType.Consumable));
        itemList.Add(new Item(5001, "파란 물약", "마나를 100 채워준다.", Item.ItemType.Consumable));
        itemList.Add(new Item(6000, "기본 검", "낡고 허름한 무기", Item.ItemType.Equipment, 1, 5, 0));
    }
}
