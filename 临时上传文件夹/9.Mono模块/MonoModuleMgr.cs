using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoModuleMgr : MonoModuleBase<MonoModuleMgr>
{
   protected override void Awake()
   {
      base.Awake();
      Debug.Log("Mono模块启动");
      Debug.Log("将OnRun方法注册");
      AddUpdateEvent(OnRun);
   }

   private void OnRun()
   {
      Debug.Log("每帧调用Run,这里用OnRun封装一下");
      Run();
   }

   private void Run()
   {
      Debug.Log("每帧统一调用Run");
   }


}
