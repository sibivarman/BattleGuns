using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject playerGO;

    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private Level[] levels;

    [SerializeField]
    private GameObject[] spawnPatterns;

    [SerializeField]
    private HealthBarManager healthBarManager;

    [SerializeField]
    private PointerManager pointerManager;

    [SerializeField]
    private GameObject diskPrefab;

    [SerializeField]
    private GameObject locGlowPrefab;

    [SerializeField]
    private int closeMeidumPossibilitySplit;

    [SerializeField]
    private int mediumLongPossibilitySplit;

    [SerializeField]
    private RuntimePanel runtimePanel;

    [SerializeField]
    private CoinPoolManager coinPoolManager;

    [SerializeField]
    private StressReceiver stressReceiver;

    [SerializeField]
    private float stress;

    private int currLevelNo;

    private int currWaveNo;

    private List<GameObject> enemiesList = new List<GameObject>();

    private GameObject currDisk;

    private bool[][] spawnPositionTaken = new bool[3][];

    private int[] spawnPositionCount = new int[3];

    private bool isWaveGeneratedEnemies;

    private Coroutine enemyGenerateCoroutine;

    private Coroutine enableEnemyCoroutine;

    private float totalEnemyCoutnForLevel;

    private float destroyedEnemyCount;

    private GameObject skeletonHandPrefab;

    void Start()
    {
        currLevelNo = gamePlayManager.GetGameLevel() - 1;
    }

    public void GenerateEnemies(int _level)
    {
        currLevelNo = _level;

        if (currWaveNo < levels[currLevelNo].waves.Length)
        {
            //Increase Wave No in Runtime Panel
            runtimePanel.SetWaveNo(currWaveNo + 1, levels[currLevelNo].waves.Length);

            //Calculate total no of enemies will be generated for this level, which will be used in the runtime progress bar
            if(currWaveNo == 0)//Do this only for first wave since this is same for all the waves
            {
                totalEnemyCoutnForLevel = 0;//reset total enemy count for level for each level
                destroyedEnemyCount = 0;//reset enemy destroyed count for each level
                for (int waveNo = 0; waveNo < levels[currLevelNo].waves.Length; waveNo++)
                {
                    totalEnemyCoutnForLevel += levels[currLevelNo].waves[waveNo].enemies.Length;
                }
                totalEnemyCoutnForLevel *= 2;
            }

            //choose minimum no of enemies required
            List<GameObject> enemiesPrefabList = new List<GameObject>();
            int noOfEnemies = levels[currLevelNo].waves[currWaveNo].enemies.Length;
            for (int i = 0; i < noOfEnemies; i++)
            {
                if(enemiesPrefabList.Count < levels[currLevelNo].waves[currWaveNo].NoOfEnemies && levels[currLevelNo].waves[currWaveNo].enemies[i].maxOccurance > 0)
                {
                    GameObject enemy = Resources.Load<GameObject>(levels[currLevelNo].waves[currWaveNo].enemies[i].enemy);
                    enemiesPrefabList.Add(enemy);
                    enemiesPrefabList.Add(enemy);
                }
            }

            //choose additional enemies to fill total no of enemies required or max no of each individual enemies
            int[] maxNoOfTimesEnemyOccurance = new int[levels[currLevelNo].waves[currWaveNo].enemies.Length];
            int availableEnemyExtraOccuranceCount = 0;
            for(int i = 0; i < maxNoOfTimesEnemyOccurance.Length; i++)
            {
                maxNoOfTimesEnemyOccurance[i] = levels[currLevelNo].waves[currWaveNo].enemies[i].maxOccurance;
                if(levels[currLevelNo].waves[currWaveNo].enemies[i].minOccurance > 0)
                    availableEnemyExtraOccuranceCount += (maxNoOfTimesEnemyOccurance[i] - 1);
                else
                    availableEnemyExtraOccuranceCount += (maxNoOfTimesEnemyOccurance[i]);
            }
            if (enemiesPrefabList.Count < levels[currLevelNo].waves[currWaveNo].NoOfEnemies)
            {
                int remainingEnemyCount = levels[currLevelNo].waves[currWaveNo].NoOfEnemies - enemiesPrefabList.Count;
                remainingEnemyCount /= 2;
                //This loop runs for no of enemies remaining to be generated to reach the total no of enemies required, but can be breaked inside
                //if each individual enemy is genereted to its own max occurance limit
                for(int i = 0; i < remainingEnemyCount; i++)
                {
                    int enemyNo = Random.Range(0, levels[currLevelNo].waves[currWaveNo].enemies.Length);
                    if(maxNoOfTimesEnemyOccurance[enemyNo] > 1)
                    {
                        GameObject enemy = Resources.Load<GameObject>(levels[currLevelNo].waves[currWaveNo].enemies[enemyNo].enemy);
                        enemiesPrefabList.Add(enemy);
                        enemiesPrefabList.Add(enemy);
                        maxNoOfTimesEnemyOccurance[enemyNo]--;
                        availableEnemyExtraOccuranceCount--;
                    }
                    else
                    {
                        //An extra loop is needed since no enemy is added in this loop
                        i--;

                        //Break the loop if all the enemies are generated for max no of occurance time
                        if (availableEnemyExtraOccuranceCount == 0)
                            break;
                    }
                }
            }

            //Remove this code if needed, this is used to show the list of enemies in console
            //for(int i = 0; i < enemiesPrefabList.Count; i++)
            //{
            //    Debug.Log(enemiesPrefabList[i].name);
            //}

            //select spawn places for the enemies
            InitializeSpawnPostions();
            GameObject[][] enemySpawnPositions = new GameObject[enemiesPrefabList.Count/2][];
            for (int i = 0,j = 0; i < enemiesPrefabList.Count; i += 2,j++)
            {
                EnemyRangeType enemyRangeType = enemiesPrefabList[i].GetComponent<Enemy>().GetRangeType();
                enemySpawnPositions[j] = GetSpawnPosition(enemyRangeType);
            }

            //Spawn the enemies
            if (enemyGenerateCoroutine != null)
            {
                //stop enemy generate coroutnine if it is running before the next enemy generation coroutine is started
                StopCoroutine(enemyGenerateCoroutine);
            }
            //if the enemies are created in second or more wave than a glow effect has to be created, it is done here or else if the enemy is creted first time
            //it is created directly in the else part. The coroutine is used to delay the enemy creation until the glow disappears, so 0 is passed as value in initial enemy creation
            if (isWaveGeneratedEnemies)
            {
                isWaveGeneratedEnemies = false;
                Vector3 loc;

                for(int i = 0; i < enemySpawnPositions.Length; i++)
                {
                    loc = enemySpawnPositions[i][0].transform.position;
                    loc.y = 0.55f;
                    GameObject locGlow1 = Instantiate(locGlowPrefab, loc, locGlowPrefab.transform.rotation);
                    Destroy(locGlow1, 8f);

                    loc = enemySpawnPositions[i][1].transform.position;
                    loc.y = 0.55f;
                    GameObject locGlow2 = Instantiate(locGlowPrefab, loc, locGlowPrefab.transform.rotation);
                    Destroy(locGlow2, 8f);
                }
                enemyGenerateCoroutine = StartCoroutine(SpawnEnemyCoroutine(enemySpawnPositions, enemiesPrefabList));
                enableEnemyCoroutine = StartCoroutine(EnableSpawnedEnemy(5));
            }
            else
            {
                SpawnEnemies(enemySpawnPositions, enemiesPrefabList);
            }

            if (currWaveNo < levels[currLevelNo].NextWaveTime.Length)
            {
                isWaveGeneratedEnemies = true;
                Invoke("GenerateEnemies", levels[currLevelNo].NextWaveTime[currWaveNo]);
            }
            currWaveNo++;
        }
    }

    private void GenerateEnemies()
    {
        GenerateEnemies(currLevelNo);
    }

    private void InitializeSpawnPostions()
    {
        spawnPositionTaken[0] = new bool[spawnPatterns[levels[currLevelNo].spawnPatternIndex].transform.GetChild(0).childCount];
        spawnPositionTaken[1] = new bool[spawnPatterns[levels[currLevelNo].spawnPatternIndex].transform.GetChild(1).childCount];
        spawnPositionTaken[2] = new bool[spawnPatterns[levels[currLevelNo].spawnPatternIndex].transform.GetChild(2).childCount];

        spawnPositionCount[0] = spawnPositionTaken[0].Length;
        spawnPositionCount[1] = spawnPositionTaken[1].Length;
        spawnPositionCount[2] = spawnPositionTaken[2].Length;

    }

    private GameObject[] GetSpawnPosition(EnemyRangeType enemyRangeType)
    {

        switch (enemyRangeType)
        {
            case EnemyRangeType.LONG:
                {
                    if(spawnPositionCount[2] == 0)
                    {
                        return GetSpawnPosition(EnemyRangeType.MEDIUM);
                    }
                    else
                    {
                        int randomSpawnPairIndex = Random.Range(0, spawnPositionTaken[2].Length);
                        while (spawnPositionTaken[2][randomSpawnPairIndex])
                        {
                            randomSpawnPairIndex = Random.Range(0, spawnPositionTaken[2].Length);
                        }
                        spawnPositionTaken[2][randomSpawnPairIndex] = true;
                        spawnPositionCount[2]--;
                        Transform spawnPairTransform = spawnPatterns[levels[currLevelNo].spawnPatternIndex].transform.GetChild(2).transform.GetChild(randomSpawnPairIndex).transform;
                        return new GameObject[] { spawnPairTransform.GetChild(0).gameObject, spawnPairTransform.GetChild(1).gameObject };
                    }
                }
            case EnemyRangeType.MEDIUM:
                {
                    if(spawnPositionCount[1] == 0)
                    {
                        if (spawnPositionCount[2] > 0)
                            return GetSpawnPosition(EnemyRangeType.LONG);
                        else if(spawnPositionCount[0] > 0)
                            return GetSpawnPosition(EnemyRangeType.CLOSE);
                        else
                        {
                            Debug.LogError("Enemy Count Higher than Total no of spawn position available, returning two generic positions for spawning");
                            return new GameObject[] { new GameObject(), new GameObject() };
                        }
                    }
                    else
                    {
                        int randomSpawnPairIndex = Random.Range(0, spawnPositionTaken[1].Length);
                        while (spawnPositionTaken[1][randomSpawnPairIndex])
                        {
                            randomSpawnPairIndex = Random.Range(0, spawnPositionTaken[1].Length);
                        }
                        spawnPositionTaken[1][randomSpawnPairIndex] = true;
                        spawnPositionCount[1]--;
                        Transform spawnPairTransform = spawnPatterns[levels[currLevelNo].spawnPatternIndex].transform.GetChild(1).transform.GetChild(randomSpawnPairIndex).transform;
                        return new GameObject[] { spawnPairTransform.GetChild(0).gameObject, spawnPairTransform.GetChild(1).gameObject };
                    }
                }
            case EnemyRangeType.CLOSE:
                {
                    if(spawnPositionCount[0] == 0)
                    {
                        return GetSpawnPosition(EnemyRangeType.MEDIUM);
                    }
                    else
                    {
                        int randomSpawnPairIndex = Random.Range(0, spawnPositionTaken[0].Length);
                        while (spawnPositionTaken[0][randomSpawnPairIndex])
                        {
                            randomSpawnPairIndex = Random.Range(0, spawnPositionTaken[0].Length);
                        }
                        spawnPositionTaken[0][randomSpawnPairIndex] = true;
                        spawnPositionCount[0]--;
                        Transform spawnPairTransform = spawnPatterns[levels[currLevelNo].spawnPatternIndex].transform.GetChild(0).transform.GetChild(randomSpawnPairIndex).transform;
                        return new GameObject[] { spawnPairTransform.GetChild(0).gameObject, spawnPairTransform.GetChild(1).gameObject };
                    }
                }
            case EnemyRangeType.CLOSE_MEDIUM:
                {
                    int closeMediumPossibility = Random.Range(0, 100);
                    if(closeMediumPossibility < closeMeidumPossibilitySplit)
                    {
                        return GetSpawnPosition(EnemyRangeType.CLOSE);
                    }
                    else
                    {
                        return GetSpawnPosition(EnemyRangeType.MEDIUM);
                    }
                }
            case EnemyRangeType.MEDIUM_LONG:
                {
                    int mediumLongPossibility = Random.Range(0, 100);
                    if(mediumLongPossibility < mediumLongPossibilitySplit)
                    {
                        return GetSpawnPosition(EnemyRangeType.MEDIUM);
                    }
                    else
                    {
                        return GetSpawnPosition(EnemyRangeType.LONG);
                    }
                }
        }
        return new GameObject[] { new GameObject(), new GameObject()};
    }

    private IEnumerator SpawnEnemyCoroutine(GameObject[][] enemySpawnPositions, List<GameObject> enemiesPrefabList)
    {
        for (int i = 0, j = 0; j < enemySpawnPositions.Length; i += 2, j++)
        {
            //spawn the enemies
            GameObject enemy1 = Instantiate(enemiesPrefabList[i], enemySpawnPositions[j][0].transform.position, enemiesPrefabList[i].transform.rotation);
            GameObject enemy2 = Instantiate(enemiesPrefabList[i + 1], enemySpawnPositions[j][1].transform.position, enemiesPrefabList[i].transform.rotation);

            //disable enemies, it will be enabled at the spawn time
            enemy1.SetActive(false);
            enemy2.SetActive(false);

            //adding enemies to list for detecting closest enemy
            enemiesList.Add(enemy1);
            enemiesList.Add(enemy2);

            Enemy enemy1Script = enemy1.GetComponent<Enemy>();
            Enemy enemy2Script = enemy2.GetComponent<Enemy>();

            //set target for enemies
            //enemy1.GetComponent<AIDestinationSetter>().target = playerGO.transform;
            //enemy2.GetComponent<AIDestinationSetter>().target = playerGO.transform;

            enemy1Script.SetTargetPlayer(playerGO.GetComponent<Player>());
            enemy2Script.SetTargetPlayer(playerGO.GetComponent<Player>());

            //set enemy generator(this object) in the enemies, which the enemy use to tell when it is dead, So the progress bar can be updated
            enemy1Script.SetEnemyGenerator(this);
            enemy2Script.SetEnemyGenerator(this);

            //Creates a health bar for this enemy under canvas and returs the referece to the enemy
            enemy1Script.SetHealthBar(healthBarManager.GetHealthBar(enemy1.transform));
            enemy2Script.SetHealthBar(healthBarManager.GetHealthBar(enemy2.transform));

            //Creates pointer to the enemy
            enemy1Script.SetPointer(pointerManager.CreatePointer(playerGO.transform, enemy1.transform));
            enemy2Script.SetPointer(pointerManager.CreatePointer(playerGO.transform, enemy2.transform));

            yield return null;
        }
    }

    private IEnumerator EnableSpawnedEnemy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        foreach(GameObject enemy in enemiesList)
        {
            enemy.SetActive(true);
        }
    }

    private void SpawnEnemies(GameObject[][] enemySpawnPositions, List<GameObject> enemiesPrefabList)
    {
        for (int i = 0, j = 0; j < enemySpawnPositions.Length; i += 2, j++)
        {
            //spawn the enemies
            GameObject enemy1 = Instantiate(enemiesPrefabList[i], enemySpawnPositions[j][0].transform.position, enemiesPrefabList[i].transform.rotation);
            GameObject enemy2 = Instantiate(enemiesPrefabList[i + 1], enemySpawnPositions[j][1].transform.position, enemiesPrefabList[i].transform.rotation);

            //adding enemies to list for detecting closest enemy
            enemiesList.Add(enemy1);
            enemiesList.Add(enemy2);

            Enemy enemy1Script = enemy1.GetComponent<Enemy>();
            Enemy enemy2Script = enemy2.GetComponent<Enemy>();

            //set target for enemies
            //enemy1.GetComponent<AIDestinationSetter>().target = playerGO.transform;
            //enemy2.GetComponent<AIDestinationSetter>().target = playerGO.transform;

            enemy1Script.SetTargetPlayer(playerGO.GetComponent<Player>());
            enemy2Script.SetTargetPlayer(playerGO.GetComponent<Player>());

            //set enemy generator(this object) in the enemies, which the enemy use to tell when it is dead, So the progress bar can be updated
            enemy1Script.SetEnemyGenerator(this);
            enemy2Script.SetEnemyGenerator(this);

            //Creates a health bar for this enemy under canvas and returs the referece to the enemy
            enemy1Script.SetHealthBar(healthBarManager.GetHealthBar(enemy1.transform));
            enemy2Script.SetHealthBar(healthBarManager.GetHealthBar(enemy2.transform));

            //Creates pointer to the enemy
            enemy1Script.SetPointer(pointerManager.CreatePointer(playerGO.transform, enemy1.transform));
            enemy2Script.SetPointer(pointerManager.CreatePointer(playerGO.transform, enemy2.transform));
        }
    }

    private void SpawnEnemies(GameObject[][] enemySpawnPositions, GameObject enemiesPrefabList)
    {
        for (int i = 0, j = 0; j < enemySpawnPositions.Length; i += 2, j++)
        {
            //spawn the enemies
            GameObject enemy1 = Instantiate(enemiesPrefabList, enemySpawnPositions[j][0].transform.position, enemiesPrefabList.transform.rotation);
            GameObject enemy2 = Instantiate(enemiesPrefabList, enemySpawnPositions[j][1].transform.position, enemiesPrefabList.transform.rotation);

            //adding enemies to list for detecting closest enemy
            enemiesList.Add(enemy1);
            enemiesList.Add(enemy2);

            Enemy enemy1Script = enemy1.GetComponent<Enemy>();
            Enemy enemy2Script = enemy2.GetComponent<Enemy>();

            //set target for enemies
            //enemy1.GetComponent<AIDestinationSetter>().target = playerGO.transform;
            //enemy2.GetComponent<AIDestinationSetter>().target = playerGO.transform;

            enemy1Script.SetTargetPlayer(playerGO.GetComponent<Player>());
            enemy2Script.SetTargetPlayer(playerGO.GetComponent<Player>());

            //set enemy generator(this object) in the enemies, which the enemy use to tell when it is dead, So the progress bar can be updated
            enemy1Script.SetEnemyGenerator(this);
            enemy2Script.SetEnemyGenerator(this);

            //Creates a health bar for this enemy under canvas and returs the referece to the enemy
            enemy1Script.SetHealthBar(healthBarManager.GetHealthBar(enemy1.transform));
            enemy2Script.SetHealthBar(healthBarManager.GetHealthBar(enemy2.transform));

            //Creates pointer to the enemy
            enemy1Script.SetPointer(pointerManager.CreatePointer(playerGO.transform, enemy1.transform));
            enemy2Script.SetPointer(pointerManager.CreatePointer(playerGO.transform, enemy2.transform));
        }
    }

    public void AddSkeletonHandEnemies(GameObject[][] _skeletonHandEnemiesPos)
    {
        totalEnemyCoutnForLevel += _skeletonHandEnemiesPos.Length * 2;//Each Array has two enemies so multiplied by 2
        if (!skeletonHandPrefab)
        {
            skeletonHandPrefab = Resources.Load("skeleton hand") as GameObject;
        }
        SpawnEnemies(_skeletonHandEnemiesPos, skeletonHandPrefab);
    }

    public GameObject GetClosestEnemy()
    {
        GameObject closestEnemy = null;
        float dist = 100000000000, temp;

        //create disk to follow enemy
        if (currDisk)
        {
            currDisk.SetActive(false);
        }
        else
        {
            currDisk = Instantiate(diskPrefab);
            currDisk.SetActive(false);
        }

        for(int i = 0; i < enemiesList.Count; i++)
        {
            if (!enemiesList[i].activeSelf)
                continue;

            temp = Vector3.SqrMagnitude(enemiesList[i].transform.position - playerGO.transform.position);
            if (temp < dist)
            {
                dist = temp;
                closestEnemy = enemiesList[i];
            }
        }
        if(closestEnemy == null)
        {
            //Game Won
            return closestEnemy;//temporay fix for null exception
        }

        currDisk.SetActive(true);
        //currDisk = Instantiate(diskPrefab);
        currDisk.GetComponent<DiskFollow>().SetTarget(closestEnemy.transform);
        return closestEnemy;
    }

    public List<GameObject> GetAllEnemies()
    {
        Destroy(currDisk);
        return enemiesList;
    }

    public float GetFireLevel()
    {
        return gamePlayManager.GetFireLevel();
    }

    public void SetCurrLevelNo(int _level)
    {
        currLevelNo = _level;
    }

    public void EnemyDestroyed(GameObject _enemy)
    {
        //Vector3 pos = _enemy.transform.position;
        enemiesList.Remove(_enemy);
        destroyedEnemyCount++;
        runtimePanel.SetRuntimeProgressBar(destroyedEnemyCount / totalEnemyCoutnForLevel);
        //Debug.Log("Destroyed Enemy Count: " + destroyedEnemyCount);
        //Debug.Log("total Enemy Count: " + totalEnemyCoutnForLevel);
        if (enemiesList.Count == 0 && playerGO.GetComponent<Player>().IsAlive())
        {
            ResetEnemyGeneratorState();
            gamePlayManager.GameWon();
        }
        stressReceiver.InduceStress(stress);
    }

    public void UpdateCoins(int coin)
    {
        runtimePanel.AddCoins(coin);
    }

    public void CreateCoinBlastAt(Vector3 pos)
    {
        coinPoolManager.CreateCoinBlastAt(new Vector3(pos.x, 0.55f, pos.z), 8);
    }

    public void DestroyAllEnemiesAndResetGeneration()
    {
    }

    public void DestroyAllEnemiesUIComponent()
    {
        ResetEnemyGeneratorState();
    }

    private void ResetEnemyGeneratorState()
    {
        //Reset Enemy Generator Properties to initial state
        currWaveNo = 0;//Reset currWaveNo since the game is won, Next level wave should start from 0
        isWaveGeneratedEnemies = false;//The first enemy has to generated instantly during the start of the game
        enemiesList.Clear();
        healthBarManager.ClearEnemyHealthBar();
        pointerManager.ClearEnemyPointers();
        CancelInvoke();
    }

    public void ResetPlayerTarget()
    {
        playerGO.GetComponent<Player>().ResetCurrEnemy();
    }
}

[System.Serializable]
public class Level
{
    public Waves[] waves;
    public int[] NextWaveTime;
    public int spawnPatternIndex;

    [System.Serializable]
    public class Waves
    {
        public Enemies[] enemies;
        public int NoOfEnemies;

        [System.Serializable]
        public class Enemies
        {
            public string enemy;
            public int minOccurance;
            public int maxOccurance;
        }
    }


}
