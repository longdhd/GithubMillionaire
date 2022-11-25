using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System;

public class QuestCollection : MonoBehaviour
{
    private QuestionModel[] allQuestion;

    void Awake()
    {
        LoadAllQuestions();
    }

    void LoadAllQuestions()
    {
        ResetAllQuestions();
        var jsonPath = Application.streamingAssetsPath + "/JSON/questions.json";
        var jsonFile = File.ReadAllText(jsonPath);
        allQuestion = JsonConvert.DeserializeObject<QuestionModel[]>(jsonFile);

        SetQuestionType();
        SetCorrectAnswer();
    }

    public QuestionModel GetUnaskedQuestion(QuestionType questionType)
    {
        QuestionModel unasked;
        switch (questionType)
        {
            case QuestionType.Unlock:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionType.Unlock)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;

            case QuestionType.Easy:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionType.Easy)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;

            case QuestionType.Medium:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionType.Medium)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault(); ;
                unasked.asked = true;
                return unasked;

            case QuestionType.Hard:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionType.Hard)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault(); ;
                unasked.asked = true;
                return unasked;

            default:
                return null;
        }
    }

    void SetQuestionType()
    {
        foreach (QuestionModel question in allQuestion)
        {
            question.Type = (QuestionType)question.questionType;
        }
    }

    void SetCorrectAnswer()
    {
        if (allQuestion.Any(t => String.IsNullOrEmpty(t.correctAns)))
        {
            foreach (var question in allQuestion)
                question.correctAns = question.answers[0];
        }

    }

    public void ResetAllQuestions()
    {
        //if (allQuestion?.Any((t => t.asked == false)) == false)
        //{
        if (allQuestion != null)
        {
            foreach (var question in allQuestion)
                question.asked = false;
        }
        //}
    }
}