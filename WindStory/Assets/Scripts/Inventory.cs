using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private DatabaseManager databaseManager;
    private Transform[] inventoryPanel; //가방 인벤토리 패널
    public List<Item> inventoryItemList;   //소지한 아이템 리스트
    public GameObject itemObj;  //아이템 생성하는 프리펩 

    // Start is called before the first frame update
    void Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
        inventoryPanel = new Transform[transform.childCount];   //가지고 있는 가방 개수
        inventoryItemList = new List<Item>();

        for (int i = 0; i < inventoryPanel.Length; i++)
        {
            inventoryPanel[i] = transform.GetChild(i).transform.GetChild(3).transform; // InventoryCase > inventoryPanel
        }
    }

    public void GetAnItem(int _itemID, int _count = 1)
    {
        for(int i = 0; i < databaseManager.itemList.Count; i++) //데이터베이스 아이템 검색.
        {
            if (_itemID == databaseManager.itemList[i].itemID)   //데이터베에스 아이템 발견
            {
                for (int j = 0; j < inventoryItemList.Count; j++)    //인벤토리에 같은 아이템이 있는지 검색.
                {
                    if(inventoryItemList[j].itemID == _itemID)     //아이템을 가지고 있을경우 count 증가
                    {
                        inventoryItemList[j].itemCount += _count;
                        return;
                    }
                }
                inventoryItemList.Add(databaseManager.itemList[i]); //아이템을 가지고 있지않을 경우 인벤토리 리스트에 추가
                ItemSetting(databaseManager.itemList[i]);
                return;
            }
        }
        Debug.Log("데이터베이스에 해당 ID를 가진 아이템이 존재하지 않습니다."); //데이터베이스에 아이템이 존재하지 않은 경우
    }

    void ItemSetting(Item _item)
    {
        for (int i = 0; i < inventoryPanel.Length; i++)
        {
            for (int j = 0; j < inventoryPanel[i].childCount; j++)
            {
                if (inventoryPanel[i].GetChild(j).transform.childCount == 0)
                {
                    GameObject item = Instantiate(itemObj, inventoryPanel[i].GetChild(j).transform.position, Quaternion.identity, inventoryPanel[i].GetChild(j).transform);
                    item.GetComponent<ItemData>().setting(_item);
                    return;
                }
            }
        }
    }
}
