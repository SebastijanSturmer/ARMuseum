using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class QuizQuestion
{
    public string QuestionEN;
    public string QuestionHR;

    public string[] AnswersEN;
    public string[] AnswersHR;

    public string CorrectAnswerEN;
    public string CorrectAnswerHR;
}
