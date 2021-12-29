using Photon.Pun;
using UnityEngine;

public class Ball : MonoBehaviourPun
{
    // IsMasterClient = Host Player인가, photonView.IsMine = Host에서 생성된 GameObject인가
    public bool IsMasterClientLocal => PhotonNetwork.IsMasterClient && photonView.IsMine;

    private Vector2 direction = Vector2.right;
    private readonly float speed = 10f;
    private readonly float randomRefectionIntensity = 0.1f;
    
    private void FixedUpdate()
    {
        if (!IsMasterClientLocal || PhotonNetwork.PlayerList.Length < 2) return;

        var distance = speed * Time.deltaTime;
        var hit = Physics2D.Raycast(transform.position, direction, distance);

        if (hit.collider != null)
        {
            // 골대에 들어갔는지 체크
            var goalPost = hit.collider.GetComponent<Goalpost>();
            if (goalPost != null)
            {
                if (goalPost.playerNumber == 1)
                {
                    GameManager.Instance.AddScore(2, 1);
                }
                else if (goalPost.playerNumber == 2)
                {
                    GameManager.Instance.AddScore(1, 1);
                }
            }

            // 부딪힌 곳에서 반사되는 벡터 구하기
            direction = Vector2.Reflect(direction, hit.normal);
            // Random.insideUnitCircle = 반지름이 한 유닛인 원 안에 랜덤한 vector를 리턴
            direction += Random.insideUnitCircle * randomRefectionIntensity;

        }
        // 공의 position은 어쨌거나 direction * distance로 움직인다. 
        transform.position = (Vector2)transform.position + direction * distance;
    }
}