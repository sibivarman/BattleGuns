using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private FloatingJoystick floatingJoystick;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform firingPoint;

    [SerializeField]
    private float defaultFireRate;

    [SerializeField]
    private GameObject bulletGO;

    [SerializeField]
    private GameObject muzzleFlash;

    [SerializeField]
    private EnemyGenerator enemyGenerator;

    [SerializeField]
    private int baseHealth;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private PowerUpProgressBar powerUpProgressBar;

    [SerializeField]
    private float healthBarHeight;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject powerUpParticleEfxPrefab;

    [SerializeField]
    private AudioSource runningAudioSource;

    [SerializeField]
    private AudioSource impactAudioSource;

    [SerializeField]
    private AudioSource deathAudioSource;

    [SerializeField]
    private GameObject powerDownSoundSfxPrefab;

    [SerializeField]
    private ParticleSystem floorTrialEfx;

    [SerializeField]
    private ParticleSystem powerUpEfx;

    [SerializeField]
    private float turnSpeed;

    [SerializeField]
    private float resetTargetEnemyTime;

    private CharacterController characterController;

    private Vector3 joyStickPos;

    private float attackTimer;

    private GameObject nearestEnemy;

    private GameObject defaultBulletGO, defaultMuzzleGO;

    private float fireRate;

    private int fireAnimationHash, runAnimationHash, dieAnimationHash, idleAnimationHash, resetAnimationHash;

    private bool isPlayerEnabled = true;

    private bool isFiringEnabled;

    private int fullHealth;

    private int health;

    private float targetEnemyTime;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        defaultBulletGO = bulletGO;
        defaultMuzzleGO = muzzleFlash;
        fireRate = defaultFireRate;
        muzzleFlash = Instantiate(muzzleFlash, firingPoint.position, Quaternion.identity, firingPoint);
        health = baseHealth;
        fullHealth = baseHealth;
        UpdateHealth(gamePlayManager.GetHealthLevel());
        //ChangeBullet(defaultBulletGO, defaultMuzzleGO , 0);

        //audioSource = GetComponent<AudioSource>();

        //Set Total Health
        healthBar.SetTotalHealth(health, healthBarHeight);

        //Set Animator trigger hash values
        fireAnimationHash = Animator.StringToHash("Fire");
        runAnimationHash = Animator.StringToHash("Run");
        dieAnimationHash = Animator.StringToHash("Die");
        idleAnimationHash = Animator.StringToHash("Idle");
        resetAnimationHash = Animator.StringToHash("Reset");
    }

    void Update()
    {
        if (isPlayerEnabled)
        {
            joyStickPos = new Vector3(floatingJoystick.Horizontal, 0, floatingJoystick.Vertical);
            if (joyStickPos != Vector3.zero)
            {
                characterController.Move(joyStickPos * Time.deltaTime * speed);
                transform.rotation = Quaternion.LookRotation(joyStickPos);
                attackTimer = 0;
                nearestEnemy = null;

                //Do run Animation
                animator.ResetTrigger(fireAnimationHash);
                animator.SetTrigger(runAnimationHash);

                //Play running Sound
                if (AudioManager.isSoundEnabled && !runningAudioSource.isPlaying)
                {
                    runningAudioSource.Play();
                }
            }
            else
            {
                //Stop run Animation
                //animator.ResetTrigger(runAnimationHash);

                if (nearestEnemy == null)
                {
                    nearestEnemy = enemyGenerator.GetClosestEnemy();
                }
                if (nearestEnemy != null)
                {
                    animator.ResetTrigger(runAnimationHash);
                    Attack();
                    Vector3 targetPos = nearestEnemy.transform.position - transform.position;
                    targetPos.y = 0;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPos), Time.deltaTime * turnSpeed);
                }
                else
                {
                    animator.SetTrigger(idleAnimationHash);
                }
                runningAudioSource.Stop();
            }
            joyStickPos = transform.position;
            joyStickPos.y = 1.05f;
            transform.position = joyStickPos;
            targetEnemyTime += Time.deltaTime;
            if(targetEnemyTime >= resetTargetEnemyTime)
            {
                targetEnemyTime = 0;
                ResetCurrEnemy();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PowerUp>())
        {
            //Do powerup animation
            PlayPowerUpParticleEffect();

            //Do powerup sound effect
            PowerUp powerUp = other.GetComponent<PowerUp>();
            if (powerUp.IsBulletPowerUp())
            {
                //Cancel Invoke is called to cancel the method call which reset the powerup for the previous powerup
                //Since a new powerup is collected previous powerup cancel invoke reset this powerup so cancel invoke is called to resetting current powerup
                CancelInvoke();
                ChangeBullet(powerUp.GetBulletGO(), powerUp.GetMuzzleGO(), powerUp.ResetTime());
                //Update UI for powerup
                UpdatePowerupProgressBar(powerUp.ResetTime(), powerUp.GetPowerUpProgressBarColor());
            }
            else if (powerUp.IsHealthPowerUp())
            {
                health += (fullHealth * powerUp.GetHealth() / 100);
                if (health > fullHealth)
                    health = fullHealth;
                healthBar.SetHealth(health);
            }
            else if (powerUp.IsSpellPowerUp())
            {
                if (nearestEnemy)
                {
                    powerUp.Fire(nearestEnemy.transform);
                }
                else
                {
                    nearestEnemy = enemyGenerator.GetClosestEnemy();
                    if (nearestEnemy)
                    {
                        powerUp.Fire(nearestEnemy.transform);
                    }
                }
            }
        }
        else if (other.GetComponent<ExitDoor>())
        {
            powerUpProgressBar.ClearPowerUpProgressBar();
            ChangeBullet(defaultBulletGO, defaultMuzzleGO, 0);

            floorTrialEfx.Clear(true);
            floorTrialEfx.Stop();
            gamePlayManager.ContinueGame(other.GetComponent<ExitDoor>().GetNextLevel());
            floorTrialEfx.Play();
        }
    }

    public void UpdateHealth(int healthLvl)
    {
        if(healthLvl > 1)
        {
            health = baseHealth + (20 * healthLvl);
            fullHealth = health;
            healthBar.SetTotalHealth(fullHealth, healthBarHeight);
        }
    }

    public void UpdatePowerupProgressBar(float resetTime, Color progressBarColor)
    {
        powerUpProgressBar.gameObject.SetActive(true);
        powerUpProgressBar.SetColor(progressBarColor);
        powerUpProgressBar.SetTotalTime(resetTime);
    }

    public void IncreaseHealthPowerUp()
    {
        PlayPowerUpParticleEffect();
    }

    public void IncreaseFirePowerUp()
    {
        PlayPowerUpParticleEffect();
    }

    public void IncreaseCoinPowerUp()
    {
        PlayPowerUpParticleEffect();
    }

    public void PlayPowerUpParticleEffect()
    {
        GameObject powerUPParticleEfx = Instantiate(powerUpParticleEfxPrefab, transform.position, powerUpParticleEfxPrefab.transform.rotation, transform);
        powerUPParticleEfx.transform.localPosition = new Vector3(0, -1, 0);
    }

    private void Attack()
    {
        if (nearestEnemy == null)
            return;

        //Stop running Soutn
        runningAudioSource.Stop();

        //Do attack Animation
        animator.SetTrigger(fireAnimationHash);

        //transform.LookAt(new Vector3(nearestEnemy.transform.position.x, 1.05f, nearestEnemy.transform.position.z));

        if (attackTimer > (1 / fireRate))
        {
            attackTimer = 0;
            if (bulletGO.GetComponent<Bullet>().IsSingleBulletType())
            {
                GameObject bullet = Instantiate(bulletGO, firingPoint.position, Quaternion.identity);
                bullet.transform.rotation = transform.rotation;
                bullet.GetComponent<Bullet>().Fire(nearestEnemy.transform);
                muzzleFlash.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                List<GameObject> enemiesList = enemyGenerator.GetAllEnemies();
                foreach(GameObject enemy in enemiesList)
                {
                    if (enemy == null)
                        continue;
                    GameObject bullet = Instantiate(bulletGO, transform.position, Quaternion.identity);
                    bullet.transform.LookAt(enemy.transform);
                    bullet.GetComponent<Bullet>().Fire(enemy.transform);
                    muzzleFlash.GetComponent<ParticleSystem>().Play();
                }
            }
        }
        attackTimer += Time.deltaTime;
    }

    public void Hit(int _damage)
    {
        _damage += (int)(_damage * (0.6f * (( fullHealth / baseHealth) - 1)));

        health -= _damage;

        if (isPlayerEnabled)
        {
            if (AudioManager.isSoundEnabled && !impactAudioSource.isPlaying)
            {
                impactAudioSource.Play();
            }
            if (health > 0)
            {
                healthBar.SetHealth(health);
            }
            else
            {
                //Game Over
                healthBar.SetHealth(0);
                runningAudioSource.Stop();
                isPlayerEnabled = false;
                animator.SetTrigger(dieAnimationHash);
                if (AudioManager.isSoundEnabled)
                {
                    deathAudioSource.Play();
                }
            }
        }
    }

    public void EnablePlayer(bool flag = true)
    {
        isPlayerEnabled = flag;
    }

    public void EnableFiring(bool flag = true)
    {
        isFiringEnabled = flag;
    }

    public void GameOver()
    {
        gamePlayManager.GameOver();
    }

    public bool IsAlive()
    {
        return isPlayerEnabled;
    }

    public void ResetCurrEnemy()
    {
        nearestEnemy = null;
    }

    public void SetGamePlayManager(GamePlayManager _gamePlayManager)
    {
        gamePlayManager = _gamePlayManager;
    }

    public void SetEnemyGenerator(EnemyGenerator _enemyGenerator)
    {
        enemyGenerator = _enemyGenerator;
    }

    public void SetPlayerHealthBarManager(HealthBar _healthBar)
    {
        healthBar = _healthBar;
    }

    public void SetFloatingJoyStick(FloatingJoystick _floatingJoyStick)
    {
        floatingJoystick = _floatingJoyStick;
    }

    public void ResetPlayer()
    {
        animator.ResetTrigger(fireAnimationHash);
        animator.ResetTrigger(runAnimationHash);
        animator.ResetTrigger(idleAnimationHash);
        animator.ResetTrigger(dieAnimationHash);
        animator.SetTrigger(resetAnimationHash);
        ChangeBullet(defaultBulletGO, defaultMuzzleGO, 0);
        health = fullHealth;
        //isFiringEnabled = true;
        //health bar height is passed to set the height of the health bar form the player, healthBar script is used by both player and enemy,
        //player and enemy have diff height so height is passed as param
        healthBar.SetTotalHealth(fullHealth,healthBarHeight);
        floorTrialEfx.Clear(true);
        floorTrialEfx.Stop();
        floorTrialEfx.Play();

        //ResetPowerUpProgressBar
        powerUpProgressBar.ClearPowerUpProgressBar();
    }

    public void RefillHealth()
    {
        health = fullHealth;
        healthBar.SetTotalHealth(fullHealth, healthBarHeight);
    }

    public void ResetPowerUP()
    {
        powerUpProgressBar.ClearPowerUpProgressBar();
    }

    private void ChangeBullet(GameObject _bulletGO, GameObject _muzzleGO, float resetTime)
    {
        bulletGO = _bulletGO;
        muzzleFlash = Instantiate(_muzzleGO, firingPoint.position, Quaternion.identity, firingPoint);
        if (resetTime > 0)
        {
            Invoke("ResetBuletGO", resetTime);
            powerUpEfx.Play(true);
        }
        else
        {
            powerUpEfx.Stop(true);
        }
        fireRate = defaultFireRate * bulletGO.GetComponent<Bullet>().GetFireRate();
    }

    private void ResetBuletGO()
    {
        //Play sound effect for power down
        if (AudioManager.isSoundEnabled)
        {
            GameObject powerDownSoundSfx = Instantiate(powerDownSoundSfxPrefab, transform.position, Quaternion.identity);
            Destroy(powerDownSoundSfx, 1.5f);
        }
        ChangeBullet(defaultBulletGO, defaultMuzzleGO, 0);
    }
}
