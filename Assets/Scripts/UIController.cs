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
        questionController = FindObjectOfType<QuestionController>();

        for (int i = 0; i < answersButton.Length; i++)
        {
            var index = i;
            answersButton[index].onClick.AddListener(() =>
            {
                SetFinalAnswer(answersButton[index]);
            });
        }

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

    public IEnumerator DisplayMoneyTree()
    {
        LeanTween.moveLocalX(moneyTree, 680f, 1f);
        yield return new WaitForSeconds(3f);
        LeanTween.moveLocalX(moneyTree, 1360f, 1f);
    }

    public IEnumerator DisplayQuestionAndAnswer()
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

    void SetFinalAnswer(Button button)
    {
        if (button.transform.GetSiblingIndex() == 0)
            button.transform.GetComponent<Image>().sprite = slicedSprite[2];
        else if (button.transform.GetSiblingIndex() == 1)
            button.transform.GetComponent<Image>().sprite = slicedSprite[10];
    }

    public void HandleFinalAnswer(bool isCorrect)
    {
        EnableButtons(false);

        foreach (Button button in answersButton)
        {
            if (button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
                                .Equals(questionController.currentQuestion.answers[0]))
            {
                if (isCorrect)
                {
                    //Flashing text here
                    Color color = Color.green;
                    button.transform.GetComponent<Image>().color = color;
                }
                else
                {
                    //Flashing text here
                    Color color = Color.red;
                    button.transform.GetComponent<Image>().color = color;
                }
            }
        }
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
    public Button[] answersButton;

    [SerializeField]
    GameObject answersContainer;

    [SerializeField]
    GameObject questionContainer;

    [SerializeField]
    GameObject moneyTree;

    [SerializeField]
    List<Sprite> slicedSprite;

    QuestCollection questCollection;
    QuestionController questionController;
}

