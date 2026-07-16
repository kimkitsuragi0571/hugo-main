using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
  
    
    void Start()
    {
      AudioManager_1.Instance.DataInit();
      DataManager_2.Instance.DataInit();
      MonsterManager_3.Instance.DataInit();
      WeaponManager_4.Instance.DataInit();
      ItemManager_5.Instance.DataInit();
      PackageManager_6.Instance.DataInit();
      UIManager_7.Instance.DataInit();
      SceneManager_8.Instance.DataInit();
      
    }

 
    void Update()
    {
        
    }
}

public class Student
{
    public static Student stu = new Student();
    private string _name;
    private int _age;

    //var隐式类型
    public void Speak(int index)
    {
        var val = index;
        Debug.Log(val);
    }

    public int Age
    {
        set { _age = value;}
        get { return _age; }
    }
    
    public string Name
    {
        set { _name = value;}
        get { return _name; }
    }
    
    public Student(int age, string name)
    {
        _age = age;
        _name = name;
    }

    public Student()
    {
        
    }
}