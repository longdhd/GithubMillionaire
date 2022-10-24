using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class QuestCollection : MonoBehaviour
{
    QuestionModel[] allQuestion;

    void Awake()
    {
        LoadAllQuestions();
    }

    void LoadAllQuestions()
    {
        ResetAllQuestions(); 

        var jsonPath = Application.dataPath + "/JSON/questions.json";
        var jsonFile = File.ReadAllText(jsonPath);
        allQuestion = JsonConvert.DeserializeObject<QuestionModel[]>(jsonFile);
    }

    public QuestionModel GetUnaskedQuestion()
    {
        var unasked = (QuestionModel)allQuestion.Where(t => t.asked == false);

        unasked.asked = true;

        return unasked;
    }

    void ResetAllQuestions()
    {
        if(allQuestion.Any((t => t.asked == false)) == false)
        {
            foreach(var question in allQuestion)
                question.asked = false;
        }
    }
}
