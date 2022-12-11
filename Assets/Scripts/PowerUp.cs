using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PowerUp : MonoBehaviour
{

    [SerializeField]
    private bool isBulletPowerUp = true;

    [ShowIf("isBulletPowerUp", true)]
    [SerializeField]
    private GameObject bulletGO;

    [ShowIf("isBulletPowerUp", true)]
    [SerializeField]
    private GameObject muzzleGO;

    [SerializeField]
    private bool isHealthPowerUp = false;

    [ShowIf("isHealthPowerUp", true)]
    [SerializeField]
    private int healthRestore;

    [SerializeField]
    private bool isSpellPowerUp = false;

    [ShowIf("isSpellPowerUp",true)]
    [SerializeField]
    private GameObject spellGOPrefab;

    [SerializeField]
    private float resetTime;

    [SerializeField]
    private Color progressBarColor;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AudioClip audioClip;

    private int collectedHash, blinkHash;

    private GameObject pointer;


    private void Start()
    {
        collectedHash = Animator.StringToHash("collected");
        blinkHash = Animator.StringToHash("blink");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>())
        {
            //Do Destroy animation
            if (AudioManager.isSoundEnabled)
            {
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
            }
            //animator.enabled = true;
            animator.SetTrigger(collectedHash);
        }
    }

    public bool IsBulletPowerUp()
    {
        return isBulletPowerUp;
    }

    public bool IsHealthPowerUp()
    {
        return isHealthPowerUp;
    }

    public bool IsSpellPowerUp()
    {
        return isSpellPowerUp;
    }

    public GameObject GetBulletGO()
    {
        return bulletGO;
    }

    public GameObject GetMuzzleGO()
    {
        return muzzleGO;
    }

    public int GetHealth()
    {
        return healthRestore;
    }

    public float ResetTime()
    {
        return resetTime;
    }

    public Color GetPowerUpProgressBarColor()
    {
        return progressBarColor;
    }

    public void SetExpiryTime(float _time, float _blinkTime)
    {
        Invoke("SetBlinkTrigger", _time - _blinkTime);
        Invoke("SetCollectedTrigger", _time);
    }

    public void SetPointer(GameObject _pointer)
    {
        pointer = _pointer;
    }

    public void Fire(Transform _target)
    {
        if (_target)
        {
            GameObject spellGO = Instantiate(spellGOPrefab, new Vector3(_target.position.x, spellGOPrefab.transform.position.y, _target.position.z), Quaternion.LookRotation((_target.position - transform.position).normalized));
            spellGO.GetComponent<Bullet>().Fire(_target);
        }
    }

    public void DestroyGameObject()
    {
        Destroy(pointer);
        Destroy(gameObject);
    }

    private void SetBlinkTrigger()
    {
        animator.SetTrigger(blinkHash);
    }

    private void SetCollectedTrigger()
    {
        animator.SetTrigger(collectedHash);
    }
}
