using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.Die();
        // �������������������¼���������ʼλ�ã�
    }
}
