using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pure_RoleView : MonoBehaviour
{
    public Button btnClose;
    public Button btnLevUp;
    
    public Text txtAtk;
    
    public void UpdateInfo_View(pure_PlayerDataObj playerData)
    {
        if (playerData == null || txtAtk == null) return;
        txtAtk.text = playerData.playerAtk.ToString();
    }
}
