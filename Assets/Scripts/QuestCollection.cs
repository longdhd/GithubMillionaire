using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class QuestCollection : MonoBehaviour
{
    private QuestionModel[] allQuestion;
    private QuestionModel[] availableQuestions;

    void Awake()
    {
        //GetAllQuestions();
    }

    //void LoadAllQuestions()
    //{
    //    ResetAllQuestions();
    //    var jsonPath = Application.streamingAssetsPath + "/JSON/questions.json";
    //    var jsonFile = File.ReadAllText(jsonPath);
    //    allQuestion = JsonConvert.DeserializeObject<QuestionModel[]>(jsonFile);

    //    CastQuestionType();
    //}

    void GetAllQuestions()
    {
        StartCoroutine(GetRequest("localhost:8080/api/v1/questions", null));
    }

    public void GetQuestionsByType(QuestionType type, Action callback)
    {
        int difficulty;
        switch (type)
        {
            case QuestionType.Unlock:
                difficulty = 0;
                break;
            case QuestionType.Easy:
                difficulty = 1;
                break;
            case QuestionType.Medium:
                difficulty = 2;
                break;
            case QuestionType.Hard:
                difficulty = 3;
                break;
            default:
                difficulty = 0;
                break;
        }
        StartCoroutine(GetRequest($"localhost:8080/api/v1/questions/page?difficulty={difficulty}&number=0&size=25", callback));
    }

    public async void AsyncGetQuestionByType(QuestionType type)
    {
        int difficulty;
        switch (type)
        {
            case QuestionType.Unlock:
                difficulty = 0;
                break;
            case QuestionType.Easy:
                difficulty = 1;
                break;
            case QuestionType.Medium:
                difficulty = 2;
                break;
            case QuestionType.Hard:
                difficulty = 3;
                break;
            default:
                difficulty = 0;
                break;
        }
        availableQuestions = await AsyncGetRequest($"localhost:8080/api/v1/questions/page?difficulty={difficulty}&number=0&size=25");
        Debug.Log("await here");
    }

    public QuestionModel GetUnaskedQuestion()
    {
        QuestionModel unasked = availableQuestions.Where(t => t.asked == false)
                                                   .OrderBy(t => UnityEngine.Random.Range(0, availableQuestions.Length - 1))
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

    IEnumerator GetRequest(string uri, Action callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
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
                    var result = webRequest.downloadHandler.text;
                    WebResponse deserializedResult = JsonConvert.DeserializeObject<WebResponse>(result);
                    List<QuestionModel> tempList = new List<QuestionModel>();
                    foreach (QuestionModel q in deserializedResult.data.content)
                    {
                        tempList.Add(q);
                    }
                    availableQuestions = tempList.ToArray();

                    CastQuestionType();
                    Debug.Log("Fetch api !");
                    if (callback != null) { callback(); }
                    break;
            }
        }
    }

    public async Task<QuestionModel[]> AsyncGetRequest(string uri)
    {
        string[] pages = uri.Split('/');
        int page = pages.Length - 1;
        UnityWebRequest request = UnityWebRequest.Get(uri);
        request.SendWebRequest();
        while (!request.isDone)
        {
            await Task.Yield();
        }

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(pages[page] + ": Error: " + request.error);
                return null;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(pages[page] + ": HTTP Error: " + request.error);
                return null;
            case UnityWebRequest.Result.Success:
                var result = request.downloadHandler.text;
                WebResponse deserializedResult = JsonConvert.DeserializeObject<WebResponse>(result);
                List<QuestionModel> tempList = new List<QuestionModel>();
                foreach (QuestionModel q in deserializedResult.data.content)
                {
                    tempList.Add(q);
                }

                Debug.Log("Fetch api !");
                return tempList.ToArray();
            default:
                return null;
        }
    }

    [Serializable]
    public class WebResponse
    {
        bool success;
        public DataResponse data;
    }

    [Serializable]
    public class DataResponse
    {
        public QuestionModel[] content;
        int pageNumber;
        int pageSize;
        int totalElements;
    }



}