using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
public class PlayerLevel : MonoBehaviour
{
    [Header(" Settings ")]
    private int requiredXp;
    private int currentXp;
    private int level;
    private int levelsEarnedThisWave;


    [Header("Visuals")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("DEBUG")]
    [SerializeField] private bool DEBUG;
    private void Awake()
    {
        Candy.onCollected += CandyCollectedCallback;
    }

    private void OnDestroy()
    {
        Candy.onCollected -= CandyCollectedCallback;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateRequiredXp();
        UpdateVisuals();

    }
    private void UpdateRequiredXp() => requiredXp = (level + 1) * 5;
    private void UpdateVisuals()
    {
        xpBar.value = (float)currentXp / requiredXp;
        levelText.text = "lvl " + (level + 1);
    }
    private void CandyCollectedCallback(Candy candy)
    {
        currentXp++;

        if (currentXp >= requiredXp)
            LevelUp();

        UpdateVisuals();
    }

    private void LevelUp()
    {
        level++;
        levelsEarnedThisWave++;
        currentXp = 0;
        UpdateRequiredXp();
    }
    public bool HasLeveledUp()
    {
        if (DEBUG)
            return true;

        if (levelsEarnedThisWave > 0)
        {
            levelsEarnedThisWave--;
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
