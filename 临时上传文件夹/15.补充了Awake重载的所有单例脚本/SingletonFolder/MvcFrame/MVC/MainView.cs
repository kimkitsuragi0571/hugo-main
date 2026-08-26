using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
{
  public Button btnRole;
  public Button btnSkill;
  
  public Text txtName;
  public Text txtLevel;
  public Text txtAtk;

  //这里传入的是PlayerModel类唯一实例Instance,从其中获取指定属性直接给
  //方法名相同不用在意,没啥特殊的
  public void UpdateInfo_View(PlayerModel playerModel)
  {
    txtName.text = playerModel.PlayerName;
    txtAtk.text = playerModel.PlayerAtk.ToString();
    txtLevel.text = playerModel.PlayerLevel.ToString();
  }
}
