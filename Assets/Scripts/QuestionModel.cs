using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuestionModel
{
    public string question { get; set; }
    public string[] answers { get; set; }
    public bool asked { get; set; }
}
