using UnityEngine;

[RequireComponent(typeof(PlayerHealth), typeof(PlayerLevel))]
public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Components")]
    private PlayerHealth playerHealth;
    private PlayerLevel playerLevel;


    [SerializeField]
    private CircleCollider2D collider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);        
        }
            playerHealth = GetComponent<PlayerHealth>();
            playerLevel = GetComponent<PlayerLevel>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) 
    {
        playerHealth.TakeDamage(damage);
    }

    public Vector2 GetCenter() 
    {
        return (Vector2)transform.position + collider.offset;
    }
    public bool HasLeveledUp()
    {
        return playerLevel.HasLeveledUp();
    }
}
