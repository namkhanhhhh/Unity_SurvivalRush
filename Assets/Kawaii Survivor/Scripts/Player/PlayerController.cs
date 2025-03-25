using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerStatsDependency
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Elements")]     
    [SerializeField]
    private MobileJoystick playerJoystick;
    [SerializeField]
    private Rigidbody2D rig;

    [Header("Settings")]
    [SerializeField] 
    private float baseMoveSpeed;
    private float moveSpeed;
    
    [Header("Rain Effect")]
    [SerializeField] private bool affectedByRain = true;
    
    private float currentRainSpeedMultiplier = 1f;
    private bool isRainEffectActive = false;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        
    }
    
    private void FixedUpdate()
    {
        rig.linearVelocity = playerJoystick.GetMoveVector() * moveSpeed * Time.deltaTime;
    }

    public void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        float moveSpeedPercent = playerStatsManager.GetStatValue(Stat.MoveSpeed) / 100;
        float baseSpeed = baseMoveSpeed * (1 + moveSpeedPercent);
        
        // Apply rain effect if active
        moveSpeed = isRainEffectActive && affectedByRain ? baseSpeed * currentRainSpeedMultiplier : baseSpeed;
    }
    
    public void ApplyRainSpeedEffect(float speedMultiplier)
    {
        if (!affectedByRain) return;
        
        currentRainSpeedMultiplier = speedMultiplier;
        isRainEffectActive = true;
        
        // Update speed based on new multiplier
        UpdateStats(FindObjectOfType<PlayerStatsManager>());
    }
    
    public void ResetRainSpeedEffect()
    {
        currentRainSpeedMultiplier = 1f;
        isRainEffectActive = false;
        
        // Reset speed to normal
        UpdateStats(FindObjectOfType<PlayerStatsManager>());
    }
}
