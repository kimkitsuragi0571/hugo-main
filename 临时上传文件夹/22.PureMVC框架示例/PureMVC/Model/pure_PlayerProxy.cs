using System.Collections;
using System.Collections.Generic;
using PureMVC.Patterns.Proxy;
using UnityEngine;

//继承Proxy父类后,还必须实现对应的构造函数
public class pure_PlayerProxy : Proxy
{
    public new const string NAME = "pure_PlayerProxy";
    
    //构造函数:初始化代理类的时候,统一添加到Proxy成员便于管理(相当于字典嘛,名字和数据)
    
    //参数分别为:代理名称,代理相关数据(这里是指定参数传入父类构造),这里用简便一点的写法(data默认null,本来就可以不用写)
    //public pure_PlayerProxy(string proxyName, object data = null) : base(proxyName, data)
    
    //这个base构造:只有一个pure_PlayerProxy实例,调用base只是初始化父类的字段,然后初始化子类独有字段
    public pure_PlayerProxy() : base(pure_PlayerProxy.NAME)
    {
        pure_PlayerDataObj playerData = new pure_PlayerDataObj();
        //初始化(后面的只是默认值)
        playerData.playerName = PlayerPrefs.GetString("PlayerName","JOJO");
        playerData.playerLev = PlayerPrefs.GetInt("PlayerLevel", 2);
        playerData.playerAtk = PlayerPrefs.GetInt("PlayerAtk", 99);
        
        //Data是Proxy内部的一个属性(是单独的Object,肯定没法直接存储多个字段,所以需要playerData对象)
        //就是把playerData实例中的字段赋值给自身pure_PlayerProxy对象的Data字段
        Data = playerData;
    }
    
    public void LevUp()
    {
        //需要先向下转型
        pure_PlayerDataObj data = Data as pure_PlayerDataObj;
        data.playerLev += 1;
        data.playerAtk += data.playerLev;
        SaveData();
    }

    private void SaveData()
    {
        pure_PlayerDataObj data = Data as pure_PlayerDataObj;
        PlayerPrefs.SetString("PlayerName", data.playerName);
        PlayerPrefs.SetInt("PlayerLevel", data.playerLev);
        PlayerPrefs.SetInt("PlayerAtk", data.playerAtk);
    }
}
