using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuizOptionHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI optionName;
    [SerializeField] Button btn;
    [SerializeField] Image img;
    [SerializeField] Color selectedColor;
    private Color originalColor;
    public void Initilize(string optionName, UnityAction action)
    {
        this.optionName.text = optionName;
        btn.onClick.AddListener(action);
        originalColor = img.color;
    }

    public void ToggleActive(bool active)
    {
        if (active)
            img.color = selectedColor;
        else
            img.color = originalColor;
    }
}
