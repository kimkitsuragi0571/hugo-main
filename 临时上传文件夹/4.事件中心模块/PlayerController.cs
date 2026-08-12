using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            EventCenter.Instance.EventTrigger("OnPlayerShoot");
        }
    }
}