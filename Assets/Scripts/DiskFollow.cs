using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskFollow : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offset;

    private void Start()
    {
        //temporary if block
        if (target == null)
            return;
        Vector3 pos = new Vector3(target.position.x, transform.position.y, target.position.z);
        pos += offset;
        transform.position = pos;
    }

    void Update()
    {
        if (target)
        {
            Vector3 pos = new Vector3(target.position.x, transform.position.y, target.position.z);
            pos += offset;
            transform.position = pos;
        }
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}
