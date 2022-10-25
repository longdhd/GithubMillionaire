using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

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

        SortQuestionType();
    }

    public QuestionModel GetUnaskedQuestion(QuestionModel.QuestionType questionType)
    {
        QuestionModel unasked;
        switch (questionType)
        {
            case QuestionModel.QuestionType.Unlock:
                unasked = allQuestion.Where(t => t.asked == false && t.questionType == QuestionModel.QuestionType.Unlock)
                                                    .OrderBy(t => Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;

            case QuestionModel.QuestionType.Easy:
                unasked = allQuestion.Where(t => t.asked == false && t.questionType == QuestionModel.QuestionType.Easy)
                                                    .OrderBy(t => Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;

            case QuestionModel.QuestionType.Medium:
                unasked = allQuestion.Where(t => t.asked == false && t.questionType == QuestionModel.QuestionType.Medium)
                                                    .OrderBy(t => Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault(); ;
                unasked.asked = true;
                return unasked;

            case QuestionModel.QuestionType.Hard:
                unasked = allQuestion.Where(t => t.asked == false && t.questionType == QuestionModel.QuestionType.Hard)
                                                    .OrderBy(t => Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault(); ;
                unasked.asked = true;
                return unasked;

            default:
                unasked = allQuestion.Where(t => t.asked == false && t.questionType == QuestionModel.QuestionType.Unlock)
                                                     .OrderBy(t => Random.Range(0, allQuestion.Length - 1))
                                                    .FirstOrDefault();
                unasked.asked = true;
                return unasked;
        }
    }

    void SortQuestionType()
    {
        for (int i = 0; i < allQuestion.Length; i++)
        {
            if (i <= unlockQuestionIndex)
                allQuestion[i].questionType = QuestionModel.QuestionType.Unlock;
            else if (i <= easyQuestionIndex && i > unlockQuestionIndex)
                allQuestion[i].questionType = QuestionModel.QuestionType.Easy;
            else if (i <= mediumQuestionIndex && i > easyQuestionIndex)
                allQuestion[i].questionType = QuestionModel.QuestionType.Medium;
            else if(i <= mediumQuestionIndex - 1 && i > mediumQuestionIndex)
                allQuestion[i].questionType = QuestionModel.QuestionType.Hard;
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
