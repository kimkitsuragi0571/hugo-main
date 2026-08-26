using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//调用者也需要配合await,一般在Start 或按钮点击事件里调用
public class AwaitLoadMgrTest : MonoBehaviour
{
   //声明await加载器对象
   public AwaitLoadMgr loader;
   //Start 方法也可以加 async，但返回值必须是 void 或 Task
   private async void Start()
   {
      //调用异步方法，并用 await 接收结果
      //代码会停在这里直到加载完毕
      GameObject enemy = await loader.LoadResAsync<GameObject>("Prefabs/Enemy");
      Debug.Log("加载完成！名字是：" + enemy.name);
   }
}
