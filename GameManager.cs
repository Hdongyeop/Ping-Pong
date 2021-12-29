using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    private static GameManager instance;

    public Text scoreText;
    public Transform[] spawnPositions;
    public GameObject playerPrefab;
    public GameObject ballPrefab;

    private int[] playerScores;

    private void Start()
    {
        playerScores = new[] {0, 0};
        SpawnPlayer();
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBall();
        }
    }

    private void SpawnPlayer()
    {
        var localPlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber는 1부터 시작
        var spawnPosition = spawnPositions[localPlayerNumber % spawnPositions.Length];
        
        // [PhotonNetwork.Instantiate] Prefab을 생성하면 다른 Player들도 Object가 remote로써 생성되고 동기화 된다.
        // 주의 : Prefab의 name을 param으로 받는다.
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition.position, spawnPosition.rotation);
    }

    private void SpawnBall()
    {
        PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        // 방을 나갈 때 실행되는 함수
        SceneManager.LoadScene("Lobby");
    }

    public void AddScore(int playerNumber, int score)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        playerScores[playerNumber - 1] += score;
        
        photonView.RPC("RPCUpdateScoreText", RpcTarget.All, playerScores[0].ToString(), playerScores[1].ToString());
    }

    
    [PunRPC]
    private void RPCUpdateScoreText(string player1ScoreText, string player2ScoreText)
    {
        scoreText.text = $"{player1ScoreText} : {player2ScoreText}";
    }
}