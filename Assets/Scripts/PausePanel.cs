using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{

    [SerializeField]
    private GameObject volumeOnButton;

    [SerializeField]
    private GameObject volumeOffButton;

    [SerializeField]
    private GamePlayManager gamePlayManager;

    private void OnEnable()
    {
        volumeOnButton.SetActive(AudioManager.isSoundEnabled);
        volumeOffButton.SetActive(!AudioManager.isSoundEnabled);
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    public void LoadMenuScene()
    {
        //SceneManager.LoadScene(1);
        gamePlayManager.LoadMenuScene();
    }
}
