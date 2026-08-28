using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//UIPanel面板基类
public class UIPanelBase : MonoBehaviour
{
    //要存储多种UI组件,所有组件父类容器都是UIBehaviour
    //一个UI可能有多个组件比如Image和Button,添加了Button后,同一个UI的Image就没法添加了(因为Key无法重复嘛)
    //所以这里改为使用List
    private Dictionary<string, List<UIBehaviour>> conDict = new Dictionary<string, List<UIBehaviour>>();

    //事件监听中用于重写的虚函数
    protected virtual void OnClick(string btnName)
    {
        
    }

    protected virtual void OnValueChanged(string togName, bool isOn)
    {
        
    }
    //虚方法方便子类在Awake中处理额外逻辑
    protected virtual void Awake()
    {
        AddConToDict<Button>();
        AddConToDict<Image>();
        AddConToDict<Text>();
        AddConToDict<Slider>();
        AddConToDict<Toggle>();
    }

    
    //基类首先要提供获取所有组件然后显隐的方法
    //找到对应类型控件,然后添加到Dict里面,不需要以前一样依次声明变量然后拖拽
    private void AddConToDict<T>()where T : UIBehaviour
    {
        //未激活对象仍然获取
        T[] cons =  this.GetComponentsInChildren<T>(true);
       
      
        foreach (T con in cons)
        {
            if (con == null)
            {
                continue;
            }
            //检测是否已经有了对应的Key,没有就创建有了就添加
            //之前uiName变量是写在Foreach外部的,现在必须改到内部
            //不然uiName始终是被更新为最后一个foreach的con的名称,后面的闭包捕获就会出错
            string uiName = con.gameObject.name;
            if (conDict.ContainsKey(uiName))
            {
              
                //如果一个UI上有两个Button(UGUI不允许重复添加同类型组件,一般来说不会有这种情况)
                //但比如你不小心调用了两次Add方法或者脚本重复添加,就会导致List有重复项
                //为了避免上面的重复项,还是得判断该物体下是否已经有同类型组件
                if (!conDict[uiName].Contains(con))
                {
                    //不添加上面的约束这里就会报错
                    //有就直接指定Key(也就是指定了List.Add)添加Value
                    conDict[uiName].Add(con);
                }
            }
            else
            {
                //没有就创建新的键值对(也是要new List并添加con)
                conDict.Add(uiName, new List<UIBehaviour>() { con });
            }
            
            //新增:根据类型直接添加监听虚函数,后面重写即可
            if (con is Button)
            {
                //这里onClick限定添加无参函数不能直接传入OnClick,所以用闭包
               
                //每次循环,btn1和btn2两个不同的按钮,各自不同的onClick事件但是都注册了同一个onClick方法,只是传入的不同参数.
                //当我们点击其中任意一个按钮都会触发onClick函数,此时就需要通过传入uiName参数区分(用switch语句之类的)
                (con as Button).onClick.AddListener(() =>
                {
                    OnClick(uiName);
                });
            }
            else if (con is Toggle)
            {
                //这里订阅函数需要有个bool参数
                (con as Toggle).onValueChanged.AddListener((bool isOn) =>
                {
                    OnValueChanged(uiName, isOn);
                });
            }
        }
    }

    protected T GetConFromDict<T>(string uiName) where T : UIBehaviour
    {
        if (conDict.ContainsKey(uiName))
        {
            //如果包含某UI,则遍历其包含的List
            foreach (UIBehaviour con in conDict[uiName])
            {
                //List中某个组件如果是指定类型(UGUI不允许重复添加同类型组件,所以List中没有重复类型组件)
                //也和向下转型一样有简易写法 if (con is T target)return target;
                if (con is T)
                {
                    return con as T;
                }
            }
        }
        return null;
    }

    //显隐方法:这个我们在UIPanelMgr中调用(比如显示面板的时候调用对应面板ShowMe)
    public virtual void ShowMe(string panelName)
    {
        print("已经显示" + panelName);
    }

    public virtual void HideMe(string panelName)
    {
        print("已经隐藏" + panelName);
    }

}
