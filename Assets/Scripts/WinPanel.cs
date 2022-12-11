using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinPanel : MonoBehaviour
{

    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private RuntimePanel runtimePanel;

    [SerializeField]
    private TextMeshProUGUI collectedCoinCount;

    [SerializeField]
    private TextMeshProUGUI totalCoinCount;

    [SerializeField]
    private TextMeshProUGUI levelCompletedText;

    [SerializeField]
    private float collectedCoinCountDuration;

    [SerializeField]
    private float multiplierCoinCountDuration;

    [SerializeField]
    private AudioSource winSound;

    [SerializeField]
    private AudioSource coinSound;

    [SerializeField]
    private float winSoundDelay;

    [SerializeField]
    private TextMeshProUGUI multiplierText;

    [SerializeField]
    private GameObject shareScreen;

    [SerializeField]
    private GameObject commingSoonScreen;

    private Coroutine collectedCoinCountCoroutine;

    private Coroutine multipliedCoinCountCoroutine;

    private int totalCoin;

    private float coinMultiplierLevel;

    private bool isShareScreenEnabled;

    private void OnEnable()
    {
        isShareScreenEnabled = false;
        runtimePanel.gameObject.SetActive(false);
        totalCoin = runtimePanel.GetCoins();
        runtimePanel.ResetRuntimePanel();
        coinMultiplierLevel = gamePlayManager.GetCoinMultiplierLevel();
        multiplierText.text = "x" + coinMultiplierLevel;
        levelCompletedText.text = "" + (gamePlayManager.GetCurrLevel() - 2);
        gamePlayManager.UpdateCoinsAvailable((int)(totalCoin * coinMultiplierLevel));
        if (AudioManager.isSoundEnabled)
        {
            winSound.Play();
        }
    }

    private void CountCollectedCoin()
    {
        if (collectedCoinCountCoroutine != null)
            StopCoroutine(collectedCoinCountCoroutine);
        collectedCoinCountCoroutine = StartCoroutine("CollectedCoinCount");
    }

    IEnumerator CollectedCoinCount()
    {
        float time = 0, lastTime = Time.time;
        for(float timeElapsed = 0; timeElapsed < collectedCoinCountDuration; timeElapsed += Time.deltaTime)
        {
            collectedCoinCount.text = "" + (int)(totalCoin * (timeElapsed / collectedCoinCountDuration));
            totalCoinCount.text = "" + (int)(totalCoin * (timeElapsed / collectedCoinCountDuration));
            if (AudioManager.isSoundEnabled && time > winSoundDelay)
            {
                time = 0;
                coinSound.Play();
            }
            time += (Time.time - lastTime);
            lastTime = Time.time;
            yield return null;
        }
        collectedCoinCount.text = "" + totalCoin;
        totalCoinCount.text = "" + totalCoin;
    }

    private void CountMultiplierTotalCoin()
    {
        if (collectedCoinCountCoroutine != null)
            StopCoroutine(collectedCoinCountCoroutine);
        multipliedCoinCountCoroutine = StartCoroutine("MultipliedCoinCount");
    }

    IEnumerator MultipliedCoinCount()
    {
        int coinDiff = (int)(totalCoin * coinMultiplierLevel) - totalCoin;
        for (float timeElapsed = 0; timeElapsed < multiplierCoinCountDuration && coinDiff > 10; timeElapsed += Time.deltaTime)
        {
            totalCoinCount.text = "" + (int)(totalCoin + (coinDiff * (timeElapsed / multiplierCoinCountDuration)));
            yield return null;
        }
        totalCoinCount.text = "" + (int)(totalCoin * coinMultiplierLevel);
    }

    public void EnableShareScreen()
    {
        if((gamePlayManager.GetCurrLevel() - 2) % 10 == 0 && (gamePlayManager.GetCurrLevel() - 2) != 50)
        {
            isShareScreenEnabled = true;
            shareScreen.SetActive(true);
        }
        else if((gamePlayManager.GetCurrLevel() - 2) == 50)
        {
            commingSoonScreen.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (!isShareScreenEnabled)
        {
            runtimePanel.gameObject.SetActive(true);
        }
    }
}
