using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField]
    private Player players;



    [Header("Settings")]
    [SerializeField]
    private float moveSpeed;

    // Update is called once per frame
    private void Start()
    {

    }
    void Update()
    {
        //if (players != null)
        //{
        //    FollowPlayer();
        //}

    }

    public void StorePlayer(Player player) => this.players = player;
        


    public void FollowPlayer()
    {
        Vector2 targetPosition = Vector2.MoveTowards(transform.position, players.transform.position, moveSpeed * Time.deltaTime);

        transform.position = targetPosition;
    }
}
