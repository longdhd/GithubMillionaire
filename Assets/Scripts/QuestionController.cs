using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionController : MonoBehaviour, IDataPersistence
{
    [HideInInspector] public QuestionModel currentQuestion;
    [HideInInspector] public QuestCollection _collection;
    [HideInInspector] public UIController _viewController;
    [HideInInspector] public FiftyLifeline _fiftyLifeline = new ();
    [HideInInspector] public SwitchLifeline _switchLifeline = new ();
    [HideInInspector] public AudienceLifeline _audienceLifeline = new();
    [SerializeField] public GameStateManager gameStateManager;
    [SerializeField] public ActionOnTimer actionOnTimer;
    [SerializeField] GameObject InstructionGO;
    [SerializeField] GameObject PlayAgainGO;
    public static QuestionController Instance;
    public int currentLevel;
    public bool IsUsing5050 = false;
    QuestionType questionType;

    void Awake()
    {
        _viewController = FindObjectOfType<UIController>();
        _collection = FindObjectOfType<QuestCollection>();
        Instance = this;
    }

    public void BeginGame()
    {
        StartCoroutine(PresentQuestion());
    }

    IEnumerator PresentQuestion()
    {
        Debug.Log("Current Level: " + currentLevel);


        if (currentLevel == 1 || currentLevel == 5
            || currentLevel == 10 || currentLevel == 15)
            StartCoroutine(_viewController.DisplayMoneyTree());

        GetQuestionType();
        Debug.Log("Question Type: " + questionType.ToString());

        currentQuestion = _collection.GetUnaskedQuestion(questionType);
        _viewController.SetUpUI(currentQuestion);
        yield return new WaitForSeconds(3f);
        StartCoroutine(_viewController.DisplayQuestionAndAnswer());
        yield return new WaitForSeconds(4f);

        GameIdleState idleState = gameStateManager.IdleState;
        gameStateManager.SwitchState(idleState);

        actionOnTimer.SetTimer(30f, () => { OnTimerAction(); });
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
            //_viewController.EnableButtons(false);
            //currentLevel = 1;
            //_viewController.ResetLayout();
            //_viewController.ResetButton();
            _viewController.HandleFinalAnswer(false);
            LeanTween.moveLocalY(PlayAgainGO.transform.GetChild(0).gameObject, 0f, 7f).setOnComplete(() => PlayAgainGO.SetActive(true));
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
            if (IsUsing5050) IsUsing5050 = false;

            _viewController.HandleFinalAnswer(isCorrect);
            if (isCorrect)
            {
                AddUpLifeline();

                if (currentLevel < 15)
                {
                    currentLevel++;
                }

                _viewController.UpdateMoneyTree(currentLevel);
                StartCoroutine(NextQuestionAfterDelay());
            }
            else
            {
                LeanTween.moveLocalY(PlayAgainGO.transform.GetChild(0).gameObject, 0f, 7f).setOnComplete(() => PlayAgainGO.SetActive(true));
            }
        }
    }

    public void LoadGame(GameData gameData)
    {
        this.currentLevel = gameData.Level;
        this._fiftyLifeline.Quantity = gameData.FiftyLifeline;
        this._switchLifeline.Quantity = gameData.SwitchLifeline;
        this._audienceLifeline.Quantity = gameData.AudienceLifeline;
    }

    public void SaveGame(ref GameData gameData)
    {
        gameData.Level = this.currentLevel;
        gameData.FiftyLifeline = this._fiftyLifeline.Quantity;
        gameData.SwitchLifeline = this._switchLifeline.Quantity;
        gameData.AudienceLifeline = this._audienceLifeline.Quantity;
    }

    public void Use5050Lifeline()
    {
        if (_fiftyLifeline.Quantity > 0 && !IsUsing5050)
        {
            actionOnTimer.Stop();

            GameLifelineState lifelifeState = gameStateManager.LifelineState;
            gameStateManager.SwitchState(lifelifeState);

            QuestionModel tempQuestion = _viewController.GetSortedAnswers();
            QuestionModel newQuestion = _fiftyLifeline.Use(tempQuestion);
            currentQuestion = newQuestion;
            _viewController.SetUpUI(newQuestion, false);
            _viewController.DisableEmptyAnswers();

            IsUsing5050 = true;
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
            {
                Debug.Log("No answers left when using Switch");
            }
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
        if (currentLevel > 1)
            currentLevel = 1;
        else
            currentLevel = -2;

        _collection.ResetAllQuestions();

        PlayAgainGO.SetActive(false);
        PlayAgainGO.transform.GetChild(0).LeanMoveY
            (-(PlayAgainGO.transform.GetComponent<RectTransform>().rect.height + Screen.height), 0.5f);
        _viewController.UpdateMoneyTree(currentLevel);
        StartCoroutine(NextQuestionAfterDelay());
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    void AddUpLifeline()
    {
        switch(currentLevel)
        {
            case 1:
                _fiftyLifeline.Quantity += 1;
                _switchLifeline.Quantity += 1;
                _audienceLifeline.Quantity += 1;
                break;
            case 11:
                _fiftyLifeline.Quantity += 1;
                break;
            default:
                break;
        }
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