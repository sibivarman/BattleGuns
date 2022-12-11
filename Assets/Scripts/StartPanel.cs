using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class StartPanel : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

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
    private TextMeshProUGUI coinText;

    [SerializeField]
    private TextMeshProUGUI levelText;

    private int closeAnimationHash = Animator.StringToHash("Close Animation");

    private int levelNo;

    private void OnEnable()
    {
        UpdateAllButtons();

        if (ES3.KeyExists("Audio"))
        {
            if (ES3.Load<bool>("Audio"))
            {
                gamePlayManager.PlayMusic();
            }
        }
        else
        {
            gamePlayManager.PlayMusic();
        }
    }

    private void Start()
    {
        UpdateAllButtons();
    }

    public void UpdateAllButtons()
    {
        int availableCoins = gamePlayManager.GetCoins();
        coinText.text = "" + availableCoins;
        levelText.text = "Level " + gamePlayManager.GetGameLevel();
        UpdateHealthButton(gamePlayManager.GetHealthLevel(), gamePlayManager.GetHealthUpgradeCost(), availableCoins);
        UpdateFireButton(gamePlayManager.GetFireLevel(), gamePlayManager.GetFireUpgradeCost(), availableCoins);
        UpdateCoinMultiplierButton(gamePlayManager.GetCoinMultiplierLevel(), gamePlayManager.GetCoinMultiplierUpgradeCost(), availableCoins);
    }

    public void UpdateHealthButton(int healthLvl, int healthCost, int availableCoins)
    {
        bool isInteractable = availableCoins >= healthCost;
        healthButton.interactable = isInteractable;
        healthUpdateArrow.SetActive(isInteractable);

        coinText.text = "" + availableCoins;
        healthLevelText.text = "lvl " + healthLvl;
        healthCostText.text = "" + healthCost;
    }

    public void UpdateFireButton(int fireLvl, int fireCost, int availableCoins)
    {
        bool isInteractable = availableCoins >= fireCost;
        fireButton.interactable = isInteractable;
        fireUpdateArrow.SetActive(isInteractable);

        coinText.text = "" + availableCoins;
        fireLevelText.text = "lvl " + fireLvl;
        fireCostText.text = "" + fireCost;
    }

    public void UpdateCoinMultiplierButton(float coinLvl, int coinCost, int availableCoins)
    {
        bool isInteractable = availableCoins >= coinCost;
        coinButton.interactable = isInteractable;
        coinUpdateArrow.SetActive(isInteractable);

        coinText.text = "" + availableCoins;
        coinLevelText.text = coinLvl + "x";
        coinCostText.text = "" + coinCost;
    }

    public void StartGame()
    {
        levelNo = gamePlayManager.GetGameLevel();
        //This animation completes and call DisablePanel function from animation
        GetComponent<Animator>().SetTrigger(closeAnimationHash);
    }

    public void StartGame(int level)
    {
        levelNo = level;
        //This animation completes and call DisablePanel function from animation
        GetComponent<Animator>().SetTrigger(closeAnimationHash);
    }

    public void DisablePanel()
    {
        //This function is part of StartGame funciton, this function will be called after completing animation in startgame function
        gamePlayManager.StartGame(levelNo);
        gameObject.SetActive(false);
    }
}
