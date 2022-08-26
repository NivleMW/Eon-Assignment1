using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MCQOptionHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI optionNoTxt;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button toggleBtn;
    [SerializeField] private GameObject toggleImg;
    private bool isOn = false;
    
    public void Initilize(int optionNo, UnityAction action)
    {
        optionNoTxt.text = "Option " + optionNo;
        toggleBtn.onClick.AddListener(action);
        ToggleToggle(false);
    }
    public void Reset()
    {
        inputField.text = "";
        isOn = true;
        toggleImg.SetActive(true);
    }

    public void LoadData(string optionsTxt, bool active)
    {
        inputField.text = optionsTxt;
        ToggleToggle(active);
    }

    public void ToggleToggle(bool active)
    {
        isOn = active;
        toggleImg.SetActive(active);
    }
    public bool GetToggleIsOn()
    {
        return isOn;
    }
    public string GetOptionText()
    {
        return inputField.text;
    }
}
