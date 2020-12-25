using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.diedTime++;
        // 死亡动画？动画结束事件传送至初始位置？
    }
}
