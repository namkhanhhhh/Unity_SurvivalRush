using UnityEngine;
using UnityEngine.Pool;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance { get; private set; }
    [Header(" Elements ")]
    [SerializeField] private Candy candyPrefab;
    [SerializeField] private Cash cashPrefab;
    [SerializeField] private Chest chestPrefab;

    [Header(" Elements ")]
    [SerializeField] [Range(0,100)] private int cashDropChance;
    [SerializeField][Range(0, 100)] private int chestDropChance;


    [Header(" Pooling ")]
    private ObjectPool<Candy> candyPool;
    private ObjectPool<Cash> cashPool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Enemy.onPassedAway += EnemyPassedAwayCallback;
        Candy.onCollected += ReleaseCandy;
        Cash.onCollected += ReleaseCash;
    }
    private void OnDestroy()
    {
        Enemy.onPassedAway -= EnemyPassedAwayCallback;
        Candy.onCollected -= ReleaseCandy;
        Cash.onCollected -= ReleaseCash;
    }
    void Start()
    {
        candyPool = new ObjectPool<Candy>(
            CandyCreateFunction,
            CandyActionOnGet,
            CandyActionOnRelease,
            CandyActionOnDestroy);
        cashPool = new ObjectPool<Cash>(
            CashCreateFunction,
            CashActionOnGet,
            CashActionOnRelease,
            CashActionOnDestroy);


    }

    // Update is called once per frame
    void Update()
    {

    }
    private Candy CandyCreateFunction() => Instantiate(candyPrefab, transform);
    private void CandyActionOnGet(Candy candy) => candy.gameObject.SetActive(true);
    private void CandyActionOnRelease(Candy candy) => candy.gameObject.SetActive(false);
    private void CandyActionOnDestroy(Candy candy) => Destroy(candy.gameObject);




    private Cash CashCreateFunction() => Instantiate(cashPrefab, transform);
    private void CashActionOnGet(Cash cash) => cash.gameObject.SetActive(true);
    private void CashActionOnRelease(Cash cash) => cash.gameObject.SetActive(false);
    private void CashActionOnDestroy(Cash cash) => Destroy(cash.gameObject);



    private void EnemyPassedAwayCallback(Vector2 enemyPosition)
    {
        bool shouldSpawnCash = Random.Range(0, 101) <= cashDropChance;
        DroppableCurrency droppable = shouldSpawnCash ? cashPool.Get() : candyPool.Get();
        droppable.transform.position = enemyPosition;

        TryDropChest(enemyPosition);
    }
    private void TryDropChest(Vector2 spawnPosition) 
    {
        bool shouldSpawnChest = Random.Range(0, 101) <= chestDropChance;

        if (!shouldSpawnChest) return;
        Instantiate(chestPrefab, spawnPosition, Quaternion.identity, transform);
    }

    private void ReleaseCandy(Candy candy) => candyPool.Release(candy);
    private void ReleaseCash(Cash cash)    => cashPool.Release(cash);
}
