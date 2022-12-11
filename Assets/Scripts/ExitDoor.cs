using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem exitParticleEffect;

    [SerializeField]
    private bool cannotPlaySound;

    [SerializeField]
    private int nextLevel;

    public void OpenExitDoor()
    {
        GetComponent<Animator>().enabled = true;
        GetComponent<BoxCollider>().isTrigger = true;
        exitParticleEffect.Play();
        if (AudioManager.isSoundEnabled && !cannotPlaySound)
        {
            GetComponent<AudioSource>().Play();
        }
    }

    public void SetNextLevel(int _nextLevel)
    {
        nextLevel = _nextLevel;
    }

    public int GetNextLevel()
    {
        return nextLevel;
    }

}
