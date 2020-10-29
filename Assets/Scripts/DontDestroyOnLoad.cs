using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField]
    public string identifier;
    private static List<string> identifiers;

    void Awake()
    {
        if (identifiers == null) identifiers = new List<string>();
        if (identifier == null) return;

        if (!identifiers.Contains(identifier))
        {
            DontDestroyOnLoad(gameObject);
            identifiers.Add(identifier);
        }
    }
}
