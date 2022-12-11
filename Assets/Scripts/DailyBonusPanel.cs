using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class DailyBonusPanel : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private StartPanel startPanel;

    [SerializeField]
    private TextMeshProUGUI coinsText;

    [SerializeField]
    private TextMeshProUGUI coinsMultiplierText;

    [SerializeField]
    private TextMeshProUGUI totalCoinsText;

    [SerializeField]
    private int dailyBonusIntervalTime;

    [SerializeField]
    private float counterDuration;

    [SerializeField]
    private int totalCoins;

    [SerializeField]
    private int noOfCoinsInUIAnimation;

    [SerializeField]
    private GameObject animationCoinPrefab;

    [SerializeField]
    private Transform animationCoinTarget;

    private int coinsAvailable;

    private float coinsMultiplier;

    private void Awake()
    {
        if (ES3.KeyExists("LastRewardedTime"))
        {
            DateTime lastRewardedTime = ES3.Load<DateTime>("LastRewardedTime");
            TimeSpan timeSpan = DateTime.Now.Subtract(lastRewardedTime);
            if(timeSpan.TotalHours < dailyBonusIntervalTime)
            {
                gameObject.SetActive(false);
            }
            else
            {
                ES3.Save<DateTime>("LastRewardedTime", DateTime.Now);
            }
        }
        else
        {
            ES3.Save<DateTime>("LastRewardedTime", DateTime.Now);
        }
    }

    private void Start()
    {
        coinsMultiplier = gamePlayManager.GetCoinMultiplierLevel();
        StartCoroutine("countCoins", totalCoins);
    }

    IEnumerator countCoins(int totalCoins)
    {
        int coins;
        coinsMultiplierText.text = "x" + coinsMultiplier;
        for(float timer = 0; timer < counterDuration; timer += Time.deltaTime)
        {
            coins = (int)((timer / counterDuration) * totalCoins);
            coinsText.text = "" + coins;
            totalCoinsText.text = "" + (int)(coins * coinsMultiplier);
            yield return null;
        }
        coinsText.text = "" + (int)totalCoins;
        totalCoinsText.text = "" + (int)(totalCoins * coinsMultiplier);
    }

    public void ClaimCoins()
    {
        DoCoinCollectingUIAnimation();
        gamePlayManager.UpdateCoinsAvailable((int)(totalCoins * coinsMultiplier));
        startPanel.UpdateAllButtons();
    }

    private void DoCoinCollectingUIAnimation()
    {
        for(int animationCoinCount = 0; animationCoinCount < noOfCoinsInUIAnimation; animationCoinCount++)
        {           
            Vector3 transformPos = new Vector3(UnityEngine.Random.Range(31,71) * Screen.width * 0.01f,UnityEngine.Random.Range(30,75) * Screen.height * 0.01f,0);
            GameObject coin = Instantiate(animationCoinPrefab,transformPos,Quaternion.Euler(new Vector3(0,0,UnityEngine.Random.Range(0,360))), transform.parent);
            coin.GetComponent<UICoin>().SetTarget(animationCoinTarget);
        }
    }
}
