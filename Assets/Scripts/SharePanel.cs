using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharePanel : MonoBehaviour
{

    [SerializeField]
    private GameObject runtimePanel;

    public void Share()
    {
        new NativeShare().SetText("Hey, this battle gun game is awesome, check this out..... https://itunes.apple.com/app/id1504130234").Share();
    }

    private void OnDisable()
    {
        runtimePanel.SetActive(true);
    }
}
