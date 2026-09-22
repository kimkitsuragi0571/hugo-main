using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

public class J1_JsonReadAndWrite : MonoBehaviour
{
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            print("写入特定字符串到Json文件中(但是这样也没有{}等符号,有点蠢)");
            File.WriteAllText
            (
                Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json",
                "\"PlayerName\":\"Valira\""
            );
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("读取Json文件字符串");
            print(File.ReadAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json"));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("将对象JsonUtility序列化为string,然后写入文件");
            Lurker valira = new Lurker();
            string val = JsonUtility.ToJson(valira);
            File.WriteAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json", val);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("读取Json文件中string,转化为对象");
            string val = File.ReadAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json");
            Lurker valira = JsonUtility.FromJson<Lurker>(val);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("将对象LitJson序列化为string,然后写入文件");
            Lurker valira = new Lurker();
            string val = JsonMapper.ToJson(valira);
            File.WriteAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json", val);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            print("LitJson直接读取Json文件中顶层数据");
            List<Weapon> weapons =
                JsonMapper.ToObject<List<Weapon>>(Application.persistentDataPath + "/" + "WeaponData_Valira" + ".json");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            print("LitJson反序列化Json中对象");
            string val = File.ReadAllText(Application.persistentDataPath + "/" + "PlayerData_Valira" + ".json");
            Lurker valira = JsonMapper.ToObject<Lurker>(val);
            print(valira.PlayerName);
        }
    }
}

[System.Serializable]
public class Lurker
{
    public string PlayerName = "Valira";
    public string Occupation = "Wizard";
    
    //private属性和public变量,都不会被存储
    private string hideSkill = "Rasengan";
    public string HideSkill
    {
        get
        {
            return hideSkill;
        }
        set
        {
            hideSkill = value;
        }
    }

    public Weapon[] Weapons = new[]
    {
        new Weapon("Dagger", 1),
        new Weapon("machete", 2)
    };

    //不支持字典
    public Dictionary<string, string> missionDict = new Dictionary<string, string>()
    {
        { "no.1","Hand Job" },
        { "no.2","Hand Job Again" }
    };
}
[System.Serializable]
public class Weapon
{
    public string WeaponName;
    public int ATK;

    public Weapon(string weaponName, int atk)
    {
        this.WeaponName = weaponName;
        this.ATK = atk;
    }
        
}