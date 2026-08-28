using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UIPanelMgrTest不是面板本体,只是个调用脚本
public class UIPanelMgrTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //这里只需要传入Panel名称即可,方法内部自动拼接Res文件夹下路径
            //callback传参以及泛型填面板预制体上的脚本
            //UnityAction要求的传入方法需要有一个T同款类型参数
            UIPanelMgr.Instance.ShowPanel<PlayPanelTest>("PlayPanel", E_UILayer.Top, (PlayPanelTest panel) =>
            {
                print(panel.name + "加载完成了喵");
            });
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            UIPanelMgr.Instance.HidePanel("PlayPanel");
        }
    }
   
}
