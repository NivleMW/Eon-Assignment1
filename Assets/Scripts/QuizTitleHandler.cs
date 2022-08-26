using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuizTitleHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI quizNo;
    [SerializeField] TextMeshProUGUI quizTitle;
    [SerializeField] Button quizButton;

    public void SetQuizDetails(int no, string title, UnityAction action)
    {
        quizNo.text = "" + no;
        quizTitle.text = title;
        quizButton.onClick.AddListener(action);
    }
}
