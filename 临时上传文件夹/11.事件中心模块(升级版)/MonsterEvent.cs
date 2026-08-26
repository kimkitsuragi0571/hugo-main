using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEvent : MonoBehaviour
{
   public int HP = 100;

   void Awake()
   {
      EventCenterMgr.Instance.AddEvent("OnPlayerAtk",PlayerAtk);
   }

   void PlayerAtk()
   {
      this.HP -= 10;
      if (this.HP <= 0)
      {
         print("Player打史了Monster");
         Destroy(this.gameObject);
      }
   }

   //哦哦就是普通的物体销毁时调用
   //不仅要销毁物体,还需要移除订阅引用,防止内存泄漏
   void OnDestroy()
   {
      EventCenterMgr.Instance.RemoveEvent("OnPlayerAtk",PlayerAtk);
   }
}
