using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGames;

public class PamsElementSearchManager : Singleton<PamsElementSearchManager>, ILoadable
{
    public SearchContainer searchContainer;

    public void Load()
    {
        searchContainer.CreateContentElements(PamsTree
            .GetAllNamesOfSubtree(PamsManager.Instance.tree.root));
    }
}
