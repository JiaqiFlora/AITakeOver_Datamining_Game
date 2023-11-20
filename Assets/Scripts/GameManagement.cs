using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance { get; private set; }

    [Header("UI")]
    public TMP_Text question;
    public TMP_Text answer;
    public TMP_Text humanAnswer;
    public TMP_Text AIAnswer;
    public TMP_Text solution;
    public TMP_Text winner;
    public GameObject resultPanel;
    public GameObject questionPanel;
    public GameObject waitPanel;
    public GameObject waitSolutionPanel;
    public GameObject gameoverPanel;
    public GameObject introPanel;
    public GameObject endPanel;

    [Header("Animator")]
    public GameObject[] AIGlasses;
    public GameObject[] humanGlasses;
    public GameObject[] people;
    public GameObject robot;
    public AnimationCurve jumpCurve;

    [Header("Button")]
    public ButtonController aiButtonAI;
    public ButtonController aiButtonHuman;

    [Header("Audio")]
    public AudioSource glassBreak;
    public AudioSource cheerAudio;
    public AudioSource booAudio;
    public AudioSource beforeQuestionAudio;
    public AudioSource beforeSolutionAudio;
    public AudioSource wonAudio;
    public AudioSource lostAudio;
    public AudioSource thinkingAudio;
    public AudioSource pressButtonAudio;
    public AudioSource backgroundAudio;

    // private variables for game contorl
    private QAList qaList;
    private int curRound = -1;
    private int maxRound = 10;
    private bool curAIAnswer;
    private bool curHumanChoice;
    private bool curSolution;

    private bool inRound = false;
    private bool humanDone = false;
    private bool AIDone = false;

    private int humanLife = 3;
    private int AILife = 3;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {

        StartCoroutine(startGame());
    }

    private void Update()
    {
        // when finish a round, trigger checking result
        if(inRound && humanDone && AIDone)
        {
            inRound = false;
            humanDone = false;
            AIDone = false;
            StartCoroutine(checkResult());
        }
        
    }

    public void SetupQAList(QAList qAList_new)
    {
        qaList = qAList_new;
        maxRound = qaList.entries.Length;
    }

    public void getHumanAnswer(bool is_human)
    {
        if(!canHumanPress())
        {
            return;
        }

        curHumanChoice = is_human;
        humanDone = true;

        humanAnswer.text = "Player chose : " + (is_human ? "human" : "AI");
    }

    public void getAIAnswer()
    {
        StartCoroutine(OnePersonJumpRoutine(robot.transform, 1));
        if(curAIAnswer)
        {
            aiButtonHuman.pressButton(false);
        } else
        {
            aiButtonAI.pressButton(false);
        }

        AIDone = true;
        AIAnswer.text = "AI chose : " + (curAIAnswer ? "human" : "AI");
    }

    public bool canHumanPress()
    {
        return inRound && !humanDone;
    }

    public bool canAIPress()
    {
        return inRound && !AIDone;
    }

    IEnumerator startGame()
    {
        // play the intro audio and show the intro panel
        backgroundAudio.Play();
        introPanel.SetActive(true);
        yield return new WaitForSeconds(20f);
        introPanel.SetActive(false);
        backgroundAudio.Stop();

        StartCoroutine(startNextQuestion());
    }

    IEnumerator startNextQuestion()
    {
        // show prep panel
        resultPanel.SetActive(false);
        questionPanel.SetActive(false);
        waitPanel.SetActive(true);

        beforeQuestionAudio.Play();
        yield return new WaitForSeconds(6f);
        beforeQuestionAudio.Stop();
        thinkingAudio.Play();

        curRound += 1;
        QAEntry curQA = qaList.entries[curRound];
        question.text = curQA.question;
        answer.text = curQA.answer;
        curAIAnswer = curQA.predicted_is_human;
        curSolution = curQA.is_human;

        inRound = true;
        humanDone = false;
        AIDone = false;

        // show question panel
        waitPanel.SetActive(false);
        questionPanel.SetActive(true);

        int randomTimeForAI = UnityEngine.Random.Range(1, 11);
        yield return new WaitForSeconds((float)randomTimeForAI);
        getAIAnswer();

        // TODO: - jiaqiga. temp mock here
        //getHumanAnswer(true);
    }

    IEnumerator checkResult()
    {
        // show the result waiting screen
        solution.text = "This correct answer is " + (curSolution ? "human" : "AI");
        questionPanel.SetActive(false);
        resultPanel.SetActive(false);
        waitSolutionPanel.SetActive(true);

        thinkingAudio.Stop();
        beforeSolutionAudio.Play();
        yield return new WaitForSeconds(2);
        beforeSolutionAudio.Stop();

        waitSolutionPanel.SetActive(false);
        resultPanel.SetActive(true);

        // check AI
        if(curAIAnswer != curSolution)
        {
            AIWrongOperation();
        }

        // check Human
        if(curHumanChoice != curSolution)
        {
            humanWrongOperation();
            StartCoroutine(humanWrongReaction());
        } else
        {
            StartCoroutine(humanCorrectReaction());
        }

        yield return new WaitForSeconds(5f);
        if (AILife != 0 && humanLife != 0)
        {
            // if run out of the question
            if (curRound == maxRound)
            {
                Debug.Log("draw!! No one won!");
                winner.text = "Draw!!";
                gameOver();
            }
            else
            {
                StartCoroutine(startNextQuestion());
            }
        } else
        {
            if (AILife == 0 && humanLife == 0)
            {
                Debug.Log("draw!");
                winner.text = "Draw!!";
            }
            else if (AILife == 0)
            {
                Debug.Log("AI Lost!");
                winner.text = "Human WON!!";
            }
            else if (humanLife == 0)
            {
                Debug.Log("Human Lost!");
                winner.text = "AI WON!!";
            }

            gameOver();
        }
    }

    private void AIWrongOperation()
    {
        // change model or trigger animation
        if(AILife == 1)
        {
            AIGlasses[3 - AILife].gameObject.GetComponent<Animator>().SetBool("breakai", true);
            AILife -= 1;
        } else
        {
            AIGlasses[3 - AILife + 1].gameObject.SetActive(true);
            AIGlasses[3 - AILife].gameObject.SetActive(false);
            AILife -= 1;
        }

        Debug.Log("AI Wrong! life now: " + AILife.ToString());

        glassBreak.Play();
    }

    private void humanWrongOperation()
    {
        // change model or trigger animation
        if (humanLife == 1)
        {
            humanGlasses[3 - humanLife].gameObject.GetComponent<Animator>().SetBool("break", true);
            humanLife -= 1;
        }
        else
        {
            humanGlasses[3 - humanLife + 1].gameObject.SetActive(true);
            humanGlasses[3 - humanLife].gameObject.SetActive(false);
            humanLife -= 1;
        }

        Debug.Log("Human Wrong! life now: " + humanLife.ToString());

        glassBreak.Play();
    }

    private void gameOver()
    {
        Debug.Log("Game OVer!");

        resultPanel.SetActive(false);
        gameoverPanel.SetActive(true);

        if (humanLife > 0)
        {
            wonAudio.Play();
        }
        else
        {
            lostAudio.Play();
        }

        StartCoroutine(gameOverLast());
    }

    IEnumerator gameOverLast()
    {
        yield return new WaitForSeconds(2f);
        endPanel.SetActive(true);
    }

    IEnumerator humanCorrectReaction()
    {
        yield return new WaitForSeconds(1f);
        cheerAudio.Play();

        foreach(GameObject peopleGroup in people)
        {
            foreach (Transform child in peopleGroup.transform)
            {
                StartCoroutine(OnePersonJumpRoutine(child));
            }
        }

        
    }

    IEnumerator humanWrongReaction()
    {
        yield return new WaitForSeconds(1f);
        booAudio.Play();
    }

    IEnumerator OnePersonJumpRoutine(Transform human, int times = 6)
    {
        for(int i = 0; i < times; ++i)
        {
            Vector3 startPosition = human.position;
            float jumpHeight = Random.Range(1f, 11f);
            float duration = Random.Range(0.3f, 1f);
            float elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / duration;
                float deltaY = 4 * (normalizedTime - normalizedTime * normalizedTime) * jumpHeight;
                human.position = new Vector3(startPosition.x, startPosition.y + deltaY, startPosition.z);

                yield return null;
            }

            human.position = startPosition;
        }
    }
}
