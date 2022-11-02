using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionController : MonoBehaviour
{
    [HideInInspector] public QuestionModel currentQuestion;
    public static QuestionController Instance;
    public QuestCollection _collection;
    public int currentLevel;
    public UIController _viewController;
    QuestionModel.QuestionType questionType;
    [HideInInspector] public FiftyLifeline _fiftyLifeline;
    [HideInInspector] SwitchLifeline _switchLifeline;
    [HideInInspector] AudienceLifeline _audienceLifeline;

    void Awake()
    {
        _viewController = FindObjectOfType<UIController>();
        _collection = FindObjectOfType<QuestCollection>();
        Instance = this;
    }

    void Start()
    {
        currentLevel = 4;
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
        if (currentLevel == 1 || currentLevel == 4
            || currentLevel == 9 || currentLevel == 14)
            StartCoroutine(_viewController.DisplayMoneyTree());

        if (currentLevel == 4 || currentLevel == 9)
            _fiftyLifeline.Quantity += 1;

        if (currentLevel < 4)
            questionType = QuestionModel.QuestionType.Unlock;
        else if (currentLevel >= 4 && currentLevel < 9)
            questionType = QuestionModel.QuestionType.Easy;
        else if (currentLevel >= 9 && currentLevel < 14)
            questionType = QuestionModel.QuestionType.Medium;
        else if (currentLevel >= 14 && currentLevel < 19)
            questionType = QuestionModel.QuestionType.Hard;

        currentQuestion = _collection.GetUnaskedQuestion(questionType);
        _viewController.SetUpUI(currentQuestion);
        yield return new WaitForSeconds(5f);
        StartCoroutine(_viewController.DisplayQuestionAndAnswer());
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
            if (isCorrect)
            {
                if (currentLevel < 19)
                    currentLevel++;
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
        QuestionModel newQuestion = _switchLifeline.Use(currentQuestion);
        if (newQuestion != null)
        {
            currentQuestion = newQuestion;
            _viewController.SetUpUI(newQuestion);
        }
        else
            throw new UnityException("newQuestion is null");
    }

    public void UseAudienceLifeline()
    {
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

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(8f);
        StartCoroutine(PresentQuestion());
    }
}