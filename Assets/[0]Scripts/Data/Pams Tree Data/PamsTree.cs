using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class PamsTree : MonoBehaviour
{
    public PamsElement root;

    public static PamsElement FindSubtreeByGO(PamsElement tree, GameObject objectToFind)
    {
        PamsElement toReturn;

        if (tree.data.GO == objectToFind)
        {
            return tree;
        }
        else if (tree.children.Count != 0)
        {
            for (int i = 0; i < tree.children.Count; i++)
            {
                toReturn = FindSubtreeByGO(tree.children[i], objectToFind);
                if (toReturn != null)
                {
                    return toReturn;
                }
            }
        }
        return null;
    }
    public static PamsElement FindSubtreeByName(PamsElement tree, string nameToFind)
    {
        PamsElement toReturn;

        if (tree.data.name == nameToFind)
        {
            return tree;
        }
        else if (tree.children.Count != 0)
        {
            for (int i = 0; i < tree.children.Count; i++)
            {
                toReturn = FindSubtreeByName(tree.children[i], nameToFind);
                if (toReturn != null)
                {
                    return toReturn;
                }
            }
        }
        return null;
    }
    public static List<string> GetAllNamesOfSubtree(PamsElement tree)
    {
        List<string> names = new List<string>();
        names.Add(tree.data.name);
        if (tree.children.Count > 0)
        {
            for (int i = 0; i < tree.children.Count; i++)
            {
                List<string> namesOfSubtree = GetAllNamesOfSubtree(tree.children[i]);
                for (int j = 0; j < namesOfSubtree.Count; j++)
                {
                    names.Add(namesOfSubtree[j]);
                }
            }
        }
        return names;
    }
    public static List<string> GetChildNamesOfSubtree(PamsElement tree)
    {
        List<string> names = new List<string>();

        if (tree.children.Count > 0)
        {
            for (int i = 0; i < tree.children.Count; i++)
            {
                names.Add(tree.children[i].data.name);
            }
        }

        return names;
    }

    public static float3 GetDeviationOfSubtreeFromTreeByName(PamsElement tree, string subtreeName)
    {
        float3 toReturn;

        if (FindSubtreeByName(tree, subtreeName) == null)
        {
            return float3.zero;
        }
        else
        {
            toReturn = tree.data.position;
            for (int i = 0; i < tree.children.Count; i++)
            {
                toReturn += GetDeviationOfSubtreeFromTreeByName(tree.children[i], subtreeName);
            }
        }

        return toReturn;
    }
}
