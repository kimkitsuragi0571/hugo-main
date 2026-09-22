using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pure_MainEntry : MonoBehaviour
{
    void Start()
    {
        pure_Facade.Instance.StartUp();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            pure_Facade.Instance.SendNotification
            (
                pure_Notification.SHOW_PANEL,
                "MainPanel"
            );      
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            pure_Facade.Instance.SendNotification
            (
                pure_Notification.HIDE_PANEL, 
                //经过我们修改,这里直接传入"MainView"也可以了
                pure_Facade.Instance.RetrieveMediator(pure_MainView_Mediator.NAME)
            );   
        }
    }
}
