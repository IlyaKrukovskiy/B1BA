using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public List<GameObject> managers;

    public void Start()
    {
        for(int i = 0; i < managers.Count; i++)
        {
            var loadComponent = managers[i].GetComponent<ILoadable>();
            if (loadComponent != null)
            {
                loadComponent.Load();
            }
        }
    }
}
