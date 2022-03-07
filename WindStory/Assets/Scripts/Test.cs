using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Inventory inventory;

    public int id;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "PassPlayer")
        {
            inventory.GetAnItem(id);
            Destroy(this.gameObject);
        }
    }
}
