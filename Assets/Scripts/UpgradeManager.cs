using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("Player & Weapons")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private ProjectileWeapon singleShotWeapon;
    [SerializeField] private ProjectileWeapon tripleSpreadWeapon;
    [SerializeField] private AoEWeapon aoeWeapon;

    [Header("Upgrade Assets")]
    [SerializeField] private UpgradeDataSO spreadUnlockUpgrade;
    [SerializeField] private UpgradeDataSO aoeUnlockUpgrade;
    [SerializeField] private List<UpgradeDataSO> boostUpgrades;

    [Header("UI Buttons (exact 3)")]
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TextMeshProUGUI[] optionTitleTexts;
    [SerializeField] private TextMeshProUGUI[] optionDescTexts;

    private bool isSpreadUnlocked;
    private bool isAoEUnlocked;
    private Action onUpgradePicked;

    private void Awake()
    {
        // Jucatorul incepe la Level 1 doar cu Single Shot
        if (tripleSpreadWeapon != null) tripleSpreadWeapon.gameObject.SetActive(false);
        if (aoeWeapon != null) aoeWeapon.gameObject.SetActive(false);
    }

    public void ShowUpgrades(int level, Action onComplete)
    {
        onUpgradePicked = onComplete;
        List<UpgradeDataSO> choices = GenerateChoices(level);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                UpgradeDataSO upgrade = choices[i];
                optionButtons[i].gameObject.SetActive(true);
                optionTitleTexts[i].text = upgrade.title;
                optionDescTexts[i].text = upgrade.description;

                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectUpgrade(upgrade));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private List<UpgradeDataSO> GenerateChoices(int level)
    {
        List<UpgradeDataSO> result = new();

        if (level % 2 == 0)
        {
            if (!isSpreadUnlocked && spreadUnlockUpgrade != null)
                result.Add(spreadUnlockUpgrade);
            if (!isAoEUnlocked && aoeUnlockUpgrade != null)
                result.Add(aoeUnlockUpgrade);
        }

        List<UpgradeDataSO> pool = new();
        foreach (UpgradeDataSO boost in boostUpgrades)
        {
            if (boost.type == UpgradeType.SpreadExtraProjectile && !isSpreadUnlocked) continue;
            if (boost.type == UpgradeType.AoERadius && !isAoEUnlocked) continue;
            pool.Add(boost);
        }

        while (result.Count < 3 && pool.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        return result;
    }

    private void SelectUpgrade(UpgradeDataSO upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.UnlockSpread:
                isSpreadUnlocked = true;
                tripleSpreadWeapon.gameObject.SetActive(true);
                weaponController.AddWeapon(tripleSpreadWeapon);
                break;

            case UpgradeType.UnlockAoE:
                isAoEUnlocked = true;
                aoeWeapon.gameObject.SetActive(true);
                weaponController.AddWeapon(aoeWeapon);
                break;

            case UpgradeType.MoveSpeed:
                playerStats.IncreaseSpeed(upgrade.value);
                break;

            case UpgradeType.MaxHealth:
                playerStats.IncreaseMaxHealth(upgrade.value);
                break;

            case UpgradeType.SingleShotDamage:
                singleShotWeapon.UpgradeDamage(upgrade.value);
                break;

            case UpgradeType.GlobalAttackSpeed:
                singleShotWeapon.UpgradeFireRate(upgrade.value);
                if (isSpreadUnlocked) tripleSpreadWeapon.UpgradeFireRate(upgrade.value);
                if (isAoEUnlocked) aoeWeapon.UpgradeFireRate(upgrade.value);
                break;

            case UpgradeType.SpreadExtraProjectile:
                tripleSpreadWeapon.AddProjectile(Mathf.RoundToInt(upgrade.value));
                break;

            case UpgradeType.AoERadius:
                aoeWeapon.UpgradeRadius(upgrade.value);
                break;
        }

        onUpgradePicked?.Invoke();
    }
}