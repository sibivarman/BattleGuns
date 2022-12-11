using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICoin : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float reachTime;

    [SerializeField]
    private float endReachedDistance;

    [SerializeField]
    private float coinWaitTime;

    [SerializeField]
    private float rotationSpeed;

    private Vector3 startPos;

    private float prct;

    private bool enableUpdateFlag;

    private float fillPrct;

    private int direction;

    private void Start()
    {
        startPos = transform.position;
        direction = Random.Range(0, 2) * 2 - 1;
        Invoke("EnableUpdate", coinWaitTime);
    }

    void Update()
    {
        if (enableUpdateFlag)
        {
            fillPrct = prct * (1 / reachTime);
            transform.position = Vector3.Lerp(startPos, target.position, fillPrct);
            transform.localScale = new Vector3(1 - fillPrct, 1 - fillPrct, 1 - fillPrct);
            if (Vector3.SqrMagnitude(target.position - transform.position) < (endReachedDistance * endReachedDistance))
            {
                Destroy(gameObject);
            }
            prct += Time.deltaTime;
        }
        transform.Rotate(Vector3.forward, direction * rotationSpeed * Time.deltaTime);
    }

    private void EnableUpdate()
    {
        enableUpdateFlag = true;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}
