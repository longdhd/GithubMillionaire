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
    [SerializeField] ActionOnTimer actionOnTimer;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Image timerImage;
    [SerializeField] List<Sprite> timerSprites;
    public static QuestionController Instance;
    public int currentLevel;
    QuestionModel.QuestionType questionType;

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

    void Update()
    {
        if (actionOnTimer.GetRemainingTime() >= 0)
            timerText.text = actionOnTimer.GetRemainingTime().ToString();
        else
            timerText.text = string.Empty;

        timerImage.sprite = SetTimerImage();
    }

    Sprite SetTimerImage()
    {
        int imageIndex = 20 * (15 - actionOnTimer.GetRemainingTime());
        string imageName = $"tmr{(imageIndex == 0 ? "00" : (imageIndex >= 100 ? string.Empty : "0"))}{imageIndex}";
        Debug.Log(imageName);
        foreach(Sprite spr in timerSprites)
        {
            if (spr.name.Equals(imageName))
                return spr;
        }

        return null;
    }

    IEnumerator PresentQuestion()
    {
        if (currentLevel == 1 || currentLevel == 5
            || currentLevel == 10 || currentLevel == 15)
            StartCoroutine(_viewController.DisplayMoneyTree());

        if (currentLevel == 4 || currentLevel == 9)
            _fiftyLifeline.Quantity += 1;

        if (currentLevel < 1)
            questionType = QuestionModel.QuestionType.Unlock;
        else if (currentLevel >= 1 && currentLevel < 6)
            questionType = QuestionModel.QuestionType.Easy;
        else if (currentLevel >= 6 && currentLevel < 11)
            questionType = QuestionModel.QuestionType.Medium;
        else if (currentLevel >= 11 && currentLevel < 16)
            questionType = QuestionModel.QuestionType.Hard;

        currentQuestion = _collection.GetUnaskedQuestion(questionType);
        _viewController.SetUpUI(currentQuestion);
        yield return new WaitForSeconds(3f);
        StartCoroutine(_viewController.DisplayQuestionAndAnswer());
        yield return new WaitForSeconds(5f);

        actionOnTimer.SetTimer(15f, () => { StartTimer(); });
        timerText.gameObject.SetActive(true);
        timerImage.gameObject.SetActive(true);
    }

    void StartTimer()
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
            _viewController.HandleFinalAnswer(isCorrect);
            actionOnTimer.Stop();
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
                currentLevel = 1;
            }
        }
    }

    public void Use5050Lifeline()
    {
        if (_fiftyLifeline.Quantity > 0)
        {
            actionOnTimer.Stop();
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

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        timerText.gameObject.SetActive(false);
        timerImage.gameObject.SetActive(false);
        StartCoroutine(PresentQuestion());
    }
}