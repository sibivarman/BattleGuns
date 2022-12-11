using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour
{

    [SerializeField]
    private Transform toTarget;

    [SerializeField]
    private Transform fromTarget;

    [SerializeField]
    private GameObject pointer;

    [SerializeField]
    private Color powerupIndicatorColor;

    [SerializeField]
    private Vector3 offset;

    private Vector3 fromPosition;
    private Vector3 toPosition;
    private Vector3 dir;
    private Vector3 targetScreenPostion;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        toPosition = toTarget.position;
        toPosition.y = 0;
        fromPosition = fromTarget.position;
        fromPosition.y = 0;
        dir = (toPosition - fromPosition);
        float angle = AngleInDeg(fromPosition, toPosition);
        transform.eulerAngles = new Vector3(0, 0, angle);

        targetScreenPostion = mainCamera.WorldToScreenPoint(toTarget.position);
        bool isOffScreen = targetScreenPostion.x < 0 || targetScreenPostion.x > Screen.width || targetScreenPostion.y < 0 || targetScreenPostion.y > Screen.height;
        pointer.SetActive(isOffScreen);
        if (isOffScreen)
        {
            if(targetScreenPostion.x < 0)
            {
                targetScreenPostion.x = 0 + offset.x;
            }
            else if(targetScreenPostion.x > Screen.width)
            {
                targetScreenPostion.x = Screen.width - offset.x;
            }
            else if(targetScreenPostion.y < 0)
            {
                targetScreenPostion.y = 0 + offset.y;
            }
            else if(targetScreenPostion.y > Screen.height)
            {
                targetScreenPostion.y = Screen.height - offset.y;
            }

            transform.position = targetScreenPostion;
        }
    }

    public void SetTargets(Transform _fromTarget, Transform _toTarget, TargetPointerType pointerType)
    {
        fromTarget = _fromTarget;
        toTarget = _toTarget;
        if (pointerType.Equals(TargetPointerType.POWERUP_POINTER))
        {
            pointer.GetComponent<Image>().color = powerupIndicatorColor;
            pointer.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
    }

    public float AngleInDeg(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Atan2(vec2.z - vec1.z, vec2.x - vec1.x) * 180 / Mathf.PI;
    }
}
