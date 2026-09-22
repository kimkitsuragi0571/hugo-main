using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pure_MainView : MonoBehaviour
{
    public Button btnRole;
    public Button btnSkill;
  
    public Text txtName;
    public Text txtLevel;
    public Text txtAtk;

   //如果是根据MVC的思路来做,这里就可以传入更新方法
   //按照MVP的话只需要放到Presenter里面
    public void UpdateInfo_View(pure_PlayerDataObj playerData)
    {
        txtName.text = playerData.playerName;
        txtAtk.text = playerData.playerAtk.ToString();
        txtLevel.text = playerData.playerLev.ToString();
    }
}
