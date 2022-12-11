using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoBullet : IBullet
{

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private float frequency;

    [SerializeField]
    private float magnitude;

    [SerializeField]
    private AudioSource audioSource;

    private Vector3 direction;
    private bool enableUpdate;

    private Vector3 position;

    private Vector3 axis;

    private void Start()
    {
        position = transform.position;
        audioSource.Play();

        axis = Vector3.right;
    }

    void Update()
    {
        if (enableUpdate)
        {
            position = transform.position;
            position.x += (Mathf.Sin(Time.time * frequency) * magnitude * Time.deltaTime);
            position.z += (transform.forward.z * Time.deltaTime * bulletSpeed);
            transform.position = position;

            //position += transform.forward * Time.deltaTime * bulletSpeed;
            //transform.position = position + (axis * Mathf.Sign(Time.deltaTime * frequency) * magnitude);
        }
    }

    public override void Fire(Transform _target)
    {
        Vector3 targetPos = _target.position;
        targetPos.x = 0;
        targetPos.y = 0;
        Vector3 currPos = transform.position;
        currPos.x = 0;
        currPos.y = 0;

        direction = targetPos - currPos;
        direction = direction.normalized;
        transform.forward = direction;
        enableUpdate = true;
    }
}
