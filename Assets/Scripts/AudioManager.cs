using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static bool isSoundEnabled;

    [SerializeField]
    private AudioSource buttonSound;

    [SerializeField]
    private AudioSource powerupSound;

    [SerializeField]
    private AudioSource receiveCoinSound;

    private void Awake()
    {
        if (ES3.KeyExists("Audio"))
        {
            isSoundEnabled = ES3.Load<bool>("Audio");
        }
        else
        {
            ES3.Save<bool>("Audio", true);
            isSoundEnabled = true;
        }
    }

    public void isAudioEnabled(bool _isEnabled)
    {
        isSoundEnabled = _isEnabled;
        ES3.Save<bool>("Audio", isSoundEnabled);
    }

    public void PlayButtonSound()
    {
        if (isSoundEnabled)
        {
            buttonSound.Play();
        }
    }

    public void PlayPowerUpBtnSound()
    {
        if (isSoundEnabled)
        {
            powerupSound.Play();
        }
    }

    public void PlayReceiveCoinSound()
    {
        if (isSoundEnabled)
        {
            receiveCoinSound.Play();
        }
    }
}
