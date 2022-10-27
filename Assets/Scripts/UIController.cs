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
        questionTMPUGUI.text = question.question.Replace("\\n", "\n");
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
        LeanTween.moveLocalX(moneyTree, 640f, 1f);
        yield return new WaitForSeconds(3f);
        //LeanTween.moveLocalX(moneyTree, 1360f, 1f);
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
        int siblingIndex = button.transform.GetSiblingIndex();
        button.transform.GetComponent<Image>().sprite
            = siblingIndex == 0 ? slicedSprite[2] : slicedSprite[10];
    }

    public void HandleFinalAnswer(bool isCorrect)
    {
        EnableButtons(false);

        foreach (Button button in answersButton)
        {
            if (button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
                                .Equals(questionController.currentQuestion.answers[0]))
            {
                StartCoroutine(PlayAnimation(button, isCorrect));
            }
        }
    }

    IEnumerator PlayAnimation(Button button, bool isCorrect)
    {
        yield return new WaitForSeconds(1.5f);
        button.transform.GetComponent<Animator>().enabled = true;
        button.transform.GetComponent<Animator>().SetTrigger(isCorrect ? "Correct" : "Incorrect");
        yield return new WaitForSeconds(delayAfterFinalAnswer);
        button.transform.GetComponent<Animator>().enabled = false;

        ResetButton();
        if (isCorrect)
            StartCoroutine(DisplayPrize());
        else
            ResetLayout();
    }

    IEnumerator DisplayPrize()
    {
        questionContainer.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        LeanTween.moveLocalY(answersContainer, -(Screen.height + answersContainer.transform.GetComponent<RectTransform>().rect.height), 0.5f);
        LeanTween.rotateX(questionContainer, 360f, 0.5f).setOnComplete(() =>
        {
            questionContainer.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        });
        LeanTween.moveLocalY(questionContainer, -320f, 0.5f);
        yield return new WaitForSeconds(delayAfterFinalAnswer);
        LeanTween.moveLocalY(questionContainer, -(Screen.height + questionContainer.transform.GetComponent<RectTransform>().rect.height), 0.5f);
        yield return new WaitForSeconds(1f);
        questionContainer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        questionContainer.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    public void Use50Lifeline()
    {
        int randomInt = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < answersButton.Length; i++)
        {
            if (answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
                                .Equals(questionController.currentQuestion.answers[0]))
            {
                if (randomInt == i) randomInt += 1;
                Debug.Log(randomInt);
            }

            if (i != randomInt &&
                !answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
                .Equals(questionController.currentQuestion.answers[0]))
            {
                answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    void ResetLayout()
    {
        LeanTween.moveLocalY(questionContainer, -(Screen.height + questionContainer.transform.GetComponent<RectTransform>().rect.height), 1f);
        LeanTween.moveLocalY(answersContainer, -(Screen.height + answersContainer.transform.GetComponent<RectTransform>().rect.height), 1f);
    }

    void ResetButton()
    {
        for (int i = 0; i < answersButton.Length; i++)
        {
            int siblingIndex = answersButton[i].transform.GetSiblingIndex();
            answersButton[i].transform.GetComponent<Image>().sprite
            = siblingIndex == 0 ? slicedSprite[0] : slicedSprite[8];
        }

        EnableButtons(true);
    }

    void EnableButtons(bool value)
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

    QuestionController questionController;
    float delayAfterFinalAnswer = 3f;
}

