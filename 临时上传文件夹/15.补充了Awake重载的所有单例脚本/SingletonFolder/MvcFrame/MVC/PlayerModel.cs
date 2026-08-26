using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerModel
{
    //数据类具有唯一性
    private static PlayerModel instance;

    public static PlayerModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerModel();
                Instance.Init();
            }

            return instance;
        }
    }

    //具体的数据内容
    private string playerName;
    private int playerLev;

    private int playerAtk;

    //数据都做属性
    public string PlayerName => playerName;
    public int PlayerLevel => playerLev;
    public int PlayerAtk => playerAtk;

    //通知外部更新的事件,这里需要传入 Model类自身实例Instance 作为参数
    private event UnityAction<PlayerModel> updateEvent;

    //初始化(直接从注册表读取数据赋值给成员属性)
    public void Init()
    {
        //后面的是初始值
        playerName = PlayerPrefs.GetString("PlayerName", "Bingo");
        playerLev = PlayerPrefs.GetInt("PlayerLevel", 2);
        playerAtk = PlayerPrefs.GetInt("PlayerAtk", 12);
    }

    //升级
    public void levUp()
    {
        playerLev += 1;
        playerAtk += playerLev;
        //当数据变动,就会调用 保存 和 通知外部数据更新 的方法
        SaveData();
        UpdateInfo_Model();
    }

    //就写把修改后的全局变量存入注册表而已
    private void SaveData()
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("PlayerLevel", playerLev);
        PlayerPrefs.SetInt("PlayerAtk", playerAtk);
    }

    //数据修改->通知更新的方法->调用通知的事件
    private void UpdateInfo_Model()
    {
        if (updateEvent != null)
        {
            //传入的是Instance实例
            updateEvent?.Invoke(this);
        }
    }

    //增减事件监听
    public void AddEvent(UnityAction<PlayerModel> act)
    {
        updateEvent += act;
    }

    public void RemoveEvent(UnityAction<PlayerModel> act)
    {
        updateEvent -= act;
    }
}


