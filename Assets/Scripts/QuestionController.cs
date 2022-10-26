using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionController : MonoBehaviour
{
    UIController _viewController;
    QuestCollection _collection;
    public QuestionModel currentQuestion;

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
        StartCoroutine(_viewController.DisplayMoneyTree());
        currentQuestion = _collection.GetUnaskedQuestion(QuestionModel.QuestionType.Unlock);
        _viewController.SetUpUI(currentQuestion);
        yield return new WaitForSeconds(5f);
        StartCoroutine(_viewController.DisplayQuestionAndAnswer());
    }

    public void SubmitAnswer(int finalAnswer)
    {
        bool isCorrect = _viewController.answersButton[finalAnswer].transform
                            .GetChild(0).GetComponent<TextMeshProUGUI>()
                            .text.Equals(currentQuestion.answers[0]);
        Debug.Log(isCorrect);
        _viewController.HandleFinalAnswer(isCorrect);
    }
}
