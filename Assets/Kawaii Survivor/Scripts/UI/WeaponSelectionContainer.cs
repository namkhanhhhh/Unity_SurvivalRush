using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSelectionContainer : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;


    [field: SerializeField] public Button Button { get; private set; }

    [Header(" Stats ")]
    [SerializeField] private Transform statContainersParent;
    [field: SerializeField] public Button button { get; private set; }
    [Header(" Color ")]
    [SerializeField] private Image[] levelDependentImages;
    [SerializeField] private Image outline;

    public void Configure(WeaponDataSO weaponData,int level)
    {
        icon.sprite = weaponData.Sprite;
        nameText.text = weaponData.Name + $" (lvl {level + 1})";

        Color imageColor = ColorHolder.GetColor(level);
        nameText.color = imageColor;
        outline.color = ColorHolder.GetOutlineColor(level);

        foreach (Image image in levelDependentImages)
            image.color = imageColor;

        Dictionary<Stat, float> calculatedStats = WeaponStatsCalculator.GetStats(weaponData, level);

        ConfigureStatContainers(calculatedStats);
    }

    private void ConfigureStatContainers(Dictionary<Stat, float> calculatedStats)
    {
        StatContainerManager.GenerateStatContainers(calculatedStats, statContainersParent);

    }

    public void Select()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one * 1.075f, .3f).setEase(LeanTweenType.easeInOutSine);
    }

    public void Deselect()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, .3f);
    }
}
