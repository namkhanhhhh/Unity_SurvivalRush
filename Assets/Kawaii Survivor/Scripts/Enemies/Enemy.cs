using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Enemy : MonoBehaviour
{
    [Header(" Components ")]
    protected EnemyMovement movement;

    [Header(" Health ")]
    [SerializeField] protected int maxHealth;
    protected int health;

    [Header(" Elements ")]
    protected Player player;

    [Header(" Spawn Sequence Related ")]
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] protected SpriteRenderer spawnIndicator;
    [SerializeField] protected Collider2D collider;
    protected bool hasSpawned;
    [SerializeField] protected bool hideSpawnIndicatorAtNight = true;

    [Header(" Effects ")]
    [SerializeField] protected ParticleSystem passAwayParticles;

    [Header(" Attack ")]
    [SerializeField] protected float playerDetectionRadius;

    [Header(" Actions ")]
    public static Action<int, Vector2, bool> onDamageTaken;
    public static Action<Vector2> onPassedAway;

    [Header(" DEBUG ")]
    [SerializeField] protected bool gizmos;

    private bool hasDied = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        movement = GetComponent<EnemyMovement>();
        player = FindFirstObjectByType<Player>();

        if (player == null)
        {
            Debug.LogWarning("No player found, Auto-destroying...");
            Destroy(gameObject);
        }

        // Subscribe to day/night changes
        EnvironmentEffectsManager.onDayNightChanged += OnDayNightChanged;
        
        StartSpawnSequence();
    }
    
    protected virtual void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        EnvironmentEffectsManager.onDayNightChanged -= OnDayNightChanged;
    }

    // Update is called once per frame
    protected bool CanAttack()
    {
        return renderer.enabled;
    }

    private void StartSpawnSequence()
    {
        SetRenderersVisibility(false);

        // Check if it's night time and should hide indicator
        if (hideSpawnIndicatorAtNight && 
            EnvironmentEffectsManager.instance != null && 
            EnvironmentEffectsManager.instance.IsNightTime())
        {
            SpawnSequenceCompleted();
            return;
        }

        // Scale up & down the spawn indicator
        Vector3 targetScale = spawnIndicator.transform.localScale * 1.2f;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, .3f)
            .setLoopPingPong(4)
            .setOnComplete(SpawnSequenceCompleted);
    }

    private void SpawnSequenceCompleted()
    {
        SetRenderersVisibility();
        hasSpawned = true;

        collider.enabled = true;

        movement.StorePlayer(player);
    }

    private void SetRenderersVisibility(bool visibility = true)
    {
        renderer.enabled = visibility;
        spawnIndicator.enabled = !visibility;
        
        // If night time and should hide indicator, force it to be invisible
        if (hideSpawnIndicatorAtNight && 
            EnvironmentEffectsManager.instance != null && 
            EnvironmentEffectsManager.instance.IsNightTime())
        {
            spawnIndicator.enabled = false;
        }
    }
    
    private void OnDayNightChanged(bool isNight)
    {
        // If spawn indicator is active during a transition to night, hide it
        if (isNight && hideSpawnIndicatorAtNight && !hasSpawned)
        {
            spawnIndicator.enabled = false;
        }
    }

    public void TakeDamage(int damage , bool isCriticalHit)
    {
        int realDamage = Mathf.Min(damage, health);
        health -= realDamage;

        onDamageTaken?.Invoke(damage, transform.position, isCriticalHit);

        if (health <= 0)
            PassAway();
    }

    public void PassAway()
    {
        //if (hasDied) return;  
        //hasDied = true;

        onPassedAway?.Invoke(transform.position);
        PassAwayAfterWave();

    }

    public void PassAwayAfterWave()
    {
        passAwayParticles.transform.SetParent(null);
        passAwayParticles.Play();

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!gizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
