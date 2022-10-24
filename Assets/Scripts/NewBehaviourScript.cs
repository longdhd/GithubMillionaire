using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

public class NewBehaviourScript : MonoBehaviour
{

    [Serializable]
    public class Question
    {
        public string question { get; set; }
        public List<string> answers { get; set; }

        public bool asked { get; set; }
    }
    public Question[] questions;
    void Start()
    {
        var file = Application.dataPath + "/JSON/questions.json";
        string json = File.ReadAllText(file);
        List<Question> questionList = JsonConvert.DeserializeObject<List<Question>>(json);
    }
}