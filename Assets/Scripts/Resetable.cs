using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Resetable : MonoBehaviour
{
    [SerializeField] List<TMP_InputField> textMeshList = new List<TMP_InputField>();
    [SerializeField] List<GameObject> goToDestroyList = new List<GameObject>();
    private void OnDisable()
    {
        // Resets Text in TextMeshProUGUI every time this is disabled
        foreach (TMP_InputField i in textMeshList)
        {
            i.text = "";
        }
        foreach(GameObject i in goToDestroyList)
        {
            Destroy(i);
        }
        goToDestroyList.Clear();
    }

    public void AddGOToDestroy(GameObject newGO)
    {
        goToDestroyList.Add(newGO);
    }
}
