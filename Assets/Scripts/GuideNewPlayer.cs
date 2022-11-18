using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GuideNewPlayer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] List<string> TextToDisplay;
    [SerializeField] List<GameObject> ObjectToDisplay;
    [SerializeField] TextMeshProUGUI DialougeText;

    int currentStep = 0;
    bool hasFinished = false;

    void OnEnable()
    {
        StartCoroutine(DisplayText());
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (hasFinished)
        {
            if (currentStep < TextToDisplay.Count - 1)
            {
                currentStep++;
                ObjectToDisplay[currentStep - 1].SetActive(false);
                StartCoroutine(DisplayText());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            StopAllCoroutines();

            DialougeText.text = TextToDisplay[currentStep];

            hasFinished = true;
        }
    }

    IEnumerator DisplayText()
    {
        hasFinished = false;

        DialougeText.text = "";

        ObjectToDisplay[currentStep].SetActive(true);

        foreach (var c in TextToDisplay[currentStep].ToCharArray())
        {
            DialougeText.text += c;

            if (DialougeText.text.Equals(TextToDisplay[currentStep]))
            {
                hasFinished = true;

            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
