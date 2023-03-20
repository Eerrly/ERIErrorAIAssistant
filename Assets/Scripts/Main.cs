using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        Debug.LogWarning("This is Debug.LogWarning!");
        Debug.LogError("This is Debu.LogError!");
        throw new System.Exception("This is System.Exception!");
    }

}
