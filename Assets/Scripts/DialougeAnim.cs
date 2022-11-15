using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialougeAnim : MonoBehaviour
{
    [SerializeField] List<string> TextToDisplay;
    [SerializeField] TextMeshProUGUI DialougeText;
    [SerializeField] Button closeButton;
    int currentStep = 0;
    void OnEnable()
    {
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(DisplayText());
    }

    public void Hide()
    {
        closeButton.gameObject.SetActive(false);
        StopAllCoroutines();

        if (currentStep < TextToDisplay.Count - 1)
        {
            currentStep++;
            Show();
        }
        else
        {
            gameObject.SetActive(false);
            QuestionController.Instance.IsUnlocked = true;
        }
    }

    IEnumerator DisplayText()
    {
        DialougeText.text = "";

        foreach (var c in TextToDisplay[currentStep].ToCharArray())
        {
            DialougeText.text += c;

            if (DialougeText.text.Equals(TextToDisplay[currentStep]))
            {
                closeButton.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
