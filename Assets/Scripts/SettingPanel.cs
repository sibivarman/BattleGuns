using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{

    [SerializeField]
    private GameObject onButton;

    [SerializeField]
    private GameObject offButton;

    private void OnEnable()
    {
        onButton.SetActive(AudioManager.isSoundEnabled);
        offButton.SetActive(!AudioManager.isSoundEnabled);
    }
}
