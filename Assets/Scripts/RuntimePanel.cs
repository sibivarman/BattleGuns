using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuntimePanel : MonoBehaviour
{

    [SerializeField]
    private GameObject floatingJoystick;

    [SerializeField]
    private TextMeshProUGUI waveNoText;

    [SerializeField]
    private PowerUpProgressBar powerUpProgressBar;

    [SerializeField]
    private Image progressBar;

    [SerializeField]
    private TextMeshProUGUI coinsText;

    [SerializeField]
    private Player player;

    private Coroutine progressBarCoroutine;

    private int coins;

    private void OnEnable()
    {
        floatingJoystick.SetActive(true);
    }

    public void SetWaveNo(int waveNo,int totalNoOfWaves)
    {
        waveNoText.text = "Wave : " + waveNo + "/"+totalNoOfWaves;
    }

    public void AddCoins(int _coins)
    {
        coins += _coins;
        coinsText.text = ""+coins;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void ResetRuntimePanel()
    {
        coins = 0;
        coinsText.text = "0";
        progressBar.fillAmount = 0;
        waveNoText.text = "";
    }

    public void SetRuntimeProgressBar(float progressValue)
    {
        if (progressBarCoroutine != null)
            StopCoroutine(progressBarCoroutine);

        progressBarCoroutine = StartCoroutine(fillProgressBar(progressValue));
    }

    private IEnumerator fillProgressBar(float progressValue)
    {
        float diffValue = progressValue - 0.001f;//This value was used to stop the coroutine running for more times to reach the exact value increasing small values each time
        while(progressBar.fillAmount < diffValue)
        {
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, progressValue, Time.deltaTime);
            yield return null;
        }
        progressBar.fillAmount = progressValue;
    }

    public void EnablePlayer()
    {
        player.EnablePlayer();
    }

    private void OnDisable()
    {
        //OnPointerUp is called to disable the joystick OnDrag during loading screen
        floatingJoystick.GetComponent<FloatingJoystick>().OnPointerUp(null);
        floatingJoystick.SetActive(false);
    }

}
