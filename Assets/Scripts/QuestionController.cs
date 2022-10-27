using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionController : MonoBehaviour
{
    [HideInInspector] public QuestionModel currentQuestion;
    UIController _viewController;
    QuestCollection _collection;
    QuestionModel.QuestionType questionType = QuestionModel.QuestionType.Unlock;
    int currentLevel = 1;

    void Awake()
    {
        _viewController = FindObjectOfType<UIController>();
        _collection = FindObjectOfType<QuestCollection>();
    }

    void Start()
    {
        StartCoroutine(PresentQuestion());
    }

    IEnumerator PresentQuestion()
    {
        if(currentLevel == 1 || currentLevel == 4 
            || currentLevel == 9 || currentLevel == 14)
            StartCoroutine(_viewController.DisplayMoneyTree());

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

    public void SubmitAnswer(int finalAnswer)
    {
        bool isCorrect = _viewController.answersButton[finalAnswer].transform
                            .GetChild(0).GetComponent<TextMeshProUGUI>()
                            .text.Equals(currentQuestion.answers[0]);
        
        _viewController.HandleFinalAnswer(isCorrect);

        if(isCorrect)
        {
            if(currentLevel < 19)
                currentLevel++;
            StartCoroutine(NextQuestionAfterDelay());
        }
        else
        {
            currentLevel = 1;
        }
    }

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(8f);
        StartCoroutine(PresentQuestion());
    }
}
