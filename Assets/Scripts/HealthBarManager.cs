using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{

    [SerializeField]
    private GameObject healthBarPrefab;

    public HealthBar GetHealthBar(Transform _target)
    {
        GameObject healthBarGO = Instantiate(healthBarPrefab, transform);
        healthBarGO.GetComponent<HealthBar>().SetTarget(_target);
        return healthBarGO.GetComponent<HealthBar>();
    }

    public void ClearEnemyHealthBar()
    {
        foreach(Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
    }
}
