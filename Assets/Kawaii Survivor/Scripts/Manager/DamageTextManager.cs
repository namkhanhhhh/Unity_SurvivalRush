using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private DamageText damageTextPrefab;

    [Header(" Pooling ")]
    private ObjectPool<DamageText> damageTextPool;

    private void Awake()
    {
        Enemy.onDamageTaken += EnemyHitCallback;
        PlayerHealth.onAttackDodged += AttackDodgedCallback;
    }
    private void OnDestroy()
    {

        Enemy.onDamageTaken -= EnemyHitCallback;
        PlayerHealth.onAttackDodged -= AttackDodgedCallback;
    }
    void Start()
    {
        damageTextPool = new ObjectPool<DamageText>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    }
    private void Update()
    {
        
    }
    private void EnemyHitCallback(int damage, Vector2 enemyPos, bool isCriticalHit) 
    {
        DamageText damageTextInstance = damageTextPool.Get();

        Vector3 spawnPosition = enemyPos + Vector2.up * 1.5f;
        damageTextInstance.transform.position = spawnPosition;

        damageTextInstance.Animate(damage.ToString(), isCriticalHit);

        LeanTween.delayedCall(1, () => damageTextPool.Release(damageTextInstance));
    }

    private void AttackDodgedCallback(Vector2 playerPosition)
    {
        DamageText damageTextInstance = damageTextPool.Get();

        Vector3 spawnPosition = playerPosition + Vector2.up * 1.5f;
        damageTextInstance.transform.position = spawnPosition;

        damageTextInstance.Animate("Dodged", false);

        LeanTween.delayedCall(1, () => damageTextPool.Release(damageTextInstance));
    }
    private DamageText CreateFunction()
    {
        return Instantiate(damageTextPrefab, transform);
    }

    private void ActionOnGet(DamageText damageText)
    {
        damageText.gameObject.SetActive(true);
    }

    private void ActionOnRelease(DamageText damageText)
    {
        if (damageText != null)
            damageText.gameObject.SetActive(false);
    }

    private void ActionOnDestroy(DamageText damageText)
    {
        Destroy(damageText.gameObject);
    }

    //private void EnemyHitCallback(int damage, Vector2 enemyPos, bool isCriticalHit)
    //{
    //    DamageText damageTextInstance = damageTextPool.Get();

    //    Vector3 spawnPosition = enemyPos + Vector2.up * 1.5f;
    //    damageTextInstance.transform.position = spawnPosition;

    //    damageTextInstance.Animate(damage.ToString(), isCriticalHit);

    //    LeanTween.delayedCall(1, () => damageTextPool.Release(damageTextInstance));
    //}

    //private void AttackDodgedCallback(Vector2 playerPosition)
    //{
    //    DamageText damageTextInstance = damageTextPool.Get();

    //    Vector3 spawnPosition = playerPosition + Vector2.up * 1.5f;
    //    damageTextInstance.transform.position = spawnPosition;

    //    damageTextInstance.Animate("Dodged", false);

    //    LeanTween.delayedCall(1, () => damageTextPool.Release(damageTextInstance));
    //}
}
