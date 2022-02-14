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

        //초기화
        for (int i = 0; i < ChannelPanel.transform.childCount; i++)
            ChannelPanel.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
        ConnectGame.GetComponent<Button>().interactable = false;
    }

    private void Update()
    {
        if (ServerConnectPanel.activeSelf && !isLogin)
        {
            //탭키로 인풋필드 이메일 -> 비밀번호 -> 이름 
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (next != null) next.Select();
            }
            //엔터로 로그인 클릭
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

    // ***선택된 이미지와 아닌 이미지 구분 스크립트 재변경 필요***
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
    // ***선택된 이미지와 아닌 이미지 구분 스크립트 재변경 필요***
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
