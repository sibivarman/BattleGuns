using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PowerUpGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] powerupPrefabs;

    [SerializeField]
    private PointerManager pointerManager;

    [SerializeField]
    private GameObject locationGlow;

    [SerializeField]
    private float expiryTime;

    [SerializeField]
    private float blinkTime;

    [SerializeField]
    private float minTime;

    [SerializeField]
    private float maxTime;

    [SerializeField]
    private int healthPowerupPrct;

    [SerializeField]
    private int spellPrct;

    [SerializeField]
    private int fireballPrct;

    [SerializeField]
    private int grenadePrct;

    [SerializeField]
    private int mediumBulletPrct;

    [SerializeField]
    private int protonPrct;

    [SerializeField]
    private int rocketPrct;

    [SerializeField]
    private int sniperPrct;

    [SerializeField]
    private GamePlayManager gamePlayManager;

    private Vector3 loc;

    private GameObject powerUp;

    private List<Vector3> locations = new List<Vector3>();

    public void StartPowerUpGeneration()
    {
        if (gamePlayManager.GetCurrLevel() <= 5)
            return;
        Invoke("StartPowerUpGenerationInvokeMethod",5);
    }

    public void StartPowerUpGenerationInvokeMethod()
    {
        int noOfPowerUps = 3;
        for(int i = 0; i < noOfPowerUps; i++)
        {
            CreateAPowerUp();
        }
        Invoke("StartPowerUpGenerationInvokeMethod", Random.Range(minTime, maxTime));
    }

    public Vector3 GetRandomPoint()
    {
        bool flag = true;
        Vector3 pos = Vector3.zero;
        while (flag)
        {
            GridGraph gridGraph = GridNode.GetGridGraph(0);
            GridNodeBase gridNode = gridGraph.GetNode(Random.Range(0, gridGraph.width), Random.Range(0, gridGraph.depth));
            pos = (Vector3)gridNode.position;
            flag = !gridNode.Walkable;
        }
        return pos;
    }

    public void CreateAPowerUp()
    {
        loc = GetRandomPoint();
        loc.y = 0.525f;
        locations.Add(loc);
        GameObject locGlow = Instantiate(locationGlow, loc, locationGlow.transform.rotation);
        Invoke("CreatePowerUp", 2);
        Destroy(locGlow, 2.5f);
    }

    private void CreatePowerUp()
    {
        loc = locations[0];
        locations.RemoveAt(0);
        loc.y = 0.8f;
        int prob = Random.Range(1, 101);
        if(prob == spellPrct)
        {
            powerUp = Instantiate(powerupPrefabs[7], loc, powerupPrefabs[0].transform.rotation);
        }
        else if (prob >= spellPrct && prob < (spellPrct + protonPrct))
        {
            powerUp = Instantiate(powerupPrefabs[5], loc, powerupPrefabs[0].transform.rotation);
        }
        else if (prob >= (spellPrct + protonPrct) && prob < (spellPrct + protonPrct + rocketPrct))
        {
            powerUp = Instantiate(powerupPrefabs[6], loc, powerupPrefabs[0].transform.rotation);
        }
        else if (prob >= (spellPrct + protonPrct + rocketPrct) && prob < (spellPrct + protonPrct + rocketPrct + sniperPrct))
        {
            powerUp = Instantiate(powerupPrefabs[1], loc, powerupPrefabs[0].transform.rotation);
        }
        else if (prob >= (spellPrct + protonPrct + rocketPrct + sniperPrct) && prob < (spellPrct + protonPrct + rocketPrct + sniperPrct + fireballPrct))
        {
            powerUp = Instantiate(powerupPrefabs[2], loc, powerupPrefabs[0].transform.rotation);
        }
        else if (prob >= (spellPrct + protonPrct + rocketPrct + sniperPrct + fireballPrct) && prob < (spellPrct + protonPrct + rocketPrct + sniperPrct + fireballPrct + grenadePrct))
        {
            powerUp = Instantiate(powerupPrefabs[3], loc, powerupPrefabs[0].transform.rotation);
        }
        else if (prob >= (spellPrct + protonPrct + rocketPrct + sniperPrct + fireballPrct + grenadePrct) && prob < (spellPrct + protonPrct + rocketPrct + sniperPrct + fireballPrct + grenadePrct + mediumBulletPrct))
        {
            powerUp = Instantiate(powerupPrefabs[4], loc, powerupPrefabs[0].transform.rotation);
        }
        else //if(prob >= (spellPrct + protonPrct + rocketPrct + sniperPrct + fireballPrct + grenadePrct + mediumBulletPrct) && prob < ((spellPrct + protonPrct + rocketPrct + sniperPrct + fireballPrct + grenadePrct + mediumBulletPrct + healthPowerupPrct)))
        {
            powerUp = Instantiate(powerupPrefabs[0], loc, powerupPrefabs[0].transform.rotation);
        }


        //if (Random.Range(0,100) <= spellPrct)
        //{
        //    powerUp = Instantiate(powerupPrefabs[powerupPrefabs.Length - 1], loc, powerupPrefabs[0].transform.rotation);
        //}
        //else
        //{
        //    powerUp = Instantiate(powerupPrefabs[Random.Range(0, powerupPrefabs.Length-1)], loc, powerupPrefabs[0].transform.rotation);
        //}
        powerUp.GetComponent<PowerUp>().SetExpiryTime(expiryTime, blinkTime);
        powerUp.GetComponent<PowerUp>().SetPointer(pointerManager.CreatePointer(powerUp.transform, TargetPointerType.POWERUP_POINTER));
    }
}
