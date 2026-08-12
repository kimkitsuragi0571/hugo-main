using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


//委托类型声明单独放一个脚本
//直接不需要写在类内部了,裸区域声明(因为本来就是类型声明嘛)
    
//只有存储无参无返回值的委托不需要使用特性(Unity自动识别)
public delegate void CustomCall();
    
//有参有返回值  使用特性就需要点击生成代码
[CSharpCallLua]
public delegate string CustomCall2(int val);
//多返回值   out参数对应Lua多个返回值   
[CSharpCallLua]
public delegate int CustomCall3(int val, out int ret0, out string ret1);
//变长参数  params object[]对应变长参数   本身也可以有固定参数
[CSharpCallLua]
public delegate void CustomCall4(string msg, params object[] args);


public class xDel_ReLoadDel: MonoBehaviour
{
    void Awake()
    {
        print("----------------启动委托类型声明的脚本(类型声明都在裸区域而非类内)--------------");
    }

}
