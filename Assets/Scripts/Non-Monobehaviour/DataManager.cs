using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

static public class DataManager
{
    static public string quizNamesFilename = "QuizNames";
    static public string quizNumberBaseFilename = "Quiz";
    static private string saveDataType = ".dat";

    #region ByteReading
    static public bool SaveData(string _fileName, byte[] _data)
    {
        _fileName += saveDataType;
        FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Create);
        stream.Write(_data, 0, _data.Length);
        stream.Close();
        Debug.Log("Data Saved: " + _fileName);
        return true;
    }
    static public bool SaveData(string _fileName, string _data)
    {
        _fileName += saveDataType;
        byte[] byteData = Encoding.UTF8.GetBytes(_data);
        FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Create);
        stream.Write(byteData, 0, byteData.Length);
        stream.Close();
        Debug.Log("Data Saved: " + _fileName);
        return true;
    }

    static public bool LoadData(string _fileName, out byte[] _data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, _fileName + saveDataType);
        if(File.Exists(filePath))
        {
            _data = File.ReadAllBytes(filePath);
            return true;
        }
        Debug.LogWarning("Unable to load file: " + _fileName);
        _data = new byte[] { 0x0 };
        return false;
    }
    static public bool LoadData(string _fileName, out string _data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, _fileName + saveDataType);
        if(File.Exists(filePath))
        {
            _data = Encoding.UTF8.GetString(File.ReadAllBytes(filePath));
            return true;
        }
        Debug.LogWarning("Unable to load file: " + _fileName);
        _data = "";
        return false;
    }
    #endregion

    #region QuizDataReading
    //QuizNames.dat
    //  Quiz1Name
    //  Quiz2Name
    //  ...

    //Quiz1.dat // Fixed Name here (in seperate file to minimize file reading time)
    //  Q1
    //  No of choices:CorrectAnswer
    //  :Q1O1
    //  :Q1O2
    //  Q2
    //  No of choices:CorrectAnswer
    //  :Q2O1
    //  :Q2O2
    //  :Q2O3
    //  ...

    // I use '\n' as seperator since it cannot be typed by the user
    static public bool GetQuizes(string _data, out List<string> _outQuizNames)
    {
        _outQuizNames = new List<string>();
        if (_data.IndexOf('\n') == -1)
        {
            return false;
        }
        while(_data.IndexOf('\n') != -1)
        {
            _outQuizNames.Add(_data.Substring(0, _data.IndexOf('\n')));
            _data = _data.Substring(_data.IndexOf('\n') + 1);
        }
        _outQuizNames.Add(_data);
        return true;
    }
    static public string EditQuizes(List<string> _quizesNames)
    {
        string outData = "";
        if (_quizesNames.Count > 0)
            outData += _quizesNames[0];
        for(int i = 1; i < _quizesNames.Count; ++i)
        {
            outData += '\n' + _quizesNames[i];
        }
        return outData;
    }

    static public bool GetQuizQuestions(string _data, out List<QuizQuestionDetails> _outQuizDet)
    {
        _outQuizDet = new List<QuizQuestionDetails>();
        if(_data.IndexOf('\n') == -1)
        {
            return false;
        }
        while(_data.IndexOf('\n') != -1)
        {
            string question;
            int choicesNo;
            int correctChoice;
            List<string> options = new List<string>();

            string testForNewLine;
            // Question Title
            question = _data.Substring(0, _data.IndexOf('\n'));
            _data = _data.Substring(_data.IndexOf('\n') + 1);

            // Number of choices
            if(_data.IndexOf(':') == -1)
            {
                Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                return false;
            }
            testForNewLine = _data.Substring(0, _data.IndexOf(':'));
            if(testForNewLine.IndexOf('\n') != -1)
            {
                Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                return false;
            }
            if(!int.TryParse(testForNewLine, out choicesNo))
            {
                Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                return false;
            }
            _data = _data.Substring(_data.IndexOf(':') + 1);

            // Correct Choice
            if (_data.IndexOf("\n:") == -1)
            {
                Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                break;
            }
            testForNewLine = _data.Substring(0, _data.IndexOf("\n:"));
            if (testForNewLine.IndexOf('\n') != -1)
            {
                Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                return false;
            }
            if(!int.TryParse(testForNewLine, out correctChoice))
            {
                Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                return false;
            }
            _data = _data.Substring(_data.IndexOf("\n:") + 2);

            // Options
            for(int i = 1; i < choicesNo; ++i)
            {
                if(_data.IndexOf("\n:") == -1)
                {
                    Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                    return false;
                }
                testForNewLine = _data.Substring(0, _data.IndexOf("\n:"));
                if(testForNewLine.IndexOf('\n') != -1)
                {
                    Debug.LogError("Error loading Quiz Question, last question loaded: " + _outQuizDet.Count);
                    return false;
                }
                options.Add(testForNewLine);
                _data = _data.Substring(_data.IndexOf("\n:") + 2);
            }
            // Last Option before next Question Or just End
            if (_data.IndexOf("\n") == -1)
            {
                options.Add(_data);
            }
            else
            {
                options.Add(_data.Substring(0, _data.IndexOf("\n")));
                _data = _data.Substring(_data.IndexOf('\n') + 1);
            }

            _outQuizDet.Add(new QuizQuestionDetails(question, options, correctChoice));
        }
        return true;
    }
    static public string EditQuizQuestions(List<QuizQuestionDetails> _quizDetails)
    {
        string outData = "";
        if (_quizDetails.Count > 0)
        {
            List<string> optionsDetList = _quizDetails[0].GetOptions();
            outData += _quizDetails[0].GetQuestionTitle() + '\n' + optionsDetList.Count + ':' + _quizDetails[0].GetCorrectAnswer();
            for(int optionsNo = 0; optionsNo < optionsDetList.Count; ++optionsNo)
            {
                outData += "\n:" + optionsDetList[optionsNo];
            }
        }
        for(int i = 1; i < _quizDetails.Count; ++i)
        {
            List<string> optionsDetList = _quizDetails[i].GetOptions();
            outData += '\n' + _quizDetails[i].GetQuestionTitle() + '\n' + optionsDetList.Count + ':' + _quizDetails[i].GetCorrectAnswer();
            for (int optionsNo = 0; optionsNo < optionsDetList.Count; ++optionsNo)
            {
                outData += "\n:" + optionsDetList[optionsNo];
            }
        }
        return outData;
    }
    #endregion
}
