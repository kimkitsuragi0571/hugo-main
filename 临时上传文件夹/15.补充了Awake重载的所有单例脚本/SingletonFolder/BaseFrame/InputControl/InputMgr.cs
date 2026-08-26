using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//如果使用纯C#单例,还要用前面的Mono模块实现生命周期
public class InputMgr : MonoSingletonBase<InputMgr>
{
    //纯C#单例这里就是构造函数里面调用Mono模块
    // 如public InputMgr()里面 MonoModuleMgr.Instance.AddUpdateEvent(Update中统一调用函数);
    protected override void Awake()
    {
        //记住得重写初始化
        base.Awake();
        print("哔哔哔...输入管理模块启动");
        
        //额,这里也统一托管到Mono模块执行吧
        //传入的就是输入检测
        MonoModuleMgr.Instance.AddUpdateEvent(InputCheck);
    }
    
    //搞个输入检测的开关,进游戏的时候默认关闭
    private bool isCheck = false;
    public void SwitchCheck(bool Switch)
    {
        isCheck = Switch;
    }
    
    private void InputCheck()
    {
        if (!isCheck)
        {
            return;
        }
        //EventTrigger的意义:
        //就是去事件中心里面找所有注册了"按下Q键"这个Key的函数并且执行(当然还会传入KeyCode.Q参数)
        //注意这里用改良版后支持参数版的事件中心
        
        //勾巴乱讲课,这里封装方法,结果全都注册到同一个"按下特定键"的Key,Key的参数自己修改后传入才对
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GenericEventCenterMgr.Instance.EventTrigger<KeyCode>("按下Q键", KeyCode.Q);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GenericEventCenterMgr.Instance.EventTrigger<KeyCode>("按下W键", KeyCode.W);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GenericEventCenterMgr.Instance.EventTrigger<KeyCode>("按下E键", KeyCode.E);
        }
    }
    
    
}
