using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using NaughtyAttributes;

using Random = UnityEngine.Random;
using Unity.VisualScripting;
using System.Resources;
public class WaveTransitionManager : MonoBehaviour, IGameStateListener
{
    public static WaveTransitionManager instance;

    [Header(" Player ")]
    [SerializeField] private PlayerObjects playerObjects;


    [Header(" Elements ")]
    [SerializeField] private PlayerStatsManager playerStatsManager;
    [SerializeField] private UpgradeContainer[] upgradeContainers;
    [SerializeField] private GameObject upgradeContainersParent;



    [Header(" Chest Related Stuff ")]
    [SerializeField] private ChestObjectContainer chestContainerPrefab;
    [SerializeField] private Transform chestContainerParent;


    [Header(" Settings ")]
    private int chestsCollected;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Chest.onCollected += ChestCollectedCallback;
    }
    private void OnDestroy()
    {
        Chest.onCollected -= ChestCollectedCallback;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure containers are properly initialized
        ClearChestContainer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState) 
        {
            case GameState.WAVETRANSITION:
                // Always clear containers first
                ClearChestContainer();
                
                // Check if this is a level-up or chest collection
                if (Player.instance.HasLeveledUp())
                {
                    // Show level-up screen
                    ConfigureUpgradeContainer();
                }
                else
                {
                    // Handle chest only if we've collected one
                    TryOpenChest();
                }
                break;
        }
    }

    private void TryOpenChest()
    {
        ClearChestContainer();

        if (chestsCollected > 0)
            ShowObject();
        else
            // If no chests and somehow we got here, go to shop
            GameManager.instance.SetGameState(GameState.SHOP);
    }
    
    private void ClearChestContainer()
    {
        // Destroy all child objects in the chest container
        if (chestContainerParent != null)
        {
            foreach (Transform child in chestContainerParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void ShowObject()
    {
        chestsCollected--;

        // Ensure upgrade container is hidden
        upgradeContainersParent.SetActive(false);

        ObjectDataSO[] objectDatas = ResourcesManager.Objects;

        ObjectDataSO randomObjectData = objectDatas[Random.Range(0, objectDatas.Length)];

        ChestObjectContainer containerInstance = Instantiate(chestContainerPrefab, chestContainerParent);
        containerInstance.Configure(randomObjectData);
        if (randomObjectData == null) 
        {
            Debug.Log("randomObjectData is null");
        }
        containerInstance.TakeButton.onClick.AddListener(() => TakeButtonCallback(randomObjectData));
        containerInstance.RecycleButton.onClick.AddListener(() => RecyleButtonCallback(randomObjectData));
    }
    private void TakeButtonCallback(ObjectDataSO objectToTake)
    {
        playerObjects.AddObject(objectToTake);
        if(objectToTake == null) 
        {
            Debug.Log("objetToTake is Null");
        }
        TryOpenChest();
    }
    private void RecyleButtonCallback(ObjectDataSO objectToRecycle)
    {
        CurrencyManager.instance.AddCurrency(objectToRecycle.RecyclePrice);
        TryOpenChest();
    }


    [Button]
    private void ConfigureUpgradeContainer()
    {
        // Always clear the chest container first
        ClearChestContainer();
        
        // Then activate the upgrade container
        upgradeContainersParent.SetActive(true);
       
        for (int i = 0; i < upgradeContainers.Length; i++)
        {
            int randomIndex = Random.Range(0, Enum.GetValues(typeof(Stat)).Length);
            Stat stat = (Stat)Enum.GetValues(typeof(Stat)).GetValue(randomIndex);

            Sprite upgradeSprite = ResourcesManager.GetStatIcon(stat);
            string randomStatString = Enums.FormatStatName(stat);

            string buttonString;
            Action action = GetActionToPerform(stat, out buttonString);

            upgradeContainers[i].Configure(upgradeSprite, randomStatString, buttonString);


            upgradeContainers[i].Button.onClick.RemoveAllListeners();

            upgradeContainers[i].Button.onClick.AddListener(() => action?.Invoke());

            upgradeContainers[i].Button.onClick.AddListener(() => BonusSelectedCallback());
        }
    }

    private void BonusSelectedCallback()
    {
        // Check if there are more level-ups to process
        if (Player.instance.HasLeveledUp())
        {
            // We have more level-ups to handle, so show the upgrade container again
            Debug.Log("Player has another level-up, showing upgrade UI again");
            ConfigureUpgradeContainer();
        }
        else if (HasCollectedChest())
        {
            // No more level-ups but we have a chest, so proceed to chest
            Debug.Log("No more level-ups, but has a chest. Showing chest UI");
            TryOpenChest();
        }
        else
        {
            // No more level-ups or chests, so proceed to shop
            Debug.Log("No more level-ups or chests, going to shop");
            GameManager.instance.SetGameState(GameState.SHOP);
        }
    }

    private Action GetActionToPerform(Stat stat, out string buttonString)
    {
        buttonString = "";
        float value;

        switch (stat)
        {
            case Stat.Attack:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.AttackSpeed:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.CriticalChance:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.CriticalPercent:
                value = Random.Range(1f, 2f);
                buttonString = "+" + value.ToString("F2") + "x";
                break;

            case Stat.MoveSpeed:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";

                break;

            case Stat.MaxHealth:
                value = Random.Range(1, 5);
                buttonString = "+" + value;

                break;

            case Stat.Range:
                value = Random.Range(1f, 5f);
                buttonString = "+" + value.ToString("F2");
                break;

            case Stat.HealthRecoverySpeed:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";

                break;

            case Stat.Armor:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";

                break;

            case Stat.Luck:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";

                break;

            case Stat.Dodge:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";

                break;

            case Stat.LifeSteal:
                value = Random.Range(1, 10);
                buttonString = "+" + value.ToString() + "%";

                break;


            default:
                return () => Debug.Log("Invalid stat");
        }
        //buttonString = Enums.FormatStatName(stat) + "\n" + buttonString;
        return () => playerStatsManager.AddPlayerStat(stat, value);
    }

    private void ChestCollectedCallback()
    {
        chestsCollected++;
        Debug.Log($"We now have {chestsCollected} chests");
    }
    public bool HasCollectedChest()
    {
        return chestsCollected > 0;
    }
}
