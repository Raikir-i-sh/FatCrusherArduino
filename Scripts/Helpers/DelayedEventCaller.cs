using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DelayedEventCaller : MonoBehaviour
{
    public static event Func<string> myMethod;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("MyMethod" , 1, 3); 
    }

    private void MyMethod()
    {
        Debug.Log( " workiing " +myMethod?.Invoke() );
    }

   
}
