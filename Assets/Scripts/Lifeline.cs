using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public interface Lifeline
{
    int Quantity { get; set; }
    QuestionModel Use(QuestionModel question);
    void PlayAudio();
}

public class FiftyLifeline : Lifeline
{
    public int Quantity { get; set; }

    public QuestionModel Use(QuestionModel question)
    {
        //Random an incorrect answer to be kept
        int randomInt = UnityEngine.Random.Range(0, question.answers.Length);
        for (int i = 0; i < question.answers.Length; i++)
        {
            if (question.answers[i].Equals(question.correctAns)
                && randomInt == i
                && randomInt != question.answers.Length - 1)
                randomInt += 1;

            if (i != randomInt
                && !question.answers[i].Equals(question.correctAns))
                question.answers[i] = string.Empty;
        }
        Quantity -= 1;
        return question;
    }

    public void PlayAudio()
    {
        Debug.Log("Play 5050 audio");
    }
}

public class SwitchLifeline : Lifeline
{
    public int Quantity { get; set; }

    public QuestionModel Use(QuestionModel question)
    {
        //Get same difficulty question 
        QuestionType t = question.Type;
        QuestionModel newQuestion = QuestionController.Instance._collection.GetUnaskedQuestion(t);
        Quantity -= 1;
        return newQuestion;
    }

    public void PlayAudio()
    {
        Debug.Log("Play Switch audio");
    }
}

public class AudienceLifeline
{
    public int Quantity { get; set; }
    private int chanceOfCorrectAudience = 90;
    public int[] Use(int correctIndex, bool isUsing5050Lifeline)
    {
        int[] results = new int[4];
        if (isUsing5050Lifeline)
        {
            //Should audience gives wrong answer
            if (UnityEngine.Random.Range(1, 101) > chanceOfCorrectAudience)
            {
                int incorrectPercent = UnityEngine.Random.Range(51, 101);
                results[correctIndex] = 100 - incorrectPercent;
                for (int i = 0; i < QuestionController.Instance.currentQuestion.answers.Length; i++)
                {
                    if (i != correctIndex && String.IsNullOrEmpty(QuestionController.Instance.currentQuestion.answers[i]))
                        results[i] = 0;
                    else if (i != correctIndex && !String.IsNullOrEmpty(QuestionController.Instance.currentQuestion.answers[i]))
                        results[i] = incorrectPercent;
                }
            }
            else
            {
                int correctPercent = UnityEngine.Random.Range(51, 101);
                results[correctIndex] = correctPercent;
                for (int i = 0; i < QuestionController.Instance.currentQuestion.answers.Length; i++)
                {
                    if (i != correctIndex && String.IsNullOrEmpty(QuestionController.Instance.currentQuestion.answers[i]))
                        results[i] = 0;
                    else if (i != correctIndex && !String.IsNullOrEmpty(QuestionController.Instance.currentQuestion.answers[i]))
                        results[i] = 100 - correctPercent;
                }
            }
        }
        else
        {
            int correctPercent = UnityEngine.Random.Range(1, 101) > chanceOfCorrectAudience ? UnityEngine.Random.Range(1, 26) : UnityEngine.Random.Range(38, 63);     
            results[correctIndex] = correctPercent;

            int unusedPercent = 100 - correctPercent;
            int total = correctPercent;

            //Random wrong answers percent
            for (int i = 0; i < QuestionController.Instance.currentQuestion.answers.Length; i++)
            {
                if (i != correctIndex)
                {
                    results[i] = 100 - (UnityEngine.Random.Range(1, unusedPercent) + total);
                    unusedPercent -= results[i];
                    total += results[i];
                }  
            }

            if (unusedPercent > 0)
            {
                int leftover = 100 - results.Sum();
                int lowest = int.MaxValue;
                int lowestIndex = -1;
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i] < lowest)
                    {
                        lowest = results[i];
                        lowestIndex = i;
                    }
                }

                if (lowestIndex != -1) results[lowestIndex] += leftover;
            }
        }
        Quantity -= 1;
        return results;
    }

    public void PlayAudio()
    {
        Debug.Log("Play Audience audio");
    }
}