using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    bool isRotating = false;
    bool IsAdvancing = false;
    bool firstClick = true;
    bool isDiceReady = false;

    Animator diceAnimator;

    //Booleanos que indican cuando se ejecutan las funciones de las otras clases
    [HideInInspector]
    public bool activeEventDisplace = false;

    [HideInInspector]
    public bool activeEventSituation = false;

    [HideInInspector]
    public bool isTVEventActive = false;

    [HideInInspector]
    public bool isEndingTurn = false;

    [HideInInspector]
    public bool isPlayerArrived = false;

    [HideInInspector]
    public bool isDisplacing = false;

    [HideInInspector]
    public bool isRandomNumReady = false;

    [HideInInspector]
    public int diceNumber;

    [HideInInspector]
    public int turnOfPlayer = 0;

    [HideInInspector]
    public int currentTarget = 0;

    [HideInInspector]
    public int temporalCurrentTarget = 0;

    public Transform tvCameraTransform;
    public Transform diceCamera;
    public Transform emptyDiceTransform;
    public Transform diceTransform;
    //public AudioClip steps;

    [Header("GameObject Arrays")]

    public GameObject[] voidCharacters = new GameObject[4];
    public GameObject[] bodyCharacters = new GameObject[4];
    public Transform[] playerCameraFP = new Transform[4];
    public Transform[] playerCameraTP = new Transform[4];
    public Transform[] wayPoints;
    public Vector3[] diceFaceRotation;

    [Header("Text")]

    public Text playerTurnText;
    public Text eventTvText;
    public Text eventDiceText;

    [Header("Strings Arrays")]

    public string[] advanceSituationText = new string[6];
    public string[] backSituationText = new string[6];
    public string[] misterySituationText = new string[6];
    public string[] neutralSituationText = new string[6];


    void PlayerTurn() // Este alterará los turnos de los jugadores.
    {
        if (firstClick) // La pantalla inicial de turno.
        {
            playerTurnText.text = "Player " + (turnOfPlayer + 1) + " Turn";
            TypesOfCameras(4);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && firstClick) //El click que hace que muestre el dado rotando.
        {
            firstClick = false;
            isDiceReady = true;
            TypesOfCameras(1);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && isRotating) // El click que muestra la animacion del dado; despues, cambia de camara.
        {
            isDiceReady = false;
            isRotating = false;
            StartCoroutine(TimeToDiceRotate(0.1f));                       
        }
        if (IsAdvancing) // El click que lo hace avanzar por los waypoints
        {
            IsAdvancing = false;
            activeEventSituation = true;
            activeEventDisplace = true;
        }
        if (isTVEventActive && isPlayerArrived)
        {
            isTVEventActive = false;
            TypesOfCameras(3);
            StartCoroutine(TimeToFinishTurn(3f));
        }
        if (isEndingTurn && isPlayerArrived)
        {
            isEndingTurn = false;

            //if (turnOfPlayer < 4)           
            //    turnOfPlayer++;            
            //else
            //    turnOfPlayer = 0;

            firstClick = true;
            isDiceReady = false;
            isRotating = false;
            IsAdvancing = false;
            isTVEventActive = false;
            isRandomNumReady = false;
            activeEventSituation = false;
            activeEventDisplace = false;
            isPlayerArrived = false;
            isDisplacing = false;
        }
    }

    IEnumerator TimeToDiceRotate(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        diceNumber = Random.Range(1, 7);

        if (diceNumber == 1)
            eventDiceText.text = "Advance... \n \n \n \n \n \n" + " space";
        else
            eventDiceText.text = "Advance... \n \n \n \n \n \n " + " spaces";

        diceAnimator.Play("Dice Animations");
        StartCoroutine(TimeToStopDice(2f));
    }

    IEnumerator TimeToStopDice(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        diceAnimator.Rebind();
        emptyDiceTransform.eulerAngles = diceFaceRotation[diceNumber - 1];
        StartCoroutine(TimeToAdvance(2f));
    }

    IEnumerator TimeToAdvance(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        TypesOfCameras(2);
        IsAdvancing = true;
    }

    IEnumerator TimeToFinishTurn(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        TypesOfCameras(2);
        StartCoroutine(TimeToStopAllCoroutines(1f));
    }

    IEnumerator TimeToStopAllCoroutines(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        StopAllCoroutines();
        isEndingTurn = true;
    }

    IEnumerator TimeToWin()
    {
        yield return new WaitForSecondsRealtime(2f);
        LoadCreditsScene();
    }
    void DiceRotate()
    {
        if (firstClick == false && isDiceReady)
        {
            emptyDiceTransform.Rotate(new Vector3(1, 1) * 15);
            isRotating = true;
        }
    }

    void TypesOfCameras(int camType)
    {
        if (camType == 1) // The diceCam is active with this num.
        {
            playerCameraFP[turnOfPlayer].gameObject.SetActive(false);
            playerCameraTP[turnOfPlayer].gameObject.SetActive(false);
            tvCameraTransform.gameObject.SetActive(false);
            diceCamera.gameObject.SetActive(true);

            playerTurnText.gameObject.SetActive(false);
            eventTvText.gameObject.SetActive(false);
            eventDiceText.gameObject.SetActive(true);
        }
        else if (camType == 2) // The third person player camera is active.
        {
            diceCamera.gameObject.SetActive(false);
            playerCameraFP[turnOfPlayer].gameObject.SetActive(false);
            tvCameraTransform.gameObject.SetActive(false);
            playerCameraTP[turnOfPlayer].gameObject.SetActive(true);

            eventDiceText.gameObject.SetActive(false);
            eventTvText.gameObject.SetActive(false);
            playerTurnText.gameObject.SetActive(true);
        }
        else if (camType == 3) // The TVcam is active in this turn.
        {
            playerCameraTP[turnOfPlayer].gameObject.SetActive(false);
            diceCamera.gameObject.SetActive(false);
            playerCameraFP[turnOfPlayer].gameObject.SetActive(false);
            tvCameraTransform.gameObject.SetActive(true);

            playerTurnText.gameObject.SetActive(false);
            eventDiceText.gameObject.SetActive(false);
            eventTvText.gameObject.SetActive(true);
        }
        else if (camType == 4) //Is active The firstPerson player camera.
        {
            tvCameraTransform.gameObject.SetActive(false);
            playerCameraTP[turnOfPlayer].gameObject.SetActive(false);
            diceCamera.gameObject.SetActive(false);
            playerCameraFP[turnOfPlayer].gameObject.SetActive(true);

            eventTvText.gameObject.SetActive(false);
            eventDiceText.gameObject.SetActive(false);
            playerTurnText.gameObject.SetActive(true);
        }
    }

    void Win()
    {
        if (voidCharacters[turnOfPlayer].transform.position == wayPoints[wayPoints.Length-1].transform.position)
        {
            StartCoroutine(TimeToWin());
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");   
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("First Room");
    }

    public void LoadStoryScene()
    {
        SceneManager.LoadScene("Story");
    }

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }

    private void Awake()
    {
        if (instance == null)    
            instance = this;       
        else        
            Destroy(this.gameObject);      

        diceAnimator = diceTransform.GetComponent<Animator>();
    }

    private void Update()
    {
        Win();
        PlayerTurn();
        DiceRotate();
    }
}



