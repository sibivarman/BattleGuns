using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private StartPanel startPanel;

    [SerializeField]
    private GameObject runtimePanel;

    [SerializeField]
    private GameObject loadingPanel;

    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private DefeatPanel defeatPanel;

    private int enabledLevel;

    public int GetEnabledLevels()
    {
        enabledLevel = gamePlayManager.GetGameLevel();
        return enabledLevel;
    }

    public void EnableRuntimePanel(bool flag = true)
    {
        runtimePanel.SetActive(flag);
    }

    public void EnableLoadingPanel(bool flag = true)
    {
        loadingPanel.SetActive(flag);
    }

    public void EnableWinPanel(bool flag = true)
    {
        winPanel.SetActive(flag);
    }

    public void EnableDefeatPanel(bool flag = true)
    {
        defeatPanel.gameObject.SetActive(flag);
    }

    public void UpdateHealthButton(int healthLvl, int healthUpgradeCost, int availableCoins)
    {
        startPanel.UpdateHealthButton(healthLvl, healthUpgradeCost, availableCoins);
        defeatPanel.UpdateHealthButton(healthLvl, healthUpgradeCost, availableCoins);
    }

    public void UpdateFireButton(int fireLvl, int fireUpgradeCost, int availableCoins)
    {
        startPanel.UpdateFireButton(fireLvl, fireUpgradeCost, availableCoins);
        defeatPanel.UpdateFireButton(fireLvl, fireUpgradeCost, availableCoins);
    }

    public void UpdateCoinButton(float coinMultiplierLvl, int coinMultiplierUpgradeCost, int availableCoins)
    {
        startPanel.UpdateCoinMultiplierButton(coinMultiplierLvl, coinMultiplierUpgradeCost, availableCoins);
        defeatPanel.UpdateCoinMultiplierButton(coinMultiplierLvl, coinMultiplierUpgradeCost, availableCoins);
    }
}
