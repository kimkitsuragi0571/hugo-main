using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenesMgrTest : MonoBehaviour
{
    public Slider progressBar; 
    public TMP_Text progressText;  
    void Start()
    {
        //订阅事件, 当ScenesMgr触发场景加载(也就是我们使用EventTrigger的时候),触发UpdateProgress
        // GenericEventCenterMgr.Instance.EventTrigger("场景加载", ao.progress);
        GenericEventCenterMgr.Instance.AddEvent<float>("场景加载", UpdateProgress);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScenesMgr.Instance.LoadSceneAsync("GameStore", () =>
            {
                print("场景加载完成");
            });
        }
    }

    void OnDestroy()
    {
        //销毁物体实例的时候,必须取消订阅
        GenericEventCenterMgr.Instance.RemoveEvent<float>("场景加载", UpdateProgress);
    }
    
    private void UpdateProgress(float progress)
    {
        // SceneManager.LoadSceneAsync 的进度范围是 0~0.9，这里映射到 0~1
        float displayProgress = Mathf.Clamp01(progress / 0.9f);
        
        progressBar.value = displayProgress;
        progressText.text = $"加载中... {(int)(displayProgress * 100)}%";
    }
}
