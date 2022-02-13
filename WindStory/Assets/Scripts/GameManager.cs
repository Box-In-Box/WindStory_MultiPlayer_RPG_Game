using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public NetworkManager networkManager;

    public GameObject GamePanel;
    public GameObject EscapePanel;

    private void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);
    }

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Setting();
    }

    public void Setting()
    {
        GamePanel = GameObject.Find("Canvas").gameObject.transform.Find("GamePanel").gameObject;
        EscapePanel = GameObject.Find("Canvas").gameObject.transform.Find("EscapePanel").gameObject;
        GamePanel.SetActive(true);
        GamePanel.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = networkManager.nickName;
    }
}
