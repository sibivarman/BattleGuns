using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefeatPanel : MonoBehaviour
{

    [SerializeField]
    private RuntimePanel runtimePanel;

    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private TextMeshProUGUI coinCount;

    [SerializeField]
    private TextMeshProUGUI totalCoinCount;

    [SerializeField]
    private float countDuration;

    [SerializeField]
    private TextMeshProUGUI healthLevelText;

    [SerializeField]
    private TextMeshProUGUI healthCostText;

    [SerializeField]
    private Button healthButton;

    [SerializeField]
    private GameObject healthUpdateArrow;

    [SerializeField]
    private TextMeshProUGUI fireLevelText;

    [SerializeField]
    private TextMeshProUGUI fireCostText;

    [SerializeField]
    private Button fireButton;

    [SerializeField]
    private GameObject fireUpdateArrow;

    [SerializeField]
    private TextMeshProUGUI coinLevelText;

    [SerializeField]
    private TextMeshProUGUI coinCostText;

    [SerializeField]
    private Button coinButton;

    [SerializeField]
    private GameObject coinUpdateArrow;

    [SerializeField]
    private AudioSource coinSound;

    [SerializeField]
    private float winSoundDelay;

    private int collectedCoins;

    private int totalAvailableCoins;

    private Coroutine countCoinCoroutine;

    private void OnEnable()
    {
        runtimePanel.gameObject.SetActive(false);
        collectedCoins = runtimePanel.GetCoins();
        runtimePanel.ResetRuntimePanel();
        totalAvailableCoins = gamePlayManager.GetCoins();
    }

    public void CountCoins()
    {
        if (countCoinCoroutine != null)
            StopCoroutine(countCoinCoroutine);
        countCoinCoroutine = StartCoroutine(CountCoinCoroutine());
    }

    public IEnumerator CountCoinCoroutine()
    {
        int coin = 0;
        float time = 0, lastTime = Time.time;
        for (float timeElaspsed = 0; timeElaspsed < countDuration && collectedCoins >= 5 ;timeElaspsed += Time.deltaTime)
        {
            coin = (int)(collectedCoins * (timeElaspsed / countDuration));
            coinCount.text = "" + coin;
            totalCoinCount.text = "" + (totalAvailableCoins + coin);
            if (AudioManager.isSoundEnabled && time > winSoundDelay)
            {
                time = 0;
                coinSound.Play();
            }
            time += (Time.time - lastTime);
            lastTime = Time.time;
            yield return null;
        }
        coinCount.text = "" + collectedCoins;
        totalCoinCount.text = "" + (totalAvailableCoins + collectedCoins);
        gamePlayManager.UpdateCoinsAvailable(collectedCoins);
        UpdateAllButtons();
    }

    public void UpdateAllButtons()
    {
        int availableCoins = gamePlayManager.GetCoins();
        UpdateHealthButton(gamePlayManager.GetHealthLevel(), gamePlayManager.GetHealthUpgradeCost(), availableCoins);
        UpdateFireButton(gamePlayManager.GetFireLevel(), gamePlayManager.GetFireUpgradeCost(), availableCoins);
        UpdateCoinMultiplierButton(gamePlayManager.GetCoinMultiplierLevel(), gamePlayManager.GetCoinMultiplierUpgradeCost(), availableCoins);
    }

    public void UpdateHealthButton(int healthLvl, int healthCost, int availableCoins)
    {
        bool isInteractable = availableCoins >= healthCost;
        healthButton.interactable = isInteractable;
        healthUpdateArrow.SetActive(isInteractable);

        totalCoinCount.text = "" + availableCoins;
        healthLevelText.text = "lvl " + healthLvl;
        healthCostText.text = "" + healthCost;
    }

    public void UpdateFireButton(int fireLvl, int fireCost, int availableCoins)
    {
        bool isInteractable = availableCoins >= fireCost;
        fireButton.interactable = isInteractable;
        fireUpdateArrow.SetActive(isInteractable);

        totalCoinCount.text = "" + availableCoins;
        fireLevelText.text = "lvl " + fireLvl;
        fireCostText.text = "" + fireCost;
    }

    public void UpdateCoinMultiplierButton(float coinLvl, int coinCost, int availableCoins)
    {
        bool isInteractable = availableCoins >= coinCost;
        coinButton.interactable = isInteractable;
        coinUpdateArrow.SetActive(isInteractable);

        totalCoinCount.text = "" + availableCoins;
        coinLevelText.text = coinLvl + "x";
        coinCostText.text = "" + coinCost;
    }

}
