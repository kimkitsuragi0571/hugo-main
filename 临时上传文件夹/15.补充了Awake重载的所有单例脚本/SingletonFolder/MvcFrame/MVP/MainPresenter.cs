using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPresenter : MonoBehaviour
{
    private mvp_MainView mainView;
    private static MainPresenter instance;

  
    public static MainPresenter Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        mainView = this.GetComponent<mvp_MainView>();
        //2.自然这里也要改为调用Presenter中的方法
        UpdateInfo_Presenter(PlayerModel.Instance);
        PlayerModel.Instance.AddEvent(UpdateInfo_Presenter);
    }
    
    //1.只是把View中的赋值改到了Presenter中,没啥东西
    private void UpdateInfo_Presenter(PlayerModel playerModel)
    {
        if (mainView != null)
        {
           mainView.txtName.text = playerModel.PlayerName;
           mainView.txtAtk.text = playerModel.PlayerAtk.ToString();
           mainView.txtLevel.text = playerModel.PlayerLevel.ToString();
        }
    }

    private void OnDestroy()
    {
        PlayerModel.Instance.RemoveEvent(UpdateInfo_Presenter);
    }


    //显隐方法
    public static void ShowView()
    {
        if (instance == null)
        {
            GameObject panelObj = Resources.Load<GameObject>("Prefabs/UI/MainView");
            GameObject mainViewGo = Instantiate(panelObj); 
            mainViewGo.transform.SetParent(GameObject.Find("Canvas").transform, false);
            instance = mainViewGo.GetComponent<MainPresenter>();
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