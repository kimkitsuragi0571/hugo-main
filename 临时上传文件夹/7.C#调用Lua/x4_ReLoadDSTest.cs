using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

public class x4_ReLoadDSTest : MonoBehaviour
{
   private  StuClass stu;
   private ICallLua iCallLua;
    void Start()
    {
        LuaReloadMgr.Instance.Init("reqluads");
        LuaReloadMgr.Instance.DoString("require ('req3_luaDS')");
        //C#调用Lua中的类(StuClass是一个用于接收的自定义类)
        stu = LuaReloadMgr.Instance.Global.Get<StuClass>("testStuClass");
        //内部类table用C#接口接收
         iCallLua = LuaReloadMgr.Instance.Global.Get<ICallLua>("testStuClass");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("开始测试调用Lua类(对象在Start中赋值)");
            print(stu.stuName);
            print(stu.stuIsDead);
            //调用Lua中成员函数--LuaFunction的调用只是方便,还是建议用前面的委托
            //需要传入实例而非StuClass类型(Lua中函数定义传入self)
            stu.stuWork.Call(stu);
            //调用内部类(LuaTable装填)
            print( stu.testTopStu.Get<string>("stuName"));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("开始测试调用Lua中Dict和List");
            List<int> list =  LuaReloadMgr.Instance.Global.Get<List<int>>("testList");
            //多类型的List用Object装填
            //记得小写别识别为Unity中Object了
            List<object> list1 =  LuaReloadMgr.Instance.Global.Get<List<object>>("testListType");
            
            Dictionary<string, int> dict = LuaReloadMgr.Instance.Global.Get<Dictionary<string, int>>("testDict");
            //对于多类型Key的Dict(即使这里Value都是int类型),仍然建立kv均用Object
            //这里会自动识别为Unity的Objdect导致报错,使用object小写,即System.Object的别名
            Dictionary<object, object> dict1 = LuaReloadMgr.Instance.Global.Get<Dictionary<object, object>>("testDictType");
            print(list[0]);
            print(list1[0]);
            print(dict["1"]);
            print(dict1[true]);
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("开始测试调用Lua中普通变量");
            int testInt = LuaReloadMgr.Instance.Global.Get<int>("testInt");
            bool testBool = LuaReloadMgr.Instance.Global.Get<bool>("testBool");
            string testStr = LuaReloadMgr.Instance.Global.Get<string>("testStr");

            print(testInt);  
            print(testBool);  
            print(testStr);   
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("开始测试调用Lua中table做为C#接口");
            print(iCallLua.stuName);              // AKI
            print(iCallLua.stuIsDead);            // False
            iCallLua.stuWork();                   // 直接调用
            print(iCallLua.testTopStu.stuName);   // JP
        }
    }

}

public class StuClass
{
    //与Lua成员同名,必须是public
    //table可以用LuaTable类型
    public string stuName;
    public bool stuIsDead;
    public LuaFunction stuWork;
    public LuaTable testTopStu;
}

[CSharpCallLua]
public interface ICallLua
{
     string stuName {get;set; }
    bool stuIsDead {get;set;}
    UnityAction stuWork {get;set;}
    //接口类型嵌套接收内部table
    ITopStu testTopStu { get; set; }
}

//接口中不能直接嵌套内部类,需要单独定义接口
[CSharpCallLua]
public interface ITopStu
{
    string stuName { get; set; }
}