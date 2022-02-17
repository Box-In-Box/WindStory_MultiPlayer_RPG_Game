using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    [Header("---PlayFab---")]
    public InputField LoginEmailInput;
    public InputField LoginPasswordInput;

    public InputField RegisterEmailInput;
    public InputField RegisterPasswordInput;
    public InputField RegisterUsernameInput;

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
        var request = new LoginWithEmailAddressRequest { Email = LoginEmailInput.text, Password = LoginPasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest { Email = RegisterEmailInput.text, Password = RegisterPasswordInput.text, Username = RegisterUsernameInput.text };
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
            string info = "방 이름 : " + PhotonNetwork.CurrentRoom.Name;
            info += ", 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            info += ", 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                info += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
            print(info);
        }
    }
    #endregion
}
