using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Transform voidCharacterTransform;

    Vector3 targetDisplacement;
    Vector3 targetPosRotation;

    int temporalCurrentTarget = 0;
    bool isPlayingAudio = false;
    Animator characterAnimator;
    AudioSource characterAudioSource;

    public void PlayerAdvance() //The player advance with this function.
    {
        UpdatingTargetPoint();
        voidCharacterTransform.LookAt(targetDisplacement);
        voidCharacterTransform.position = Vector3.MoveTowards(voidCharacterTransform.position, targetDisplacement, 50f * Time.deltaTime);
    }

    void UpdatingTargetPoint() // Here, the objetive point is update.
    {
        if (voidCharacterTransform.position == targetDisplacement)
        {
            characterAnimator.Play("Waiting");
            isPlayingAudio = true;
            characterAudioSource.Stop();

            if (temporalCurrentTarget < GameManager.instance.currentTarget)
                temporalCurrentTarget++;
            else if (temporalCurrentTarget == GameManager.instance.currentTarget && GameManager.instance.isPlayerArrived == false)           
                GameManager.instance.isTVEventActive = true;            
        }
        else
        {
            characterAnimator.Play("Running");
            if (isPlayingAudio)
            {
                isPlayingAudio = false;
                characterAudioSource.Play();
            }
        }

    }

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
        characterAudioSource = GetComponent<AudioSource>();

    }
    private void Update()
    {
        voidCharacterTransform = GameManager.instance.voidCharacters[GameManager.instance.turnOfPlayer].transform;
        targetDisplacement = GameManager.instance.wayPoints[temporalCurrentTarget].transform.position;
        GameManager.instance.temporalCurrentTarget = temporalCurrentTarget;

        if (GameManager.instance.activeEventDisplace)
        {
            PlayerAdvance();
        }
    }
}


