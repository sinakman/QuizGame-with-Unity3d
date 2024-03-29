using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    public SimpleObjectPool answerButtonObjectPool;
    public Text questionText;
    public Text scoreDisplay;
    public Text timeRemainingDisplay;
    public Transform answerButtonParent;

    public GameObject questionDisplay;
    public GameObject roundEndDisplay;
    public Text highScoreDisplay;

    private DataController dataController;
    private RoundData currentRoundData;
    private QuestionData[] questionPool;
    private static List<QuestionData> unansweredQuestions;
    private QuestionData currentQuestion;
    //private static List<QuestionData> unansweredQuestions;
    //private QuestionData currentQuestions;

    public AudioSource incorrect;
    public AudioSource correct;

    private bool isRoundActive = false;
    private float timeRemaining;
    private int playerScore;
    private int questionIndex;
    private List<GameObject> answerButtonGameObjects = new List<GameObject>();

    void Start()
    {

        dataController = FindObjectOfType<DataController>();                                

        currentRoundData = dataController.GetCurrentRoundData();                           


        //if (unansweredQuestions==null||unansweredQuestions.Count==0)
        //{
        //    unansweredQuestions=currentRoundData.questions.ToList<QuestionData>();
        //}





        questionPool = currentRoundData.questions;

        unansweredQuestions = questionPool.ToList();

        if (unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            unansweredQuestions = questionPool.ToList();
        }


       
        timeRemaining = currentRoundData.timeLimitInSeconds;                                

        UpdateTimeRemainingDisplay();
        playerScore = 0;
        questionIndex = 0;

        ShowQuestion();
        isRoundActive = true;
    }

    //void GetRandomQuestion()
    //{
    //    int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count);
    //   currentQuestions = unansweredQuestions[randomQuestionIndex];
    //}

    void Update()
    {
        if (isRoundActive)
        {
            
            timeRemaining -= Time.deltaTime;
            UpdateTimeRemainingDisplay();

            if (timeRemaining <= 0f)
            {
                EndRoundSound();
                EndRound();
            }
        }
    }

    void ShowQuestion()
    {
        RemoveAnswerButtons();
        //
        //int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count);
        //currentQuestion = unansweredQuestions[randomQuestionIndex];
        //questionText.text = currentQuestion.questionText;
        //
        
        QuestionData questionData = questionPool[questionIndex];                            
        questionText.text = questionData.questionText;                                     


        for (int i = 0; i < questionData.answers.Length; i++)                             
        {
            GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();        
            answerButtonGameObjects.Add(answerButtonGameObject);
            answerButtonGameObject.transform.SetParent(answerButtonParent);
            answerButtonGameObject.transform.localScale = Vector3.one;

            AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
            answerButton.SetUp(questionData.answers[i]);                                    
        }
    }

    void RemoveAnswerButtons()
    {
        while (answerButtonGameObjects.Count > 0)                                           
        {
            answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
            answerButtonGameObjects.RemoveAt(0);
        }
    }

    public void EndRoundSound()
    {
        incorrect.Play();
    }

    public void TrueRoundSound()
    {
        correct.Play();
    }

public void AnswerButtonClicked(bool isCorrect)
    {
        if (isCorrect)
        {
            timeRemaining = currentRoundData.timeLimitInSeconds;
            timeRemaining -= Time.deltaTime;
            UpdateTimeRemainingDisplay();

            TrueRoundSound();
            playerScore += currentRoundData.pointsAddedForCorrectAnswer;                    

            if (playerScore>500)
            {
                EndRound();
            }
            else
            {
                scoreDisplay.text = playerScore.ToString();
            }
            
            Update();
            
        }
        else if (!isCorrect)
        {
            EndRoundSound();
            EndRound();
        }


        if (unansweredQuestions.Count > questionIndex + 1)                                             
        {
            Update();
            questionIndex = Random.Range(0, unansweredQuestions.Count);
            currentQuestion = questionPool[questionIndex];


            //questionIndex = Random.Range(0, questionPool.Count());

            //questionIndex++;
            ShowQuestion();
        }
        else if (unansweredQuestions.Count == questionIndex + 1)                                        
       {
            Update();

            questionIndex = Random.Range(0, unansweredQuestions.Count);
            currentQuestion = unansweredQuestions[questionIndex];


            //questionIndex = Random.Range(0, questionPool.Count());

            //questionIndex++;
            ShowQuestion();

        }
        else 
        {
            
            EndRound();
        }
    }
    void SetCurrentQuestion()   
    {
        int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[randomQuestionIndex];
    }
    private void UpdateTimeRemainingDisplay()
    {
        timeRemainingDisplay.text = Mathf.Round(timeRemaining).ToString();
    }

    public void EndRound()
    {
        isRoundActive = false;
        dataController.SubmitNewPlayerScore(playerScore);
        highScoreDisplay.text = dataController.GetHighestPlayerScore().ToString();

        questionDisplay.SetActive(false);
        roundEndDisplay.SetActive(true);
        unansweredQuestions.Clear();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }
}