using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInChildren<SlimeDeform>().form == Form.eat)
        {
            collision.GetComponent<SlimeMove>().getKey = true;
            gameObject.SetActive(false);
        }
            
    }
}
