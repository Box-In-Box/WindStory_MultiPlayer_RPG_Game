using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject ServerConnectPanel;
    public GameObject EscapePanel;
    public GameObject GamePanel;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        ServerConnectPanel.SetActive(true);
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text == "" ? "Player" : NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        ServerConnectPanel.SetActive(false);
        Setting();
        Spawn();
    }

    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-3f, 3f), -1.8f, 0), Quaternion.identity);
    }

    public void Setting()
    {
        GamePanel.SetActive(true);
        GamePanel.transform.Find("NickNameText").gameObject.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) EscapePanel.SetActive(true);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    } 
}
