using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class QuestCollection : MonoBehaviour
{
    private QuestionModel[] availableQuestions;
    private QuestionModel[] SetData(QuestionModel[] data)
    {
        return data;
    }
    public void FetchQuestionsByType(QuestionType type, Action callback)
    {
        int difficulty = (int)type;

        StartCoroutine(
            UwrGetRequest<QuestionModel>($"localhost:8080/api/v1/questions/page?difficulty={difficulty}&number=0&size=25"
                ,callback
                ,(result) =>
                    {
                        availableQuestions = SetData(result);
                    }
                 ));
    }

    public QuestionModel GetUnaskedQuestion()
    {
        Debug.Log(availableQuestions.Length);
        QuestionModel unasked = availableQuestions.Where(t => t.asked == false)
                                                   .OrderBy(t => UnityEngine.Random.Range(0, availableQuestions.Length))
                                                   .FirstOrDefault();
        unasked.asked = true;
        return unasked;
    }

    void CastQuestionType()
    {
        foreach (QuestionModel question in availableQuestions)
        {
            question.Type = (QuestionType)question.difficulty;
        }
    }


    public void ResetAllQuestions()
    {
        //if (allQuestion?.Any((t => t.asked == false)) == false)
        //{
        if (availableQuestions != null)
        {
            foreach (var question in availableQuestions)
                question.asked = false;
        }
        //}
    }

    IEnumerator UwrGetRequest<T>(string uri, Action onSuccess, Action<T[]> handleData)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();

        string[] pages = uri.Split('/');
        int page = pages.Length - 1;

        switch (webRequest.result)
        {

            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log("Fetch sucess");
                var result = webRequest.downloadHandler.text;
                WebResponse<T> deserializedResult = JsonConvert.DeserializeObject<WebResponse<T>>(result);

                T[] tempArr = deserializedResult.data.content;
                handleData(tempArr);

                onSuccess?.Invoke();
                break;
        }
    }

    [Serializable]
    public class WebResponse<T>
    {
        bool success;
        public DataResponse<T> data;
    }

    [Serializable]
    public class DataResponse<T>
    {
        public T[] content;
        int pageNumber;
        int pageSize;
        int totalElements;
    }



}