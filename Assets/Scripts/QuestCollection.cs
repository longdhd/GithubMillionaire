using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System;

public class QuestCollection : MonoBehaviour
{
    [SerializeField] int unlockQuestionIndex;
    [SerializeField] int easyQuestionIndex;
    [SerializeField] int mediumQuestionIndex;
    [SerializeField] int hardQuestionIndex;

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

        SetQuestionType();
        SetCorrectAnswer();
    }

    public QuestionModel GetUnaskedQuestion(QuestionModel.QuestionType questionType)
    {
        QuestionModel unasked;
        switch (questionType)
        {
            case QuestionModel.QuestionType.Unlock:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionModel.QuestionType.Unlock)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;

            case QuestionModel.QuestionType.Easy:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionModel.QuestionType.Easy)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;

            case QuestionModel.QuestionType.Medium:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionModel.QuestionType.Medium)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault(); ;
                unasked.asked = true;
                return unasked;

            case QuestionModel.QuestionType.Hard:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionModel.QuestionType.Hard)
                                                    .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault(); ;
                unasked.asked = true;
                return unasked;

            default:
                unasked = allQuestion.Where(t => t.asked == false && t.Type == QuestionModel.QuestionType.Unlock)
                                                     .OrderBy(t => UnityEngine.Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;
        }
    }

    void SetQuestionType()
    {
        foreach (QuestionModel question in allQuestion)
        {
            question.Type = (QuestionModel.QuestionType)question.questionType;
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

    void ResetAllQuestions()
    {
        if (allQuestion?.Any((t => t.asked == false)) == false)
        {
            foreach (var question in allQuestion)
                question.asked = false;
        }
    }
}