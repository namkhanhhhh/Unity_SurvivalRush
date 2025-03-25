using System;
using System.Collections;
using UnityEngine;

public class EnvironmentEffectsManager : MonoBehaviour
{
    public static EnvironmentEffectsManager instance;

    [Header("Day/Night Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = new Color(0.1f, 0.1f, 0.3f, 1f);
    [SerializeField] private float colorTransitionDuration = 2f;
    
    [Header("Rain Settings")]
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private float rainSpeedChangeInterval = 2f;
    [SerializeField] private float minSpeedMultiplier = 0.7f;
    [SerializeField] private float maxSpeedMultiplier = 1.0f;
    
    private bool isNightTime = false;
    private bool isRaining = false;
    private Coroutine rainSpeedChangeRoutine;

    public static Action<bool> onDayNightChanged;
    public static Action<bool> onRainStatusChanged;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        if (rainParticles != null)
            rainParticles.gameObject.SetActive(false);
    }

    public void SetNightTime(bool isNight)
    {
        if (isNightTime == isNight) return;
        
        isNightTime = isNight;
        StartCoroutine(TransitionBackgroundColor(isNight ? nightColor : dayColor));
        onDayNightChanged?.Invoke(isNight);
    }

    public void SetRainEffect(bool isActive)
    {
        if (isRaining == isActive) return;
        
        isRaining = isActive;
        
        if (rainParticles != null)
            rainParticles.gameObject.SetActive(isActive);
            
        if (isActive)
        {
            if (rainSpeedChangeRoutine != null)
                StopCoroutine(rainSpeedChangeRoutine);
                
            rainSpeedChangeRoutine = StartCoroutine(RandomizePlayerSpeed());
        }
        else
        {
            if (rainSpeedChangeRoutine != null)
            {
                StopCoroutine(rainSpeedChangeRoutine);
                rainSpeedChangeRoutine = null;
            }
            
            // Reset player speed to normal
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach (var player in players)
            {
                player.ResetRainSpeedEffect();
            }
        }
        
        onRainStatusChanged?.Invoke(isActive);
    }

    private IEnumerator TransitionBackgroundColor(Color targetColor)
    {
        Color startColor = mainCamera.backgroundColor;
        float elapsedTime = 0f;
        
        while (elapsedTime < colorTransitionDuration)
        {
            mainCamera.backgroundColor = Color.Lerp(startColor, targetColor, elapsedTime / colorTransitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.backgroundColor = targetColor;
    }

    private IEnumerator RandomizePlayerSpeed()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        
        while (isRaining)
        {
            float randomMultiplier = UnityEngine.Random.Range(minSpeedMultiplier, maxSpeedMultiplier);
            
            foreach (var player in players)
            {
                player.ApplyRainSpeedEffect(randomMultiplier);
            }
            
            yield return new WaitForSeconds(rainSpeedChangeInterval);
        }
    }

    public bool IsNightTime()
    {
        return isNightTime;
    }

    public bool IsRaining()
    {
        return isRaining;
    }
} 