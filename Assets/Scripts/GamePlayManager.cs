using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

public class GamePlayManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] objectsNotToDestroyOnLoad;

    [SerializeField]
    private EnemyGenerator enemyGenerator;

    [SerializeField]
    private GameObject playerGO;

    [SerializeField]
    private PowerUpGenerator powerUpGenerator;

    [SerializeField]
    private AudioClip[] audioClips;

    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private bool deleteSavedValues;

    private AudioSource audioSource;

    private int currMusic;

    private GameObject floatingJoyStick;

    private Player player;

    #region PROPERTIES
    private int levelNo;

    private int coinsAvailable;

    private int healthLvl;

    private int healthUpgradeCost;

    private int fireLvl;

    private int fireUpgradeCost;

    private float coinMultiplierLvl;

    private int coinMultiplierUpgradeCost;
    #endregion

    private int currLevelNo;

    private GameObject door;

    private void Awake()
    {
        foreach(GameObject objectNotToDestroyOnLoad in objectsNotToDestroyOnLoad)
        {
            DontDestroyOnLoad(objectNotToDestroyOnLoad);
        }

        //Load Game Saved Values
        LoadGameState();

        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneIndex);
        menuManager.EnableRuntimePanel(false);
        menuManager.EnableLoadingPanel();
        while (!loadingScene.isDone)
        {
            yield return null;
        }
        if(sceneIndex > 1)
        {
            powerUpGenerator.StartPowerUpGeneration();
            enemyGenerator.GenerateEnemies(sceneIndex - 2);
            menuManager.EnableRuntimePanel();
        }
        else
        {
            powerUpGenerator.CancelInvoke();
        }
        menuManager.EnableLoadingPanel(false);
        door = GameObject.FindGameObjectWithTag("Door");

        //Check if the new loaded scene has skeleton hand, if it has then assign to the enemy generator
        if (door.GetComponent<Door>().HasSkeletonHandObstacle())
        {
            enemyGenerator.AddSkeletonHandEnemies(door.GetComponent<Door>().GetSkeletionHands());
        }
    }

    void Start()
    {
        player = playerGO.GetComponent<Player>();
#if UNITY_IOS
        NotificationServices.RegisterForNotifications(
            NotificationType.Alert |
            NotificationType.Badge);
#endif
        NotificationService();
    }

    public void PlayMusic()
    {
        GetComponent<AudioSource>().clip = audioClips[currMusic];
        GetComponent<AudioSource>().Play();
        Invoke("PlayMusic", GetComponent<AudioSource>().clip.length);
        if (currMusic == 0)
        {
            currMusic = 1;
        }
        else
        {
            currMusic = 0;
        }
    }

    public void StopMusic()
    {
        currMusic = 0;
        GetComponent<AudioSource>().Stop();
        CancelInvoke();
    }

    public void StartGame(int _levelNo)
    {
        currLevelNo = _levelNo;
        StopMusic();
        menuManager.EnableRuntimePanel();
        if(_levelNo == 51)
            door.GetComponent<Door>().SetNextLevel(50);
        else
            door.GetComponent<Door>().SetNextLevel(_levelNo + 1);
        //door.GetComponent<Door>().SetNextLevel(3);
        door.GetComponent<Door>().OpenExitDoor();
        player.EnablePlayer();

        TinySauce.OnGameStarted(currLevelNo+"");
    }

    public void ContinueGame(int level)
    {
        if (level == 0)
            level = levelNo;
        currLevelNo = level;
        cameraController.enabled = true;
        player.GetComponent<CharacterController>().enabled = false;
        StartCoroutine(LoadSceneAsync(level));
        player.transform.position = new Vector3(0, 1.05f, -3.25f);
        player.transform.rotation = Quaternion.identity;
        player.GetComponent<CharacterController>().enabled = true;

        TinySauce.OnGameStarted(currLevelNo + "");
    }

    public void LoadMenuScene()
    {
        cameraController.ResetCameraPos();
        cameraController.enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        StartCoroutine(LoadSceneAsync(1));
        player.transform.position = new Vector3(0, 1.05f, 2);
        player.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        player.GetComponent<CharacterController>().enabled = true;

        TinySauce.OnGameFinished(currLevelNo + "", coinsAvailable);
    }

    public void ReloadCurrScene()
    {
        ContinueGame(currLevelNo);
    }

    private void LoadGameState()
    {
        if (ES3.KeyExists("isAlreadySaved") && !deleteSavedValues)
        {
            levelNo = ES3.Load<int>("levelNo");
            coinsAvailable = ES3.Load<int>("coinsAvailable");
            healthLvl = ES3.Load<int>("healthLvl");
            healthUpgradeCost = ES3.Load<int>("healthUpgradeCost");
            fireLvl = ES3.Load<int>("fireLvl");
            fireUpgradeCost = ES3.Load<int>("fireUpgradeCost");
            coinMultiplierLvl = ES3.Load<float>("coinMultiplierLvl");
            coinMultiplierUpgradeCost = ES3.Load<int>("coinMultiplierUpgradeCost");
        }
        else
        {
            ES3.Save<bool>("isAlreadySaved", true);
            ES3.Save<int>("levelNo", 1);
            ES3.Save<int>("coinsAvailable",0);
            ES3.Save<int>("healthLvl",1);
            ES3.Save<int>("healthUpgradeCost", 200);
            ES3.Save<int>("fireLvl",1);
            ES3.Save<int>("fireUpgradeCost",200);
            ES3.Save<float>("coinMultiplierLvl",1.1f);
            ES3.Save<int>("coinMultiplierUpgradeCost",200);

            levelNo = 1;
            coinsAvailable = 0;
            healthLvl = 1;
            healthUpgradeCost = 200;
            fireLvl = 1;
            fireUpgradeCost = 200;
            coinMultiplierLvl = 1.1f;
            coinMultiplierUpgradeCost = 200;
        }
    }

    public void UpgradePlayerHealthWithParticleEfx()
    {
        player.IncreaseHealthPowerUp();
        UpgradePlayerHealth();
    }

    public void UpgradePlayerHealth()
    {
        healthLvl++;
        coinsAvailable -= healthUpgradeCost;
        healthUpgradeCost += 200;
        ES3.Save<int>("healthLvl", healthLvl);
        ES3.Save<int>("healthUpgradeCost", healthUpgradeCost);
        ES3.Save<int>("coinsAvailable", coinsAvailable);
        UpdateLevelUpBtns();
        player.UpdateHealth(healthLvl);
    }

    public void UpgradePlayerFireWithParticleEfx()
    {
        player.IncreaseFirePowerUp();
        UpgradePlayerFire();
    }

    public void UpgradePlayerFire()
    {
        fireLvl++;
        coinsAvailable -= fireUpgradeCost;
        fireUpgradeCost += 200;
        ES3.Save<int>("fireLvl", fireLvl);
        ES3.Save<int>("fireUpgradeCost", fireUpgradeCost);
        ES3.Save<int>("coinsAvailable", coinsAvailable);
        UpdateLevelUpBtns();
    }

    public void UpgradePlayerCoinMultiplierWithParticleEfx()
    {
        player.IncreaseCoinPowerUp();
        UpgradePlayerCoinMultiplier();
    }

    public void UpgradePlayerCoinMultiplier()
    {
        coinMultiplierLvl += 0.1f;
        coinsAvailable -= coinMultiplierUpgradeCost;
        coinMultiplierUpgradeCost += 250;
        ES3.Save<float>("coinMultiplierLvl", coinMultiplierLvl);
        ES3.Save<int>("coinMultiplierUpgradeCost", coinMultiplierUpgradeCost);
        ES3.Save<int>("coinsAvailable", coinsAvailable);
        UpdateLevelUpBtns();
    }

    public void UpdateLevelUpBtns()
    {
        menuManager.UpdateHealthButton(healthLvl, healthUpgradeCost, coinsAvailable);
        menuManager.UpdateFireButton(fireLvl, fireUpgradeCost, coinsAvailable);
        menuManager.UpdateCoinButton(coinMultiplierLvl, coinMultiplierUpgradeCost, coinsAvailable);
    }

    public void GameWon()
    {
        //levelNo++;

        TinySauce.OnGameFinished(currLevelNo + "", coinsAvailable);
        currLevelNo++;
        if (currLevelNo - 1 > levelNo)
        {
            ES3.Save<int>("levelNo", currLevelNo - 1);
            levelNo = currLevelNo - 1;
        }
        powerUpGenerator.CancelInvoke();
        Invoke("EnableWinPanel", 1.5f);
    }

    public void EnableWinPanel()
    {
        menuManager.EnableWinPanel();
    }

    public void OpenDoor()
    {
        door.GetComponent<Door>().OpenExitDoor();
    }

    public void GameOver()
    {
        powerUpGenerator.CancelInvoke();
        menuManager.EnableDefeatPanel();
    }

    public void UpdateCoinsAvailable(int _addCoin)
    {
        coinsAvailable += _addCoin;
        ES3.Save<int>("coinsAvailable", coinsAvailable);
    }

    public int GetCurrLevel()
    {
        return currLevelNo;
    }

    public int GetGameLevel()
    {
        return levelNo;
    }

    public int GetCoins()
    {
        return coinsAvailable;
    }

    public int GetHealthLevel()
    {
        return healthLvl;
    }

    public int GetHealthUpgradeCost()
    {
        return healthUpgradeCost;
    }

    public int GetFireLevel()
    {
        return fireLvl;
    }

    public int GetFireUpgradeCost()
    {
        return fireUpgradeCost;
    }

    public float GetCoinMultiplierLevel()
    {
        return coinMultiplierLvl;
    }

    public int GetCoinMultiplierUpgradeCost()
    {
        return coinMultiplierUpgradeCost;
    }

    private void NotificationService()
    {
#if UNITY_IOS
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
        UnityEngine.iOS.LocalNotification locNotify0 = new UnityEngine.iOS.LocalNotification();
        locNotify0.alertTitle = "Pss't Over here";
        locNotify0.alertBody = "Let's Battle some enemies";
        locNotify0.fireDate = System.DateTime.Now.AddHours(2);
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(locNotify0);
        UnityEngine.iOS.LocalNotification locNotify1 = new UnityEngine.iOS.LocalNotification();
        locNotify1.alertTitle = "Pss't Over here";
        locNotify1.alertBody = "Battle the enemies!!!!!!!!!!!";
        locNotify1.fireDate = System.DateTime.Now.AddDays(1);
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(locNotify1);
        UnityEngine.iOS.LocalNotification locNotify2 = new UnityEngine.iOS.LocalNotification();
        locNotify2.alertTitle = "Pss't Over here";
        locNotify2.alertBody = "It's time to kill the enemies";
        locNotify2.fireDate = System.DateTime.Now.AddDays(2);
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(locNotify2);
        UnityEngine.iOS.LocalNotification locNotify3 = new UnityEngine.iOS.LocalNotification();
        locNotify3.alertTitle = "Pss't Over here";
        locNotify3.alertBody = "Battle! Guns!";
        locNotify3.fireDate = System.DateTime.Now.AddDays(3);
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(locNotify3);
#endif
    }
}
