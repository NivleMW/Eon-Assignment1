using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizQuestionDetails
{
    private string questionTitle = "UNDEFINED";
    private List<string> optionsList = new List<string>();
    private int correctOption = -1;

    public QuizQuestionDetails() { }
    public QuizQuestionDetails(string questionTitle, List<string> optionsList, int correctOption)
    {
        this.questionTitle = questionTitle;
        this.optionsList = optionsList;
        if (correctOption < optionsList.Count)
            this.correctOption = correctOption;
        else
            Debug.LogWarning("Correct Answer not in options");
    }
    public void EditQuestionAll(string questionTitle, List<string> optionsList, int correctOption)
    {
        this.questionTitle = questionTitle;
        this.optionsList = optionsList;
        if (correctOption < optionsList.Count)
            this.correctOption = correctOption;
        else
            Debug.LogWarning("Correct Answer not in options");
    }
    public string GetQuestionTitle()
    {
        return questionTitle;
    }

    public List<string> GetOptions()
    {
        return optionsList;
    }

    // <returns>Returns -1 when error</returns>
    public int GetCorrectAnswer()
    {
        return correctOption;
    }
}
