using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RobbyManager : MonoBehaviour
{
    [Header("---serverConnect---")]
    public GameObject ServerConnectPanel;
    public Button loginTapBtn, registerTapBtn;
    public GameObject loginTap, registerTap;
    public bool isLogin = false;

    [Header("---RobbyPanel---")]
    public GameObject RobbyPanel;
    public GameObject WolrdPanel;
    public GameObject ChannelPanel;
    public Button ConnectGame;

    public int currentWorld = -1;
    public int currentChannel = -1;

    [Header("---ETC---")]
    EventSystem system;
    public AudioManager theAudio;

    void Start()
    {
        ServerConnectPanel.SetActive(true);
        RobbyPanel.SetActive(false);

        system = EventSystem.current;
        theAudio = FindObjectOfType<AudioManager>();
    }

    public void Login()
    {
        isLogin = true;
        RobbyPanel.SetActive(true);
        ServerConnectPanel.SetActive(false);

        //�ʱ�ȭ
        for (int i = 0; i < ChannelPanel.transform.childCount; i++)
            ChannelPanel.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
        ConnectGame.GetComponent<Button>().interactable = false;
    }

    private void Update()
    {
        if (ServerConnectPanel.activeSelf && !isLogin)
        {
            //��Ű�� ��ǲ�ʵ� �̸��� -> ��й�ȣ -> �̸� 
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (next != null) next.Select();
            }
            //���ͷ� �α��� Ŭ��
        }
    }

    public void LoginTap()
    {
        loginTap.SetActive(true);
        registerTap.SetActive(false);
        loginTapBtn.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
        registerTapBtn.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
    }

    public void RegisterTap()
    {
        loginTap.SetActive(false);
        registerTap.SetActive(true);
        loginTapBtn.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
        registerTapBtn.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
    }

    // ***���õ� �̹����� �ƴ� �̹��� ���� ��ũ��Ʈ �纯�� �ʿ�***
    public void SelectWolrd(int WorldNum)
    {
        currentWorld = WorldNum;
        currentChannel = -1;

        for (int i = 0; i < WolrdPanel.transform.childCount; i++)
            WolrdPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color = i == currentWorld ? 
                new Color(0.5f, 0.8f, 1f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f);

        for (int i = 0; i < ChannelPanel.transform.childCount; i++)
        {
            ChannelPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f, 1f);
            ChannelPanel.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = true;
        }
        ConnectGame.GetComponent<Button>().interactable = false;
    }
    // ***���õ� �̹����� �ƴ� �̹��� ���� ��ũ��Ʈ �纯�� �ʿ�***
    public void SelectChannel(int ChannelNum)
    {
        currentChannel = ChannelNum;
        for (int i = 0; i < ChannelPanel.transform.childCount; i++)
            ChannelPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color = i == currentChannel ?
                new Color(0.5f, 0.8f, 1f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f);

        ConnectGame.GetComponent<Button>().interactable = true;
    }

    public void SelectCharacter()
    {
        SceneManager.LoadScene("InGame");
    }
}
