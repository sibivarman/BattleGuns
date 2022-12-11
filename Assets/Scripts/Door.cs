using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Door : MonoBehaviour
{

    [SerializeField]
    private ExitDoor exitDoor;

    [SerializeField]
    private bool hasSkeletonHandObstacle;

    [ShowIf("hasSkeletonHandObstacle",true)]
    [SerializeField]
    private GameObject[] skeletonHandsPos;

    public void OpenExitDoor()
    {
        exitDoor.OpenExitDoor();
    }

    public void SetNextLevel(int level)
    {
        exitDoor.SetNextLevel(level);
    }

    public bool HasSkeletonHandObstacle()
    {
        return hasSkeletonHandObstacle;
    }

    public GameObject[][] GetSkeletionHands()
    {
        GameObject[][] skeletonHandPos2D = new GameObject[skeletonHandsPos.Length / 2][];
        for(int skeletonHandCount = 0, counter = 0; skeletonHandCount < skeletonHandsPos.Length/2; skeletonHandCount++)
        {
            skeletonHandPos2D[skeletonHandCount] = new GameObject[2];
            skeletonHandPos2D[skeletonHandCount][0] = skeletonHandsPos[counter++];
            skeletonHandPos2D[skeletonHandCount][1] = skeletonHandsPos[counter++];
        }
        return skeletonHandPos2D;
    }
}
