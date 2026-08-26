using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOfTriggerEvent : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("开始触发事件中心PlayerAtk方法");
            //这里触发的是整个字典的Key,里面可能对应多个事件
            EventCenterMgr.Instance.EventTrigger("OnPlayerAtk");
            //使用这个事件中心后:
            //之前你必须引入扣血的怪物,现在只要触发Event,场景中订阅了Event的小怪自动扣血(Player不需要知道是谁)
            //MonsterEvent 只关心“自己怎么掉血、怎么死”,PlayerOfTriggerEvent 只关心“什么时候触发攻击信号”
        }
    }
}
