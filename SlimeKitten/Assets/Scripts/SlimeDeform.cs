using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDeform : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private SlimeMove sm;
    [HideInInspector]
    public Form form;
    [HideInInspector]
    public int status;
    private bool setStrip;
    private float timer;

    public float balloonForce = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponentInParent<Rigidbody2D>();
        sm = GetComponentInParent<SlimeMove>();

        form = Form.normal;
    }

    void Update()
    {
        animator.SetBool("StandBy", sm.isOnGround);
        if (!sm.isOnGround)
        {
            if (Input.GetKeyDown(KeyCode.Q) && status == 1)
            {
                animator.SetTrigger("Eat");
            }

            if (Input.GetKeyDown(KeyCode.W) && status == 2)
            {
                animator.SetTrigger("Strip");
                setStrip = true;
            }

            if (Input.GetKeyDown(KeyCode.E) && status == 3)
            {
                animator.SetTrigger("Balloon");
            }
        }
    }

    private void FixedUpdate()
    {
        if (setStrip)
        {
            if (timer == 0)
                rb2d.velocity += 3 * new Vector2(transform.up.x, transform.up.y);
            timer += Time.fixedDeltaTime;
        }
        if (timer > 2f)
        {
            rb2d.velocity -= 3 * new Vector2(transform.up.x, transform.up.y);
            setStrip = false;
            timer = 0;
        }
    }

    public void StartEat()
    {
        form = Form.eat;
    }

    public void StopEat()
    {
        form = Form.normal;
    }

    public void Balloon()
    {
        rb2d.AddForce(balloonForce * transform.up);
    }
}

public enum Form
{
    normal,
    eat,
    strip,
    balloon
}

