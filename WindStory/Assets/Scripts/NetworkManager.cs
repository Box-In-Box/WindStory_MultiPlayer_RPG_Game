using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    [Header("---PlayFab---")]
    public InputField EmailInput;
    public InputField PasswordInput;
    public InputField UsernameInput;
    public RobbyManager robbyManager;
    public bool isLogin = false;

    [Header("---Photon---")]
    public InputField NickNameInput;
    public string nickName;

    void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);

        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    #region "PlayFab"
    void Start() { robbyManager = FindObjectOfType<RobbyManager>(); }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        isLogin = true;
        Debug.Log("로그인 성공");
        robbyManager.Login();
    }

    void OnLoginFailure(PlayFabError error) => Debug.Log("로그인 실패");
    void OnRegisterSuccess(RegisterPlayFabUserResult result) => Debug.Log("회원가입 성공");
    void OnRegisterFailure(PlayFabError error) => Debug.Log("회원가입 실패");
    #endregion

    #region "Photon"
    public void GetNickName() { nickName = NickNameInput.text; }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = nickName == "" ? "Player" : nickName;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom() => Spawn();
  
    public void Spawn()
    {
        NickNameInput = null;
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-3f, 3f), -1.8f, 0), Quaternion.identity);
        Infomation();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

    void Infomation()
    {
        if (PhotonNetwork.InRoom)
        {
            print("방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);
            string playerStr = "플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) 
                playerStr += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
            print(playerStr);
        }
        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }
    #endregion
}
