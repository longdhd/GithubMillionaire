using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestionType
{
    Unlock,
    Easy,
    Medium,
    Hard
}
[Serializable]
public class QuestionModel
{
    public string question { get; set; }
    public string[] answers { get; set; }
    public string correctAnswer { get; set; }
    public bool asked { get; set; } = false;

    public int difficulty;
    public QuestionType Type { get; set; }
}