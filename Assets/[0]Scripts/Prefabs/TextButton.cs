using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textContainer;

    public void SetText(string text)
    {
        textContainer.text = text;
    }

    public void VisualizeSubtreeOnClick()
    {
        PamsManager.Instance
            .VisualizeSubtreeByName(textContainer.text);
    }
}
