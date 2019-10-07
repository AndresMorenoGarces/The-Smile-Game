using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformScript : MonoBehaviour
{
    [SerializeField]
    PlataformType plataformType = PlataformType.NeutralAsteroid;

    PlataformType updatingPlataformType;
    Text eventText;

    Transform[] wayPoints;

    bool IsTimeToContinue = true;

    int randomEvent;
    int temporalRandomEvent;
    int temporalCurrentTarget = 0;
    int updatingTemporalRandomEvent;

    // Don't modify
    string[] advanceSituationText;
    string[] backSituationText;
    string[] misterySituationText;
    string[] neutralSituationText;


    public void EventPlatformDisplacement()
    {
        if (GameManager.instance.isDisplacing == false)
        {
            GameManager.instance.isDisplacing = true;
            GameManager.instance.isRandomNumReady = true;
            temporalRandomEvent = Random.Range(0, 6);

            if (GameManager.instance.currentTarget <= wayPoints.Length - randomEvent)
            {
                GameManager.instance.currentTarget = temporalCurrentTarget + randomEvent;
                GameManager.instance.diceNumber = 0;
            }
            else
                GameManager.instance.currentTarget = wayPoints.Length - 1;
        }
    }

    public void EventPlatformArrive() 
    {
        if (this.transform.position == wayPoints[GameManager.instance.currentTarget].position)
        {
            if (GameManager.instance.isRandomNumReady)
            {
                GameManager.instance.isRandomNumReady = false;
                temporalRandomEvent = updatingTemporalRandomEvent;
            }

            if (updatingPlataformType == PlataformType.AdvanceAsteroid)
            {
                //StartCoroutine(TimeToWait(5f));

                if (GameManager.instance.currentTarget <= wayPoints.Length - temporalRandomEvent)
                    eventText.text = advanceSituationText[temporalRandomEvent] /*+ " Advance " + (temporalRandomEvent + 1) + " points."*/;
                else
                    temporalCurrentTarget = wayPoints.Length;
            }
            else if (updatingPlataformType == PlataformType.MisteryAsteroid && IsTimeToContinue)
            {
                IsTimeToContinue = false;
                eventText.text = misterySituationText[temporalRandomEvent]/* + " Teleport. "*/;
            }

            else if (updatingPlataformType == PlataformType.NeutralAsteroid && IsTimeToContinue)
            {
                IsTimeToContinue = false;
                eventText.text = neutralSituationText[temporalRandomEvent] /*+ ", wait the next turn."*/;
            }

            else if (updatingPlataformType == PlataformType.ReverseAsteroid)
            {
                //StartCoroutine(TimeToWait(5f));

                if (GameManager.instance.currentTarget >= 0 + temporalRandomEvent)               
                    eventText.text = backSituationText[temporalRandomEvent] /*+ ", reverse " + (temporalRandomEvent + 1) + " points."*/;                
                else
                    temporalCurrentTarget = 0;
            }
            GameManager.instance.isPlayerArrived = true;

        }
    }
    
    IEnumerator TimeToWait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        //GameManager.instance.randomEvent = Random.Range(1, 7);

        if (this.plataformType == PlataformType.AdvanceAsteroid)
        {
            GameManager.instance.currentTarget = temporalCurrentTarget + temporalRandomEvent;
        }
        else if (this.plataformType == PlataformType.ReverseAsteroid)
        {
            GameManager.instance.currentTarget = temporalCurrentTarget - temporalRandomEvent;
        }
    }

    void Start()
    {
        eventText = GameManager.instance.eventTvText;
        wayPoints = GameManager.instance.wayPoints;

        advanceSituationText = GameManager.instance.advanceSituationText;
        backSituationText = GameManager.instance.backSituationText;
        neutralSituationText = GameManager.instance.neutralSituationText;
        misterySituationText = GameManager.instance.misterySituationText;
    }
    private void Update()
    {
        updatingPlataformType = this.plataformType;
        randomEvent = GameManager.instance.diceNumber;
        temporalCurrentTarget = GameManager.instance.temporalCurrentTarget;
        updatingTemporalRandomEvent = Random.Range(0, 6);
        if (GameManager.instance.activeEventSituation)
        {
            EventPlatformDisplacement();
        }
        if (GameManager.instance.isTVEventActive)
        {
            EventPlatformArrive();
        }

    }
}
enum PlataformType
{
    AdvanceAsteroid,
    MisteryAsteroid,
    NeutralAsteroid,
    ReverseAsteroid
};
