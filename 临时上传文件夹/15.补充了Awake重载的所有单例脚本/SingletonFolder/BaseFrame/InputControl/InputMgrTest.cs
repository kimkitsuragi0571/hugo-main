using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgrTest : MonoBehaviour
{
    void Start()
    {
        //先开启输入检测
        InputMgr.Instance.SwitchCheck(true);
        //添加事件监听即可(这里调用的是泛型方法哈)
        GenericEventCenterMgr.Instance.AddEvent<KeyCode>("按下Q键", SkillQ);
        GenericEventCenterMgr.Instance.AddEvent<KeyCode>("按下W键", SkillW);
        GenericEventCenterMgr.Instance.AddEvent<KeyCode>("按下E键", SkillE);
    }

    private void SkillQ(KeyCode keyCode)
    {
        print("触发了Q技能");
    }
    private void SkillW(KeyCode keyCode)
    {
        print("触发了W技能");
    }
    private void SkillE(KeyCode keyCode)
    {
        print("触发了E技能");
    }
}
