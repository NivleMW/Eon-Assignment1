using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizDetails
{
    private string quizTitle = "UNDEFINED";
    private List<QuizQuestionDetails> quizQuestionList = new List<QuizQuestionDetails>();

    public QuizDetails(string quizTitle)
    {
        this.quizTitle = quizTitle;
    }

    public QuizDetails(string quizTitle, List<QuizQuestionDetails> quizQuestions)
    {
        this.quizTitle = quizTitle;
        quizQuestionList = quizQuestions;
    }

    public string GetQuizTitle()
    {
        return quizTitle;
    }

    public void AddQuizQuestion()
    {
        quizQuestionList.Add(new QuizQuestionDetails());
    }

    public void RemoveQuizQuestion(int removeAt)
    {
        quizQuestionList.RemoveAt(removeAt);
    }

    public void EditQuizQuestion(string questionTitle, List<string> options, int correctOption, int questionNo)
    {
        if(questionNo < quizQuestionList.Count)
            quizQuestionList[questionNo].EditQuestionAll(questionTitle, options, correctOption);
    }

    public List<QuizQuestionDetails> GetQuizQuestions()
    {
        return quizQuestionList;
    }
    public QuizQuestionDetails GetQuizQuestion(int index)
    {
        if (index < quizQuestionList.Count)
            return quizQuestionList[index];
        else
            return null;
    }
}