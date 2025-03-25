using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(WaveManagerUI))]
public class WaveManager : MonoBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private Player player;
    private WaveManagerUI ui;
    [SerializeField] private EnvironmentEffectsManager environmentManager;

    [Header(" Settings ")]
    [SerializeField] private float waveDuration;
    private float timer;
    private bool isTimerOn;
    private int currentWaveIndex;

    [Header(" Environment Effects ")]
    [SerializeField] private bool enableDayNightCycle = true;
    [SerializeField] private bool enableRandomRain = true;
    [SerializeField] private float rainChancePerWave = 0.3f; // 30% chance per wave
    [SerializeField] private float minRainDuration = 10f;
    [SerializeField] private float maxRainDuration = 30f;
    private bool isNightTime = false;
    private bool wasRainTriggered = false;
    private Coroutine rainCoroutine;

    [Header(" Waves ")]
    [SerializeField] private Wave[] waves;
    private List<float> localCounters = new List<float>();

    private void Awake()
    {
        ui = GetComponent<WaveManagerUI>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (environmentManager == null)
            environmentManager = FindObjectOfType<EnvironmentEffectsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimerOn)
            return;
        if (timer < waveDuration)
        {
            ManageCurrentWave();
            ManageEnvironmentEffects();

            string timerString = ((int)(waveDuration - timer)).ToString();
            ui.UpdateTimerText(timerString);
        }
        else StartWaveTransition();
    }

    private void StartWave(int waveIndex)
    {
        ui.UpdateWaveText("Wave " + (currentWaveIndex + 1) + " / " + waves.Length);
        localCounters.Clear();
        foreach (WaveSegment segment in waves[waveIndex].segments)
            localCounters.Add(1);

        timer = 0;
        isTimerOn = true;
        isNightTime = false;
        wasRainTriggered = false;
        
        // Reset to day time at start of wave
        if (environmentManager != null && enableDayNightCycle)
        {
            environmentManager.SetNightTime(false);
        }
        
        // Consider random rain for this wave
        TryStartRandomRain();
    }
    
    private void ManageEnvironmentEffects()
    {
        // Handle day/night cycle
        if (enableDayNightCycle && environmentManager != null)
        {
            // Switch to night time when timer reaches half of wave duration
            if (timer >= waveDuration / 2 && !isNightTime)
            {
                isNightTime = true;
                environmentManager.SetNightTime(true);
            }
        }
        
        // Handle random rain if it hasn't been triggered yet this wave
        if (enableRandomRain && !wasRainTriggered && Random.value < rainChancePerWave / waveDuration * Time.deltaTime)
        {
            TriggerRainEffect();
        }
    }
    
    private void TryStartRandomRain()
    {
        if (enableRandomRain && Random.value < rainChancePerWave)
        {
            // Trigger rain after a random delay
            float randomDelay = Random.Range(5f, waveDuration / 2);
            rainCoroutine = StartCoroutine(TriggerRainAfterDelay(randomDelay));
        }
    }
    
    private System.Collections.IEnumerator TriggerRainAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerRainEffect();
    }
    
    private void TriggerRainEffect()
    {
        if (environmentManager != null && !wasRainTriggered)
        {
            wasRainTriggered = true;
            float rainDuration = Random.Range(minRainDuration, maxRainDuration);
            
            environmentManager.SetRainEffect(true);
            
            // Schedule rain stop
            rainCoroutine = StartCoroutine(StopRainAfterDelay(rainDuration));
        }
    }
    
    private System.Collections.IEnumerator StopRainAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (environmentManager != null)
            environmentManager.SetRainEffect(false);
    }

    private void ManageCurrentWave()
    {
        Wave currentWave = waves[currentWaveIndex];

        for (int i = 0; i < currentWave.segments.Count; i++)
        {
            WaveSegment segment = currentWave.segments[i];

            float tStart = segment.tStartEnd.x / 100 * waveDuration;
            float tEnd = segment.tStartEnd.y / 100 * waveDuration;

            if (timer < tStart || timer > tEnd)
            {
                continue;
            }
            float timeSinceSegmentStart = timer - tStart;

            float spawnDelay = 1f / segment.spawnFrequency;

            if (timeSinceSegmentStart / spawnDelay > localCounters[i])
            {
                Instantiate(segment.prefab, GetSpawnPosition(), Quaternion.identity, transform);
                localCounters[i]++;
            }
        }
        timer += Time.deltaTime;
    }

    private void StartWaveTransition()
    {
        isTimerOn = false;

        DefeatAllEnemies();
        
        // Stop any ongoing rain
        if (rainCoroutine != null)
        {
            StopCoroutine(rainCoroutine);
            rainCoroutine = null;
        }
        
        if (environmentManager != null)
        {
            environmentManager.SetRainEffect(false);
            // Reset to day for transition
            environmentManager.SetNightTime(false);
        }

        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            ui.UpdateTimerText("");
            ui.UpdateWaveText("Stage Completed");
            GameManager.instance.SetGameState(GameState.STAGECOMPLETE);
        }
        else 
        {
            GameManager.instance.WaveCompletedCallback();
        }
    }
    
    private void StartNextWave()
    {
        StartWave(currentWaveIndex);
    }
    
    private void DefeatAllEnemies()
    {
        foreach (Enemy enemy in transform.GetComponentsInChildren<Enemy>())
            enemy.PassAwayAfterWave();
            
        ClearArea();
    }
    
    private void ClearArea()
    {
        // Find and deactivate all uncollected items
        Cash[] cashItems = FindObjectsOfType<Cash>();
        foreach (Cash cash in cashItems)
        {
            cash.gameObject.SetActive(false);
        }
        
        // Find and deactivate candy items
        Candy[] candyItems = FindObjectsOfType<Candy>();
        foreach (Candy candy in candyItems)
        {
            candy.gameObject.SetActive(false);
        }
        
        // Find and deactivate chest items
        Chest[] chestItems = FindObjectsOfType<Chest>();
        foreach (Chest chest in chestItems)
        {
            chest.gameObject.SetActive(false);
        }
    }
    
    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = Random.onUnitSphere;
        Vector2 offset = direction.normalized * Random.Range(6, 10);
        Vector2 targetPosition = (Vector2)player.transform.position + offset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -18, 18);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -8, 8);

        return targetPosition;
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GAME:
                StartNextWave();
                break;

            case GameState.GAMEOVER:
                isTimerOn = false;
                DefeatAllEnemies();
                
                // Reset environment effects
                if (environmentManager != null)
                {
                    environmentManager.SetRainEffect(false);
                    environmentManager.SetNightTime(false);
                }
                break;
        }
    }
}

[System.Serializable]
public struct Wave
{
    public string name;
    public List<WaveSegment> segments;
}

[System.Serializable]
public struct WaveSegment
{
    [MinMaxSlider(0, 100)] public Vector2 tStartEnd;
    public float spawnFrequency;
    public GameObject prefab;
}
