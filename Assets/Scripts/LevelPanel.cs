using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private Button[] levelButtons;

    [SerializeField]
    private StartPanel startPanel;
    
    private int closePanelAnimationHash;

    private int enabledLevels;

    private void Start()
    {
        closePanelAnimationHash = Animator.StringToHash("CloseLevelPanel");
    }

    private void OnEnable()
    {
        enabledLevels = menuManager.GetEnabledLevels();
        EnableDisableLevelButtons();
    }

    public void CloseLevelPanel()
    {
        GetComponent<Animator>().SetTrigger(closePanelAnimationHash);
    }

    public void DisableLevelPanel()
    {
        gameObject.SetActive(false);
    }

    public void EnableDisableLevelButtons()
    {
        for(int enabledLevelCount = 0; enabledLevelCount < enabledLevels; enabledLevelCount++)
        {
            levelButtons[enabledLevelCount].interactable = true;
        }
    }

    public void PlayLevel(int level)
    {
        CloseLevelPanel();
        startPanel.StartGame(level);
    }
}
