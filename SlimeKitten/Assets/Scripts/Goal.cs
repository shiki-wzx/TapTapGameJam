using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SlimeMove>().getKey)
        {
            collision.GetComponent<SlimeMove>().getKey = false;
            GameManager.Instance.LevelPass();
        }
    }
}
