using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class HealthBar : MonoBehaviour
{

    [SerializeField]
    private Transform firstBar;

    [HideIf("isPlayer")]
    [SerializeField]
    private Transform secondBar;

    [SerializeField]
    private Image bgImage;

    [SerializeField]
    private float barFillSpeed;

    [SerializeField]
    private bool isPlayer;

    [ShowIf("isPlayer")]
    [SerializeField]
    private Transform target;

    [ShowIf("isPlayer")]
    [SerializeField]
    private TextMeshProUGUI healthText;

    private Vector3 offsetHeight;

    private float totalHealth;

    private Coroutine asyncHealthBar;

    private Camera mainCamera;


    void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.position = mainCamera.WorldToScreenPoint(target.position + offsetHeight);
    }

    public void SetHealth(float health)
    {
        if (firstBar)//First Bar is checked, It will be destroyed when home btn is clicked in pause screen, which may cause null exception
        {
            Vector3 scale = firstBar.localScale;
            scale.x = health / totalHealth;
            firstBar.localScale = scale;
            if (!isPlayer)
            {
                if (asyncHealthBar != null)
                {
                    StopCoroutine(asyncHealthBar);
                }
                asyncHealthBar = StartCoroutine(AsyncHealthBar(health / totalHealth));
            }
            else
            {
                healthText.text = "" + health;
            }
        }

    }

    IEnumerator AsyncHealthBar(float progress)
    {
        while (secondBar.localScale.x > progress)
        {
            Vector3 scale = secondBar.localScale;
            scale.x -= (Time.deltaTime * barFillSpeed);
            secondBar.localScale = scale;
            yield return null;
        }
    }

    public void SetTotalHealth(float _totalHealth, float yOffset = 0)
    {
        totalHealth = _totalHealth;
        offsetHeight = new Vector3(0, yOffset, 0);
        SetHealth(totalHealth);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void HideHealthBar()
    {
        firstBar.GetComponentInChildren<Image>().enabled = false;
        secondBar.GetComponentInChildren<Image>().enabled = false;
        bgImage.enabled = false;
    }
}
