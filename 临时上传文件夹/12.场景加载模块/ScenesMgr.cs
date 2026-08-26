using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesMgr : MonoSingletonBase<ScenesMgr>
{
    //1.普通的同步加载方法
    //传入加载的场景名,场景加载完毕时执行的函数
    public void LoadScene(string sceneName, UnityAction OnLoadComplete)
    {
        //场景同步加载
        SceneManager.LoadScene(sceneName);
        //加载完毕执行
        OnLoadComplete();
    }
    
    
    //2.异步加载方法
    public void LoadSceneAsync(string sceneName, UnityAction OnLoadComplete)
    {
        //如果这个ScenesMgr继承纯C#单例,也是可以用之前讲的纯C#实现协程(在Mono模块里面)
        //MonoModuleMgr.Instance.StartCor(IELoadSceneAsync(sceneName, OnLoadComplete));
        IEnumerator ie = IELoadSceneAsync(sceneName, OnLoadComplete);
        StartCoroutine(ie);
        
    }
    //定义异步加载所用的协程
    private IEnumerator IELoadSceneAsync(string sceneName, UnityAction OnLoadComplete)
    {
        //场景异步加载的句柄AsyncOperation
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        //还可以通过.progress属性获取场景加载进度
        while (!ao.isDone)
        {
            //事件中心模块(支持泛型的升级版)
            //这里就是触犯了Key = "场景加载"下的所有函数并且传入参数为ao.progress
            //不用在意传入的函数名或者说事件容器,那个是Add里面才写的
            GenericEventCenterMgr.Instance.EventTrigger("场景加载", ao.progress);
           yield return ao.progress;
           //yield return null;
        }

        yield return ao;
        OnLoadComplete();
    }

}
