using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_1 
{
  private static AudioManager_1 _instance;

  public static AudioManager_1 Instance
  {
      get
      {
          if (_instance == null)
          {
              _instance = new AudioManager_1();
          }
          return _instance;
      }
  }
  
  private AudioManager_1()
  {
      
  }

  public void DataInit()
  {
      Debug.Log("AudioManager-懒汉单例");
  }
}
