using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPoolManager : MonoBehaviour
{

    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private int noOfCoins;

    [SerializeField]
    private float coinPlacingRadius;

    [SerializeField]
    private float coinPlacingHeight;

    [SerializeField]
    private AudioSource audioSource;

    private List<GameObject> coinsList = new List<GameObject>();

    private int availableCoins;

    private void Awake()
    {
        InstantiateCoins(noOfCoins);
        availableCoins = noOfCoins;
    }

    private void Start()
    {
        //Invoke("CraeteCoinBlast", 3);
    }

    public void CraeteCoinBlast()
    {
        CreateCoinBlastAt(new Vector3(0,0.5f,0), 8);
    }

    public void CreateCoinBlastAt(Vector3 pos, int coinCount)
    {
        GameObject coin;
        Vector2 coinPos;
        if(availableCoins < coinCount)
        {
            InstantiateCoins(coinCount - availableCoins);
        }
        availableCoins -= coinCount;
        for(int count = 0; count < coinCount; count++)
        {
            coin = GetNextCoin();
            coinPos = Random.insideUnitCircle * coinPlacingRadius;
            coin.transform.position = pos + new Vector3(coinPos.x, 0, coinPos.y);
            coin.SetActive(true);
            coin.GetComponent<Coin>().Explode(pos);
        }
        if (AudioManager.isSoundEnabled)
        {
            audioSource.Play();
        }
    }

    private GameObject GetNextCoin()
    {
        GameObject coin = coinsList[0];
        coinsList.RemoveAt(0);
        return coin;
    }

    private void InstantiateCoins(int noOfCoins)
    {
        GameObject coin;
        for (int count = 0; count < noOfCoins; count++)
        {
            coin = Instantiate(coinPrefab, transform);
            coin.SetActive(false);
            coinsList.Add(coin);
            coin.GetComponent<Coin>().SetTarget(target);
        }
    }

    public void CollectCoin(GameObject coin)
    {
        availableCoins++;
        coinsList.Add(coin);
        coin.SetActive(false);
    }
}
