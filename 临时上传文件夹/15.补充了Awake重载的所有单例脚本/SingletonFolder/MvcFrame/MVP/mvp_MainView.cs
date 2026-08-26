using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mvp_MainView : MonoBehaviour
{
  public Button btnRole;
  public Button btnSkill;
  
  public Text txtName;
  public Text txtLevel;
  public Text txtAtk;

//现在没有UpdateInfo_View方法了,改为在Presenter中执行
//   public void UpdateInfo_View(PlayerModel playerModel)
//   {
//     txtName.text = playerModel.PlayerName;
//     txtAtk.text = playerModel.PlayerAtk.ToString();
//     txtLevel.text = playerModel.PlayerLevel.ToString();
//   }
}
