using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int HP = 100;

    //物体添加到订阅列表
    void Awake()
    {
        //看似只是存储了一个函数名,实际委托间接创建了一个类实例引用和方法本身的引用
        //总之事件的订阅列表实际上其实关联了实例方法和实例两部分
        EventCenter.Instance.AddEvent("OnPlayerShoot", OnBeShot);
    }

    void OnBeShot()
    {
        //这里定义类字段HP,并且访问HP时
        //默认相当于this.HP -= 10,优先访问
        HP -= 10;
        if (HP <= 0)
        {
            Debug.Log("敌人死亡！");
            // 死亡时销毁物体，触发 OnDestroy
            Destroy(gameObject);
        }
    }

    //物体销毁时移除订阅
    void OnDestroy()
    {
        //即使方法内部只是Debug.Log仍然需要销毁订阅(因为委托还是关联了实例)
        //如果是静态方法就不需要销毁订阅了
        EventCenter.Instance.RemoveEvent("OnPlayerShoot", OnBeShot);
    }
}