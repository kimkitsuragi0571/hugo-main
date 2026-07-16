using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager_2 
{
    private static readonly  DataManager_2 _instance = new DataManager_2();

    public static DataManager_2 Instance
    {
        get
        {
            return _instance;
        }
    }

    private DataManager_2()
    {
        
    }

    public void DataInit()
    {
        Debug.Log("DataManager-饿汉单例");
    }
}
