using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootPoint : MonoBehaviour
{
    public RootPoint NextPoint;
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
