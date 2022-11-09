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
        UpdateLifeline();

        if (questionController.actionOnTimer.GetRemainingTime() >= 0)
            timerText.text = questionController.actionOnTimer.GetRemainingTime().ToString();
        else
            timerText.text = string.Empty;

        timerImage.sprite = SetTimerImage();
    }

    public void SetUpUI(QuestionModel question, bool randomOrderAnswers = true)
    {
        questionTMPUGUI.text = question.question.Replace("\\n", "\n").Replace("'", string.Empty);

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
        yield return new WaitForSeconds(1f);
        timerText.gameObject.SetActive(true);
        timerImage.gameObject.SetActive(true);
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
        GameCorrectState correctState = questionController.gameStateManager.CorrectState;
        GameIncorrectState incorrectState = questionController.gameStateManager.IncorrectState;
        questionController.gameStateManager.SwitchState(isCorrect ? correctState : incorrectState);

        button.transform.GetComponent<Animator>().enabled = true;
        button.transform.GetComponent<Animator>().SetTrigger(isCorrect ? "Correct" : "Incorrect");
        yield return new WaitForSeconds(1.5f);
        button.transform.GetComponent<Animator>().enabled = false;

        timerText.gameObject.SetActive(false);
        timerImage.gameObject.SetActive(false);

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

    void UpdateLifeline()
    {
        lifelineContainer.transform.GetChild(0).GetComponent<Image>().sprite
            = questionController._fiftyLifeline.Quantity > 0 ? fiftyLifelineSprite[1] : fiftyLifelineSprite[0];
        lifelineContainer.transform.GetChild(0).GetChild(0).gameObject.SetActive(questionController._fiftyLifeline.Quantity > 0 ? true : false);
        lifelineContainer.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = questionController._fiftyLifeline.Quantity > 1 ? questionController._fiftyLifeline.Quantity.ToString() : string.Empty;

        lifelineContainer.transform.GetChild(1).GetComponent<Image>().sprite
            = questionController._switchLifeline.Quantity > 0 ? switchLifelineSprite[1] : switchLifelineSprite[0];
        lifelineContainer.transform.GetChild(1).GetChild(0).gameObject.SetActive(questionController._switchLifeline.Quantity > 0 ? true : false);
        lifelineContainer.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = questionController._switchLifeline.Quantity > 1 ? questionController._switchLifeline.Quantity.ToString() : string.Empty;

        lifelineContainer.transform.GetChild(2).GetComponent<Image>().sprite
            = questionController._audienceLifeline.Quantity > 0 ? audienceLifelineSprite[1] : audienceLifelineSprite[0];
        lifelineContainer.transform.GetChild(2).GetChild(0).gameObject.SetActive(questionController._audienceLifeline.Quantity > 0 ? true : false);
        lifelineContainer.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = questionController._audienceLifeline.Quantity > 1 ? questionController._audienceLifeline.Quantity.ToString() : string.Empty;
    }

    Sprite SetTimerImage()
    {
        int imageIndex = 20 * (15 - questionController.actionOnTimer.GetRemainingTime());
        string imageName = $"tmr{(imageIndex == 0 ? "00" : (imageIndex >= 100 ? string.Empty : "0"))}{imageIndex}";
        //Debug.Log(imageName);
        foreach (Sprite spr in timerSprites)
        {
            if (spr.name.Equals(imageName))
                return spr;
        }

        return timerSprites[0];
    }

    public void UpdateMoneyTree(int currentLevel)
    {
        if (currentLevel > 0)
        {
            Vector3 initialPos = new Vector3(currentPrize.transform.position.x, 53.2f, currentPrize.transform.position.z);
            float offset = currentPrize.rect.height;
            float newY = initialPos.y + offset * currentLevel;
            Vector3 newPos = new Vector3(initialPos.x, newY, initialPos.z);
            currentPrize.transform.position = newPos;
        }

        int childCount = diamondContainer.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            if (i + currentLevel > childCount)
                diamondContainer.transform.GetChild(i).gameObject.SetActive(true);
            else
                diamondContainer.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ResetLayout()
    {
        LeanTween.moveLocalY(questionContainer, -(Screen.height + questionContainer.transform.GetComponent<RectTransform>().rect.height), 1f);
        LeanTween.moveLocalY(answersContainer, -(Screen.height + answersContainer.transform.GetComponent<RectTransform>().rect.height), 1f);
    }

    public void DisableEmptyAnswers()
    {
        for (int i = 0; i < answersButton.Length; i++)
        {
            answersButton[i].enabled
                = !answersButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Equals(string.Empty);
        }
    }

    public void ResetButton()
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

    GameObject lifelineContainer;

    [SerializeField]
    GameObject diamondContainer;

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
    List<Sprite> switchLifelineSprite;

    [SerializeField]
    List<Sprite> audienceLifelineSprite;

    [SerializeField]
    List<TextMeshProUGUI> prizeList;

    [SerializeField] 
    TextMeshProUGUI timerText;

    [SerializeField] 
    Image timerImage;

    [SerializeField] 
    List<Sprite> timerSprites;

    QuestionController questionController;
    float delayAfterFinalAnswer = 3f;
}