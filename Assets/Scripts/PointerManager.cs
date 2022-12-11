using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{

    [SerializeField]
    private GameObject pointerPrefab;

    [SerializeField]
    private Transform playerTransform;

    public GameObject CreatePointer(Transform toTarget, TargetPointerType targetPointerType = TargetPointerType.ENEMY_POINTER)
    {
        return CreatePointer(playerTransform, toTarget, targetPointerType);
    }

    public GameObject CreatePointer(Transform fromTarget, Transform toTarget, TargetPointerType targetPointerType = TargetPointerType.ENEMY_POINTER)
    {
        GameObject pointer = Instantiate(pointerPrefab,transform);
        pointer.GetComponent<Pointer>().SetTargets(fromTarget, toTarget, targetPointerType);
        return pointer;
    }

    public void ClearEnemyPointers()
    {
        foreach(Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
    }
}

public enum TargetPointerType { ENEMY_POINTER, POWERUP_POINTER }


