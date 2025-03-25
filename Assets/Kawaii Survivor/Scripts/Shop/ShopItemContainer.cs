using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopItemContainer : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header(" Stats ")]
    [SerializeField] private Transform statContainersParent;

    [SerializeField] public Button purchaseButton;

    [Header(" Color ")]
    [SerializeField] private Image[] levelDependentImages;
    [SerializeField] private Image outline;

    [Header(" Lock Elements ")]
    [SerializeField] private Image lockImage;
    [SerializeField] private Sprite lockedSprite, unlockedSprite;
    public bool IsLocked { get; private set; }

    [Header(" Purchasing ")]
    public WeaponDataSO WeaponData { get; private set; }
    public ObjectDataSO ObjectData { get; private set; }
    private int weaponLevel;

    [Header(" Actions ")]
    public static Action<ShopItemContainer, int> onPurchased;


    private void Awake()
    {
        CurrencyManager.onUpdated += CurrencyUpdatedCallback;
    }

    private void CurrencyUpdatedCallback()
    {
        // Skip the callback if this container has been cleared/sold
        if (WeaponData == null && ObjectData == null)
            return;

        int itemPrice;
        if (WeaponData != null)
            itemPrice = WeaponStatsCalculator.GetPurchasePrice(WeaponData, weaponLevel);
        else
            itemPrice = ObjectData.Price;

        purchaseButton.interactable = CurrencyManager.instance.HasEnoughCurrency(itemPrice);
    }

    private void OnDestroy()
    {
        CurrencyManager.onUpdated -= CurrencyUpdatedCallback;
    }
    public void Configure(WeaponDataSO weaponData, int level)
    {
        weaponLevel = level;
        WeaponData = weaponData;

        icon.sprite = weaponData.Sprite;
        nameText.text = weaponData.Name + $" (lvl {level + 1})";
        priceText.text = WeaponStatsCalculator.GetPurchasePrice(weaponData, level).ToString();

        int weaponPrice = WeaponStatsCalculator.GetPurchasePrice(weaponData, level);

        Color imageColor = ColorHolder.GetColor(level);
        nameText.color = imageColor;
        outline.color = ColorHolder.GetOutlineColor(level);

        foreach (Image image in levelDependentImages)
            image.color = imageColor;

        Dictionary<Stat, float> calculatedStats = WeaponStatsCalculator.GetStats(weaponData, level);

        ConfigureStatContainers(calculatedStats);

        purchaseButton.onClick.AddListener(Purchase);
        purchaseButton.interactable = CurrencyManager.instance.HasEnoughCurrency(weaponPrice);
    }
    public void Configure(ObjectDataSO objectData)
    {
        ObjectData = objectData;

        icon.sprite = objectData.Icon;
        nameText.text = objectData.Name;
        priceText.text = objectData.Price.ToString();

        Color imageColor = ColorHolder.GetColor(objectData.Rarity);
        nameText.color = imageColor;
        outline.color = ColorHolder.GetOutlineColor(objectData.Rarity);

        foreach (Image image in levelDependentImages)
            image.color = imageColor;
        ConfigureStatContainers(objectData.BaseStats);

        purchaseButton.onClick.AddListener(Purchase);
        purchaseButton.interactable = CurrencyManager.instance.HasEnoughCurrency(objectData.Price);

    }


    private void ConfigureStatContainers(Dictionary<Stat, float> stats)
    {
        statContainersParent.Clear();
        StatContainerManager.GenerateStatContainers(stats, statContainersParent);

    }
    


    private void Purchase()
    {
        onPurchased?.Invoke(this, weaponLevel);
    }
    public void LockButtonCallBack() 
    {
        IsLocked = !IsLocked;
        UpdateLockVisuals();
    }
    private void UpdateLockVisuals()
    {
        lockImage.sprite = IsLocked ? lockedSprite : unlockedSprite;
    }




    //public void Configure(WeaponDataSO weaponData, int level)
    //{
    //    weaponLevel = level;
    //    WeaponData = weaponData;

    //    icon.sprite = weaponData.Sprite;
    //    nameText.text = weaponData.Name + $" (lvl {level + 1})";

    //    int weaponPrice = WeaponStatsCalculator.GetPurchasePrice(weaponData, level);

    //    priceText.text = weaponPrice.ToString();

    //    Color imageColor = ColorHolder.GetColor(level);
    //    nameText.color = imageColor;

    //    outline.color = ColorHolder.GetOutlineColor(level);

    //    foreach (Image image in levelDependentImages)
    //        image.color = imageColor;

    //    Dictionary<Stat, float> calculatedStats = WeaponStatsCalculator.GetStats(weaponData, level);

    //    ConfigureStatContainers(calculatedStats);

    //    purchaseButton.onClick.AddListener(Purchase);
    //    purchaseButton.interactable = CurrencyManager.instance.HasEnoughCurrency(weaponPrice);
    //}

    //public void Configure(ObjectDataSO objectData)
    //{
    //    ObjectData = objectData;

    //    icon.sprite = objectData.Icon;
    //    nameText.text = objectData.Name;
    //    priceText.text = objectData.Price.ToString();

    //    Color imageColor = ColorHolder.GetColor(objectData.Rarity);
    //    nameText.color = imageColor;

    //    outline.color = ColorHolder.GetOutlineColor(objectData.Rarity);

    //    foreach (Image image in levelDependentImages)
    //        image.color = imageColor;

    //    ConfigureStatContainers(objectData.BaseStats);

    //    purchaseButton.onClick.AddListener(Purchase);
    //    purchaseButton.interactable = CurrencyManager.instance.HasEnoughCurrency(objectData.Price);
    //}

    //private void ConfigureStatContainers(Dictionary<Stat, float> stats)
    //{
    //    statContainersParent.Clear();
    //    StatContainerManager.GenerateStatContainers(stats, statContainersParent);
    //}



    //public void LockButtonCallback()
    //{
    //    IsLocked = !IsLocked;
    //    UpdateLockVisuals();
    //}

    //private void UpdateLockVisuals()
    //{
    //    lockImage.sprite = IsLocked ? lockedSprite : unlockedSprite;
    //}
}
