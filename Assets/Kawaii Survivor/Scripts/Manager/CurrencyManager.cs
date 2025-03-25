using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tabsil.Sijil;


public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [field: SerializeField] public int Currency { get; private set; }
    
    [Header(" Settings ")]
    [SerializeField] private int cashValue = 10;

    [Header(" Actions ")]
    public static Action onUpdated;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        //AddPremiumCurrency(PlayerPrefs.GetInt(premiumCurrencyKey, 100), false);

        //Candy.onCollected += CandyCollectedCallback;
        Cash.onCollected += CashCollectedCallback;
    }
    
    private void OnDestroy()
    {
        Cash.onCollected -= CashCollectedCallback;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateTexts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void CashCollectedCallback(Cash cash)
    {
        AddCurrency(cashValue);
    }

    [NaughtyAttributes.Button]
    private void Add500Currency() => AddCurrency(500);
    public void AddCurrency(int amount)
    {
        Currency += amount;
        UpdateTexts();

        onUpdated?.Invoke();
        //UpdateVisuals();
    }
    private void UpdateTexts()
    {
        CurrencyText[] currencyTexts = FindObjectsByType<CurrencyText>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (CurrencyText text in currencyTexts)
            text.UpdateText(Currency.ToString());
    }
    public void UseCurrency(int price) => AddCurrency(-price);
    public bool HasEnoughCurrency(int price) => Currency >= price;
}
