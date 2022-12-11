using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpProgressBar : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Image progressBarImage;

    private float totalTime = 1;

    private int closeAnimationHash = Animator.StringToHash("Close");

    private float timeLapsed;

    private void Update()
    {
        if(timeLapsed >= 0)
        {
            progressBarImage.fillAmount = timeLapsed / totalTime;
            timeLapsed -= Time.deltaTime;
        }
        else
        {
            animator.SetTrigger(closeAnimationHash);
        }
    }

    public void SetTotalTime(float time)
    {
        totalTime = time;
        timeLapsed = totalTime;
    }

    public void SetColor(Color color)
    {
        progressBarImage.color = color;
    }

    public void ClearPowerUpProgressBar()
    {
        timeLapsed = -Time.deltaTime;
        progressBarImage.fillAmount = 0;
        if (animator.gameObject.activeSelf)
        {
            animator.SetTrigger(closeAnimationHash);
        }
        DisableProgressBar();
    }

    public void DisableProgressBar()
    {
        gameObject.SetActive(false);
    }
}
