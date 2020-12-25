using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDeform : MonoBehaviour
{
    private Animator animator;
    [HideInInspector]
    public Form form;

    void Start()
    {
        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //animator.SetTrigger("Eat");
            form = Form.eat; // 动画事件，复原form
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetTrigger("Longer");
            form = Form.longer;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Bullon");
            form = Form.bullon;
        }
    }
}

public enum Form
{
    origin,
    eat,
    longer,
    bullon
}

