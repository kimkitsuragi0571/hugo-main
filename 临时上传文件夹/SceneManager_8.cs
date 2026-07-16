using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneManager_8 : Singleton_2<SceneManager_8>
{
    public int lev;

    protected override void Awake()
    {
      base.Awake();
      Debug.Log("继承单例基类拓展Awake逻辑");
    }

    public void DataInit()
    {
        Debug.Log("SceneManager-继承了Mono单例基类");
    }
}
