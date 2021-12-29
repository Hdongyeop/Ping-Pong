using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";

    public Text connectionInfoText;
    public Button joinButton;
    
    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        // Photon Network Master Server 접속
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
        connectionInfoText.text = "Connecting to master server...";
        
    }
    
    // Master Server에 접속됬을 때 실행되는 Method
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "ONLINE : Connected to master server";
    }
    
    // 접속이 안되거나 끊겼을 경우
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = $"OFFLINE : Connection disabled {cause.ToString()} - try connecting again";

        PhotonNetwork.ConnectUsingSettings();
    }
    
    public void Connect()
    {
        joinButton.interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "Connecting to random room";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "OFFLINE : Connection disabled - try connecting again";

            PhotonNetwork.ConnectUsingSettings();
        }
    }
    
    // Master서버에 비어 있는 Room이 하나도 없을 때
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "There is no empty room, creating a new room";
        // 빈 방이 없기 때문에 새로운 Room을 만듬
        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 2});
    }
    
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "Connected with room";
        // SceneManager.LoadScene으로 넘어가지 않는 이유는 Local만 넘어가기 떄문이다.
        PhotonNetwork.LoadLevel("Main");
    }
}