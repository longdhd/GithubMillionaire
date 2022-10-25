using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    void Awake()
    {
        questCollection = FindObjectOfType<QuestCollection>();

        //Add listener
        for (int i = 0; i < answersButton.Length; i++)
        {
            var index = i;
            answersButton[i].onClick.AddListener(() =>
            {
                HandleFinalAnswer(answersButton[index]);
            });
        }
    }

    void Start()
    {
        SetUpUI(questCollection.GetUnaskedQuestion(QuestionModel.QuestionType.Unlock));
        StartCoroutine(DisplayQuestionAndAnswer());
    }

    public void SetUpUI(QuestionModel question)
    {
        questionTMPUGUI.text = question.question;
        string[] copiedArr = new string[question.answers.Length];
        Array.Copy(question.answers, copiedArr, question.answers.Length);
        Array.Sort(copiedArr, 0, UnityEngine.Random.Range(1, question.answers.Length));
        for (int i = 0; i < question.answers.Length; i++)
        {
            answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = copiedArr[i];
        }
    }

    IEnumerator DisplayMoneyTree()
    {
        LeanTween.moveLocalX(moneyTree, 680f, 1f);
        yield return new WaitForSeconds(3f);
        LeanTween.moveLocalX(moneyTree, 1360f, 1f);
    }

    IEnumerator DisplayQuestionAndAnswer()
    {
        LeanTween.moveLocalY(questionContainer, -320f, 0.5f);
        yield return new WaitForSeconds(3f);
        LeanTween.moveLocalY(questionContainer, 0f, 0.5f)
                 .setEase(LeanTweenType.easeOutBounce)
                 .setOnComplete(() =>
                 {
                     LeanTween.moveLocalY(answersContainer, -180f, 0.5f);
                 });
    }

    void HandleFinalAnswer(Button button)
    {
        if(button.transform.GetSiblingIndex() == 0)
            button.transform.GetComponent<Image>().sprite = slicedSprite[2];
        else if(button.transform.GetSiblingIndex() == 1)
            button.transform.GetComponent<Image>().sprite = slicedSprite[10];

        EnableButtons(false);
    }

    public string GetFinalAnswerText(Button button)
    {
        return button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }

    public void RevealCorrectAnswer(bool isCorrect)
    {

    }

    public void EnableButtons(bool value)
    {
        for (int i = 0; i < answersButton.Length; i++)
        {
            answersButton[i].enabled = value;
        }
    }



    [SerializeField]
    TextMeshProUGUI questionTMPUGUI;

    [SerializeField]
    Button[] answersButton;

    [SerializeField]
    GameObject answersContainer;

    [SerializeField]
    GameObject questionContainer;

    [SerializeField]
    GameObject moneyTree;

    [SerializeField]
    List<Sprite> slicedSprite;

    private QuestCollection questCollection;
}

