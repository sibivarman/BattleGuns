using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float speed;

    private Vector3 smoothTime, cameraPos;

    private float cameraPosX;

    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
    }

    void Update()
    {
        //cameraPos = Vector3.SmoothDamp(transform.position, target.position + offset, ref smoothTime, speed);
        //cameraPos.z = transform.position.z;
        //cameraPos.x = Mathf.Clamp(target.position.x, -3.9f, 3.9f);
        //transform.position = cameraPos;

        cameraPos = transform.position;
        cameraPos.x = target.position.x;
        //cameraPos.x = Mathf.Clamp(target.position.x, -3.9f, 3.9f);
        //cameraPos.z = target.position.z;
        //cameraPos.z = Mathf.SmoothDamp(transform.position.z, target.position.z + offset.z, ref smoothTime.z, speed);
        transform.position = cameraPos;

        //cameraPos = transform.position;
        //cameraPosX = Mathf.SmoothDamp(transform.position.x, target.position.x + offset.x, ref smoothTime.x, speed);
        //cameraPos.x = cameraPosX;
        //transform.position = cameraPos;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void ResetCameraPos()
    {
        transform.position = startPos;
    }
}
