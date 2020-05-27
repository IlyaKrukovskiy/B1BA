using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchContainer : MonoBehaviour
{
    public Transform contentElementsContainer;

    [SerializeField] private GameObject contentElementPrefab;

    private List<string> elementTexts = new List<string>();

    private Dictionary<string, GameObject> contentElementsByText = new Dictionary<string, GameObject>();

    private GameObject currentElement;

    public void CreateContentElements(List<string> texts)
    {
        elementTexts = new List<string>(texts);

        DisposeCurrentContent();

        for (int i = 0; i < texts.Count; i++)
        {
            GameObject newElement = Instantiate(contentElementPrefab);          
            newElement.GetComponent<TextButton>().SetText(texts[i]);
            newElement.SetActive(false);
            newElement.transform.SetParent(contentElementsContainer, false);
            contentElementsByText[elementTexts[i]] = newElement;
        }

       
    }

    public void DisposeCurrentContent()
    {
        if (contentElementsByText.Count > 0)
        {
            foreach (var key in contentElementsByText.Keys)
            {
                PoolManager.Instance.Despawn(PoolType.UI, contentElementsByText[key]);
            }

            contentElementsByText.Clear();
        }
    }

    public void OnValueChangedOnSearchInput(TMP_InputField inputField)
    {
        string inputFieldText = inputField.text.ToLowerInvariant();

        if (currentElement != null)
        {
            currentElement.SetActive(false);
        }
        int j = 0;
        for (int i = 0; i < elementTexts.Count; i++)
        {
            GameObject element = contentElementsByText[elementTexts[i]];

            if (elementTexts[i].ToLowerInvariant().Contains(inputFieldText))
            {
                element.SetActive(true);
                currentElement = element;

                break;
            }

        }

    }

}
