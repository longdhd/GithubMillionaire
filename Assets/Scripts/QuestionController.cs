using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionController : MonoBehaviour
{
    [HideInInspector] public QuestionModel currentQuestion;
    [HideInInspector] public QuestCollection _collection;
    [HideInInspector] public UIController _viewController;
    [HideInInspector] public FiftyLifeline _fiftyLifeline;
    [HideInInspector] public SwitchLifeline _switchLifeline;
    [HideInInspector] public AudienceLifeline _audienceLifeline;
    [SerializeField] public GameStateManager gameStateManager;
    [SerializeField] public ActionOnTimer actionOnTimer;
    [SerializeField] GameObject InstructionGO;
    [SerializeField] GameObject PlayAgainGO;
    public static QuestionController Instance;
    public int currentLevel;
    public bool IsUnlocked { get { return isUnlocked; } set { isUnlocked = value; } }
    bool isUnlocked = false;
    QuestionType questionType;

    void Awake()
    {
        _viewController = FindObjectOfType<UIController>();
        _collection = FindObjectOfType<QuestCollection>();
        Instance = this;
    }

    void Start()
    {
        currentLevel = -2;
        _fiftyLifeline = new FiftyLifeline
        {
            Quantity = 1
        };

        _switchLifeline = new SwitchLifeline()
        {
            Quantity = 1
        };
        _audienceLifeline = new AudienceLifeline()
        {
            Quantity = 1
        };
        StartCoroutine(PresentQuestion());

    }

    IEnumerator PresentQuestion()
    {
        if (currentLevel == 1 || currentLevel == 5
            || currentLevel == 10 || currentLevel == 15)
            StartCoroutine(_viewController.DisplayMoneyTree());

        AddUpLifeline();

        GetQuestionType();

        currentQuestion = _collection.GetUnaskedQuestion(questionType);
        _viewController.SetUpUI(currentQuestion);
        yield return new WaitForSeconds(3f);
        StartCoroutine(_viewController.DisplayQuestionAndAnswer());
        yield return new WaitForSeconds(4f);

        GameIdleState idleState = gameStateManager.IdleState;
        gameStateManager.SwitchState(idleState);

        if (isUnlocked)
            actionOnTimer.SetTimer(30f, () => { OnTimerAction(); });
        else
        {
            InstructionGO.SetActive(true);
        }
    }

    void OnTimerAction()
    {
        bool hasSubmit = false;
        foreach (Button btn in _viewController.answersButton)
        {
            if (btn.GetComponent<OnClickPointer>().pointerState == OnClickPointer.PointerState.FINAL)
                hasSubmit = true;
        }

        if (!hasSubmit)
        {
            _viewController.EnableButtons(false);
            currentLevel = 1;
            _viewController.ResetLayout();
            _viewController.ResetButton();
            StartCoroutine(NextQuestionAfterDelay());
        }
    }

    public void LockAnswer(int finalAnswer)
    {
        bool isSubmitted = _viewController.answersButton[finalAnswer]
            .GetComponent<OnClickPointer>().pointerState == OnClickPointer.PointerState.FINAL;

        bool isCorrect = _viewController.answersButton[finalAnswer].transform
                            .GetChild(0).GetComponent<TextMeshProUGUI>()
                            .text.Equals(currentQuestion.correctAns);
        if (isSubmitted)
        {
            GameFinalState finalState = gameStateManager.FinalState;
            gameStateManager.SwitchState(finalState);

            actionOnTimer.Stop();

            _viewController.HandleFinalAnswer(isCorrect);
            if (isCorrect)
            {
                if (currentLevel < 15)
                {
                    currentLevel++;
                }
                _viewController.UpdateMoneyTree(currentLevel);
                StartCoroutine(NextQuestionAfterDelay());
            }
            else
            {
                LeanTween.moveLocalY(PlayAgainGO.transform.GetChild(0).gameObject, 0f, 5f).setOnComplete(() => PlayAgainGO.SetActive(true));
            }
        }
    }

    public void Use5050Lifeline()
    {
        if (_fiftyLifeline.Quantity > 0)
        {
            actionOnTimer.Stop();

            GameLifelineState lifelifeState = gameStateManager.LifelineState;
            gameStateManager.SwitchState(lifelifeState);

            QuestionModel tempQuestion = _viewController.GetSortedAnswers();
            QuestionModel newQuestion = _fiftyLifeline.Use(tempQuestion);
            currentQuestion = newQuestion;
            _viewController.SetUpUI(newQuestion, false);
            _viewController.DisableEmptyAnswers();
        }
        else
            return;
    }

    public void UseSwitchLifeline()
    {
        if (_switchLifeline.Quantity > 0)
        {
            GameLifelineState lifelifeState = gameStateManager.LifelineState;
            gameStateManager.SwitchState(lifelifeState);

            actionOnTimer.Stop();

            QuestionModel newQuestion = _switchLifeline.Use(currentQuestion);
            if (newQuestion != null)
            {
                currentQuestion = newQuestion;
                _viewController.SetUpUI(newQuestion);
            }
            else
                throw new UnityException("newQuestion is null");
        }
    }

    public void UseAudienceLifeline()
    {
        if (_audienceLifeline.Quantity > 0)
        {
            GameLifelineState lifelifeState = gameStateManager.LifelineState;
            gameStateManager.SwitchState(lifelifeState);

            actionOnTimer.Stop();

            int availableAnswers = 0;
            int correctAnswerIndex = 0;
            for (int i = 0; i < _viewController.answersButton.Length; i++)
            {
                if (!string.IsNullOrEmpty(_viewController.answersButton[i].transform
                    .GetChild(0).GetComponent<TextMeshProUGUI>().text))
                    availableAnswers++;

                if (_viewController.answersButton[i].transform
                    .GetChild(0).GetComponent<TextMeshProUGUI>().text.Equals(currentQuestion.correctAns))
                    correctAnswerIndex = i;
            }

            bool isUsing5050 = availableAnswers == 2 ? true : false;
            int[] results = _audienceLifeline.Use(correctAnswerIndex, isUsing5050);
            _viewController.DisplayAudiencePanel(results);
        }
    }

    public void PlayAgain()
    {
        if (isUnlocked)
            currentLevel = 1;
        else
            currentLevel = -2;

        PlayAgainGO.SetActive(false);
        PlayAgainGO.transform.GetChild(0).LeanMoveY
            (-(PlayAgainGO.transform.GetComponent<RectTransform>().rect.height + Screen.height), 0.5f);
        _viewController.UpdateMoneyTree(currentLevel);
        StartCoroutine(NextQuestionAfterDelay());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void AddUpLifeline()
    {
        if (currentLevel == 4 || currentLevel == 9)
            _fiftyLifeline.Quantity += 1;
    }

    void GetQuestionType()
    {
        if (currentLevel < 1)
            questionType = QuestionType.Unlock;
        else if (currentLevel >= 1 && currentLevel < 6)
            questionType = QuestionType.Easy;
        else if (currentLevel >= 6 && currentLevel < 11)
            questionType = QuestionType.Medium;
        else if (currentLevel >= 11 && currentLevel < 16)
            questionType = QuestionType.Hard;
    }

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        StartCoroutine(PresentQuestion());
    }
}