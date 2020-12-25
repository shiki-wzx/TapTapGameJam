using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePiece : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<SlimeDeform>().form == Form.eat)
        {
            Destroy(gameObject);
        }
    }
}
