using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuestionModel
{
    public string question { get; set; }
    public string[] answers { get; set; }
    public string correctAns { get; set; }
    public bool asked { get; set; }

    public enum QuestionType
    {
        Unlock,
        Easy,
        Medium,
        Hard
    }

    public int questionType;

    public QuestionType Type { get; set; }
}