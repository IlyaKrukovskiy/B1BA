using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PamsElementPrefab : MonoBehaviour, IPoolable
{
    [SerializeField] private GameObject elementWithSection;
    [SerializeField] private GameObject elementWithoutSection;

    private void ShowSection(bool flag)
    {
        elementWithSection.SetActive(flag);
        elementWithoutSection.SetActive(!flag);
    }

    public void OnDespawn()
    {
        if (elementWithSection != null)
        {
            PamsManager.Instance.showSections -= ShowSection;
        }
    }

    public void OnSpawn()
    {
        if (elementWithSection != null)
        {
            PamsManager.Instance.showSections += ShowSection;
        }
    }
    
}
