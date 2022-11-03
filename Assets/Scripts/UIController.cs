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
        lifelineContainer = moneyTree.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < answersButton.Length; i++)
        {
            var index = i;
            answersButton[index].onClick.AddListener(() =>
            {
                LockAndSetFinalAnswer(answersButton[index]);
            });
        }

    }

    void Update()
    {
        lifelineContainer.transform.GetChild(0).GetComponent<Image>().sprite
            = questionController._fiftyLifeline.Quantity > 0 ? fiftyLifelineSprite[1] : fiftyLifelineSprite[0];
    }

    public void SetUpUI(QuestionModel question, bool randomOrderAnswers = true)
    {
        questionTMPUGUI.text = question.question.Replace("\\n", "\n").Replace("'", "");

        //Should the answers be arranged ramdomly
        if (randomOrderAnswers)
        {
            string[] copiedArr = new string[question.answers.Length];
            Array.Copy(question.answers, copiedArr, question.answers.Length);
            Array.Sort(copiedArr, 0, UnityEngine.Random.Range(1, question.answers.Length));
            for (int i = 0; i < question.answers.Length; i++)
            {
                answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = copiedArr[i];
            }
        }
        else
        {
            for (int i = 0; i < question.answers.Length; i++)
            {
                answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = question.answers[i];
            }
        }

        LeanTween.moveX(audiencePanel, 0f, 1f);
    }

    public IEnumerator DisplayMoneyTree()
    {
        LeanTween.moveLocalX(moneyTree, 640f, 1f);
        yield return new WaitForSeconds(3f);
        LeanTween.moveLocalX(moneyTree, 1360f, 1f);
    }

    public void ToggleMoneyTree()
    {
        bool isOn = moneyTree.transform.position.x == 1600f;
        LeanTween.moveLocalX(moneyTree, isOn ? 1360f : 640f, 1f);
    }

    public void ClimbUpMoneyTree()
    {
        if (questionController.currentLevel > 0)
        {
            Vector3 currentPos = currentPrize.transform.position;
            float offset = currentPrize.rect.height;
            float newY = currentPos.y + offset;
            Vector3 newPos = new Vector3(currentPos.x, newY, currentPos.z);
            currentPrize.transform.position = newPos;
        }
    }
    public IEnumerator DisplayQuestionAndAnswer()
    {
        LeanTween.moveLocalY(questionContainer, -240f, 0.5f);
        yield return new WaitForSeconds(3f);
        LeanTween.moveLocalY(questionContainer, 0f, 0.5f)
                 .setEase(LeanTweenType.easeOutBounce)
                 .setOnComplete(() =>
                 {
                     LeanTween.moveLocalY(answersContainer, -160f, 0.5f);
                 });
    }

    void LockAndSetFinalAnswer(Button button)
    {
        OnClickPointer onClickPointer = button.gameObject.GetComponent<OnClickPointer>();
        if (onClickPointer.pointerState == OnClickPointer.PointerState.LOCK)
        {
            int siblingIndex = button.transform.GetSiblingIndex();
            button.transform.GetComponent<Image>().sprite
                = siblingIndex == 0 ? slicedSprite[0] : slicedSprite[8];

            UnlockOtherAnswers(button);
        }
        else if (onClickPointer.pointerState == OnClickPointer.PointerState.FINAL)
        {
            Debug.Log("Set Final Answer!");
            int siblingIndex = button.transform.GetSiblingIndex();
            button.transform.GetComponent<Image>().sprite
                = siblingIndex == 0 ? slicedSprite[2] : slicedSprite[10];
        }
    }

    void UnlockOtherAnswers(Button finalButton)
    {
        foreach (Button btn in answersButton)
        {
            if (btn != finalButton)
            {
                OnClickPointer onClickBtn = btn.gameObject.GetComponent<OnClickPointer>();
                onClickBtn.pointerState = OnClickPointer.PointerState.IDLE;
                int siblingIndex = btn.transform.GetSiblingIndex();
                btn.transform.GetComponent<Image>().sprite
                    = siblingIndex == 0 ? slicedSprite[3] : slicedSprite[11];
            }
        }
    }

    public void HandleFinalAnswer(bool isCorrect)
    {
        EnableButtons(false);

        foreach (Button button in answersButton)
        {
            if (button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
                                .Equals(questionController.currentQuestion.correctAns))
            {
                StartCoroutine(RevealCorrectAnswer(button, isCorrect));
            }
        }
    }

    IEnumerator RevealCorrectAnswer(Button button, bool isCorrect)
    {
        yield return new WaitForSeconds(delayAfterFinalAnswer);
        button.transform.GetComponent<Animator>().enabled = true;
        button.transform.GetComponent<Animator>().SetTrigger(isCorrect ? "Correct" : "Incorrect");
        yield return new WaitForSeconds(1.5f);
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
        if (questionController.currentLevel > 1)
        {
            LeanTween.rotateX(questionContainer, 360f, 0.5f).setOnComplete(() =>
            {
                questionContainer.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                questionContainer.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = prizeList[questionController.currentLevel - 2].text;
            });
            LeanTween.moveLocalY(questionContainer, -320f, 0.5f);
            yield return new WaitForSeconds(delayAfterFinalAnswer);
        }
        LeanTween.moveLocalY(questionContainer, -(Screen.height + questionContainer.transform.GetComponent<RectTransform>().rect.height), 0.5f);
        yield return new WaitForSeconds(1f);
        questionContainer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        questionContainer.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    public void DisplayAudiencePanel(int[] results)
    {
        LeanTween.moveX(audiencePanel, 1920f, 1f);
        for (int i = 0; i < audienceSlider.Length; i++)
        {
            audienceSlider[i].value = results[i] / 100f;
            audienceSlider[i].transform.GetChild(0).GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = $"{results[i]}%";
        }
    }

    public QuestionModel GetSortedAnswers()
    {
        QuestionModel sortedAnswerQuestion = new QuestionModel()
        {
            question = questionController.currentQuestion.question,
            correctAns = questionController.currentQuestion.correctAns,
            answers = new string[answersButton.Length],
            asked = true
        };

        for (int i = 0; i < answersButton.Length; i++)
        {
            sortedAnswerQuestion.answers[i] = answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        }

        return sortedAnswerQuestion;
    }

    void ResetLayout()
    {
        LeanTween.moveLocalY(questionContainer, -(Screen.height + questionContainer.transform.GetComponent<RectTransform>().rect.height), 1f);
        LeanTween.moveLocalY(answersContainer, -(Screen.height + answersContainer.transform.GetComponent<RectTransform>().rect.height), 1f);
    }

    public void DisableEmptyAnswers()
    {
        for (int i = 0; i < answersButton.Length; i++)
        {
            answersButton[i].enabled
                = !answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Equals("");
        }
    }

    void ResetButton()
    {
        for (int i = 0; i < answersButton.Length; i++)
        {
            int siblingIndex = answersButton[i].transform.GetSiblingIndex();
            answersButton[i].transform.GetComponent<Image>().sprite
            = siblingIndex == 0 ? slicedSprite[3] : slicedSprite[11];

            answersButton[i].GetComponent<OnClickPointer>().pointerState = OnClickPointer.PointerState.IDLE;
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

    GameObject lifelineContainer;

    [SerializeField]
    GameObject moneyTree;

    [SerializeField]
    GameObject audiencePanel;

    [SerializeField]
    RectTransform currentPrize;

    [SerializeField]
    Slider[] audienceSlider;

    [SerializeField]
    List<Sprite> slicedSprite;

    [SerializeField]
    List<Sprite> fiftyLifelineSprite;

    [SerializeField]
    List<TextMeshProUGUI> prizeList;

    [SerializeField]
    List<TextMeshProUGUI> lifelineQuantity;


    QuestionController questionController;
    float delayAfterFinalAnswer = 3f;
}