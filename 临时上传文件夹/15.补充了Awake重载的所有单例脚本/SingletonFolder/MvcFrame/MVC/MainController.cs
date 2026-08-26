using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
   private MainView mainView;
   private static MainController instance;

  
   public static MainController Instance
   {
      get
      {
         return instance;
      }
   }

   private void Start()
   {
     //刚创建面板时,获取脚本组件,并且更新面板数据(从PlayerModel)
      mainView = this.GetComponent<MainView>();
      mainView.UpdateInfo_View(PlayerModel.Instance);
      //订阅Model事件
      PlayerModel.Instance.AddEvent(UpdateInfo_Controller);
   }

   //收到PlayerModel数据->UpdateCon->传递给UpdateView
   private void UpdateInfo_Controller(PlayerModel playerModel)
   {
      if (mainView != null)
      {
         mainView.UpdateInfo_View(playerModel);
      }
   }

   private void OnDestroy()
   {
      PlayerModel.Instance.RemoveEvent(UpdateInfo_Controller);
   }


   //就是很普通的显隐方法
   public static void ShowView()
   {
      if (instance == null)
      {
         GameObject panelObj = Resources.Load<GameObject>("Prefabs/UI/MainView");
         GameObject mainViewGo = Instantiate(panelObj); 
         mainViewGo.transform.SetParent(GameObject.Find("Canvas").transform, false);
         instance = mainViewGo.GetComponent<MainController>();
      }
      instance.gameObject.SetActive(true);
   }

   public static void HideView()
   {
         if (instance != null)
         {
            instance.gameObject.SetActive(false);
         }
      
   }
}
