using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private GameObject quizInstance;
    [SerializeField] private Transform quizContentArea;

    [Header("New Quiz UI")]
    [SerializeField] private GameObject newQuizScreen;
    [SerializeField] private TMP_InputField newQuizTitle;

    [Header("Quiz Details UI")]
    [SerializeField] private GameObject quizDetailsScreen;
    [SerializeField] private TextMeshProUGUI quizTitle;
    [SerializeField] private GameObject previewQuizBtn;

    [Header("Quiz Question Details UI")]
    [SerializeField] private Transform questionsContentArea;
    private List<GameObject> questionsTempList = new List<GameObject>();

    [Header("Question Details")]
    [SerializeField] private GameObject newQuestionScreen;
    [SerializeField] private GameObject optionInstance;
    [SerializeField] private Transform optionsContentArea;
    [SerializeField] private TMP_InputField questionTitleTxt;
    [SerializeField] private List<MCQOptionHandler> newQuestionOptions = new List<MCQOptionHandler>();

    [Header("Preview Quiz")]
    [SerializeField] private GameObject quizScreen;
    [SerializeField] private TextMeshProUGUI quizpreviewQuestion;
    [SerializeField] private GameObject quizpreviewOptionInstance;
    [SerializeField] private Transform quizpreviewOptionContentArea;
    private List<QuizOptionHandler> previewQuizOptions = new List<QuizOptionHandler>();
    [SerializeField] private GameObject quizResultScreen;
    [SerializeField] private TextMeshProUGUI finalScoreShow;

    private List<string> quizList = new List<string>();
    private QuizDetails currentQuiz = null;
    private int currentQuizNumber = -1;
    private int currentQuestionNoOpen = -1;
    public int currentScore = 0;
    private int currentQuizProgress = -1;
    private int currentQuizOptionSelection = -1;

    void Start()
    {
        string dataGotten;
        if (DataManager.LoadData(DataManager.quizNamesFilename, out dataGotten))
        {
            List<string> loadedQuizNames;
            if (DataManager.GetQuizes(dataGotten, out loadedQuizNames))
            {
                for (int i = 0; i < loadedQuizNames.Count; ++i)
                {
                    quizList.Add(loadedQuizNames[i]);
                    GameObject newGO = Instantiate(quizInstance, quizContentArea);
                    if (newGO.GetComponent<QuizTitleHandler>() != null)
                    {
                        int tempNumber = i; // Uses Pointer to i otherwise
                        newGO.GetComponent<QuizTitleHandler>().SetQuizDetails(i + 1, loadedQuizNames[i], () => { OpenQuiz(tempNumber); });
                    }
                }
            }
        }
        for(int i = 0; i < newQuestionOptions.Count; ++i)
        {
            int tempNumber = i; // Uses Pointer to i otherwise
            newQuestionOptions[tempNumber].Initilize(tempNumber + 1, () => {SelectThisAsCorrect(newQuestionOptions[tempNumber]); });
        }
        if(newQuestionOptions.Count > 0)
        {
            newQuestionOptions[0].Reset();
        }
    }

    #region Buttons
    public void ToggleCreateQuiz(bool activate)
    {
        newQuizScreen.SetActive(activate);
    }

    public void ExitQuizDetails()
    {
        quizDetailsScreen.SetActive(false);
        currentQuiz = null;
        currentQuizNumber = -1;
        foreach (GameObject i in questionsTempList)
        {
            Destroy(i);
        }
        questionsTempList.Clear();
    }

    public void SaveNewQuizTitle()
    {
        quizList.Add(newQuizTitle.text);
        GameObject newGO = Instantiate(quizInstance, quizContentArea);
        if(newGO)
        {
            int tempNumber = quizList.Count - 1;
            newGO.GetComponent<QuizTitleHandler>().SetQuizDetails(quizList.Count, newQuizTitle.text, () => { OpenQuiz(tempNumber); });
        }
        ToggleCreateQuiz(false);
        OpenQuiz(quizList.Count - 1);
        DataManager.SaveData(DataManager.quizNamesFilename, DataManager.EditQuizes(quizList));
    }

    public void ToggleQuestionScreen(bool active)
    {
        if(!active)
        {
            questionTitleTxt.text = "";

            // MCQ Creation part
            for (int i = 1; i < newQuestionOptions.Count; ++i)
            {
                Destroy(newQuestionOptions[i].gameObject);
            }
            MCQOptionHandler keepGO = newQuestionOptions[0];
            newQuestionOptions.Clear();
            newQuestionOptions.Add(keepGO);
            keepGO.Reset();

            // No Save, just exit, so no add question
            if (currentQuiz.GetQuizQuestion(currentQuestionNoOpen).GetCorrectAnswer() == -1)
            {
                currentQuiz.RemoveQuizQuestion(currentQuestionNoOpen);
                currentQuestionNoOpen = -1;
            }
        }
        else
        {
            currentQuiz.AddQuizQuestion();
            currentQuestionNoOpen = currentQuiz.GetQuizQuestions().Count - 1;
        }
        newQuestionScreen.SetActive(active);
    }

    public void AddNewMCQOption()
    {
        GameObject newGO = Instantiate(optionInstance, optionsContentArea);
        if (newGO.GetComponent<MCQOptionHandler>() != null)
        {
            newGO.transform.SetSiblingIndex(newQuestionOptions.Count);
            newQuestionOptions.Add(newGO.GetComponent<MCQOptionHandler>());
            int tempNumber = newQuestionOptions.Count - 1; // Uses Pointer to i otherwise
            newQuestionOptions[tempNumber].Initilize(tempNumber + 1, () => { SelectThisAsCorrect(newQuestionOptions[tempNumber]); });
        }
    }
    public void SaveQuestion()
    {
        List<string> questionOptionsListTxt = new List<string>();
        int correctOptionInt = -1;
        for(int i = 0; i < newQuestionOptions.Count; ++i)
        {
            questionOptionsListTxt.Add(newQuestionOptions[i].GetOptionText());
            if (newQuestionOptions[i].GetToggleIsOn())
                correctOptionInt = i;
        }

        currentQuiz.EditQuizQuestion(questionTitleTxt.text, questionOptionsListTxt, correctOptionInt, currentQuestionNoOpen);

        LoadQuizDetails();
        previewQuizBtn.SetActive(true);

        DataManager.SaveData(DataManager.quizNumberBaseFilename + currentQuizNumber, DataManager.EditQuizQuestions(currentQuiz.GetQuizQuestions()));

        ToggleQuestionScreen(false);// Part of Requirement Specific
    }

    public void TogglePreviewQuiz(bool active)
    {
        quizScreen.SetActive(active);
        if(active)
        {
            currentScore = 0;
            currentQuizProgress = -1;
            quizResultScreen.SetActive(false);
            LoadNextQuestionQuiz();
        }
    }

    public void QuizNextBtn()
    {
        if (currentQuizOptionSelection == currentQuiz.GetQuizQuestion(currentQuizProgress).GetCorrectAnswer())
            ++currentScore;
        LoadNextQuestionQuiz();
    }
    #endregion

    private void LoadNextQuestionQuiz()
    {
        if (currentQuizOptionSelection == -1 && currentQuizProgress != -1)
        {
            Debug.LogWarning("Did not choose an option");
            return;
        }

        ++currentQuizProgress;
        if(currentQuizProgress == currentQuiz.GetQuizQuestions().Count)
        {
            quizResultScreen.SetActive(true);
            finalScoreShow.text = "Your Score: " + currentScore + '/' + currentQuiz.GetQuizQuestions().Count;
            return;
        }
        quizpreviewQuestion.text = currentQuiz.GetQuizQuestion(currentQuizProgress).GetQuestionTitle();
        foreach(QuizOptionHandler i in previewQuizOptions)
        {
            Destroy(i.gameObject);
        }
        previewQuizOptions.Clear();
       
        List<string> tempPreviewOptionList = currentQuiz.GetQuizQuestion(currentQuizProgress).GetOptions();
        for(int i = 0; i< tempPreviewOptionList.Count; ++i)
        {
            GameObject newGO = Instantiate(quizpreviewOptionInstance, quizpreviewOptionContentArea);
            if(newGO.GetComponent<QuizOptionHandler>() != null)
            {
                QuizOptionHandler newHandler = newGO.GetComponent<QuizOptionHandler>();
                previewQuizOptions.Add(newHandler);
                newHandler.Initilize(tempPreviewOptionList[i], () => {SelectedPreviewQuizOption(newHandler); });
            }
        }
        currentQuizOptionSelection = -1;
    }

    private void LoadQuizDetails()
    {
        foreach (GameObject i in questionsTempList)
        {
            Destroy(i);
        }
        questionsTempList.Clear();

        List<QuizQuestionDetails> quizQDetailsList = currentQuiz.GetQuizQuestions();
        for(int i =0; i < quizQDetailsList.Count; ++i)
        {
            GameObject newGO = Instantiate(quizInstance, questionsContentArea);
            if (newGO.GetComponent<QuizTitleHandler>() != null)
            {
                questionsTempList.Add(newGO);
                int tempNumber = i; // Uses Pointer to i otherwise
                newGO.GetComponent<QuizTitleHandler>().SetQuizDetails(i + 1, quizQDetailsList[i].GetQuestionTitle(), () => { OpenEditQuestion(tempNumber); });
            }
        }
    }

    #region UnityActions
    private void OpenQuiz(int quizNo)
    {
        quizDetailsScreen.SetActive(true);
        quizTitle.text = "Title: " + quizList[quizNo];
        string receivedData;
        if(DataManager.LoadData(DataManager.quizNumberBaseFilename + quizNo, out receivedData))
        {
            List<QuizQuestionDetails> loadedQuizQuestions;
            if(DataManager.GetQuizQuestions(receivedData, out loadedQuizQuestions))
            {
                currentQuiz = new QuizDetails(quizList[quizNo], loadedQuizQuestions);
                currentQuizNumber = quizNo;

                LoadQuizDetails();
                previewQuizBtn.SetActive(true);
            }
        }
        else
        {
            currentQuiz = new QuizDetails(quizList[quizNo]);
            currentQuizNumber = quizNo;
            previewQuizBtn.SetActive(false);
        }
    }
    private void OpenEditQuestion(int questionNo)
    {
        currentQuestionNoOpen = questionNo;
        newQuestionScreen.SetActive(true);
        questionTitleTxt.text = currentQuiz.GetQuizQuestion(questionNo).GetQuestionTitle();
        List<string> loadedOptionsList = currentQuiz.GetQuizQuestion(questionNo).GetOptions();
        for(int i = 1; i < loadedOptionsList.Count; ++i)
        {
            AddNewMCQOption();
        }
        for(int i = 0; i< newQuestionOptions.Count; ++i)
        {
            newQuestionOptions[i].LoadData(loadedOptionsList[i], currentQuiz.GetQuizQuestion(questionNo).GetCorrectAnswer() == i);
        }
    }
    private void SelectThisAsCorrect(MCQOptionHandler theHandler)
    {
        foreach(MCQOptionHandler i in newQuestionOptions)
        {
            i.ToggleToggle(i == theHandler);
        }
    }
    private void SelectedPreviewQuizOption(QuizOptionHandler theHandler)
    {
        for(int i = 0;  i < previewQuizOptions.Count; ++i)
        {
            if (previewQuizOptions[i] == theHandler)
            {
                currentQuizOptionSelection = i;
                previewQuizOptions[i].ToggleActive(true);
            }
            else
                previewQuizOptions[i].ToggleActive(false);
        }
    }
    #endregion
}
