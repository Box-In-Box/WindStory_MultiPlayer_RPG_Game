using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private DatabaseManager databaseManager;
    private Transform[] inventoryPanel; //���� �κ��丮 �г�
    public List<Item> inventoryItemList;   //������ ������ ����Ʈ
    public GameObject itemObj;  //������ �����ϴ� ������ 

    // Start is called before the first frame update
    void Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
        inventoryPanel = new Transform[transform.childCount];   //������ �ִ� ���� ����
        inventoryItemList = new List<Item>();

        for (int i = 0; i < inventoryPanel.Length; i++)
        {
            inventoryPanel[i] = transform.GetChild(i).transform.GetChild(3).transform; // InventoryCase > inventoryPanel
        }
    }

    public void GetAnItem(int _itemID, int _count = 1)
    {
        for(int i = 0; i < databaseManager.itemList.Count; i++) //�����ͺ��̽� ������ �˻�.
        {
            if (_itemID == databaseManager.itemList[i].itemID)   //�����ͺ����� ������ �߰�
            {
                for (int j = 0; j < inventoryItemList.Count; j++)    //�κ��丮�� ���� �������� �ִ��� �˻�.
                {
                    if(inventoryItemList[j].itemID == _itemID)     //�������� ������ ������� count ����
                    {
                        inventoryItemList[j].itemCount += _count;
                        return;
                    }
                }
                inventoryItemList.Add(databaseManager.itemList[i]); //�������� ������ �������� ��� �κ��丮 ����Ʈ�� �߰�
                ItemSetting(databaseManager.itemList[i]);
                return;
            }
        }
        Debug.Log("�����ͺ��̽��� �ش� ID�� ���� �������� �������� �ʽ��ϴ�."); //�����ͺ��̽��� �������� �������� ���� ���
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
