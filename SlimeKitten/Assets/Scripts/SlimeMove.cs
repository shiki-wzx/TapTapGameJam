using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMove : MonoBehaviour
{
    private static int s_pos, s_nor, s_rel, s_time;
    static SlimeMove()
    {
        s_pos = Shader.PropertyToID("_Position");
        s_nor = Shader.PropertyToID("_Normal");
        s_rel = Shader.PropertyToID("_Released");
        s_time = Shader.PropertyToID("_PointTime");
    }

    public SkinnedMeshRenderer skinnedMeshRenderer;
    private Rigidbody2D rb2d;
    private LineRenderer lr;

    private Camera mainCamera;
    private RaycastHit2D hit;
    private RaycastHit2D hitGround_1;
    private RaycastHit2D hitGround_2;

    private bool isClicked;
    private bool ready2Fly;
    [HideInInspector]
    public bool isOnGround;

    private Vector3 hitPosition;
    private Vector2 referralDirection;
    private Vector3 screenSpace;

    private Vector3 velocity;

    private int directionTag;
    private float deformationQuantity;

    private Vector3 currentFrameMousePosition;
    private Vector3 lastFrameMousePosition;
    private Vector3 offset;

    public int slimePiecesCount;
    public int largerPercent;
    public bool getKey;

    private List<Vector3> pointsList = new List<Vector3>();

    [Header("Parameters Of Bounce")]
    public float elasticCoefficient = 10f;

    [Header("Limits Of Dragging")]
    public float maxDraggingDistance = .5f;
    public float zoomout = .5f;
    public float referenceRange = 1f;

    [Header("Parameters Of Trail")]
    public float maxOffset;
    public int pointsCount = 100;

    [Header("Parameters Of Ground Check")]
    public float offsetX = .2f;
    public float offsetY = .5f;

    [Header("Parameters Of Move")]
    public float moveSpeed = .2f;

    [Header("Parameters Of Rotate")]
    public float rotateSpeed = 15f;

    private void Start()
    {
        //skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();

        mainCamera = Camera.main;

        isClicked = false;
        ready2Fly = false;
    }

    void Update()
    {
        if (!isOnGround)
        {
            CheckOnGround();

            Rotate();
        }
        else
        {
            Move();

            rb2d.bodyType = RigidbodyType2D.Dynamic;

            hitGround_1 = Physics2D.Raycast(new Vector2(transform.position.x - offsetX, transform.position.y), Vector2.down, offsetY, LayerMask.GetMask("Ground"));
            hitGround_2 = Physics2D.Raycast(new Vector2(transform.position.x + offsetX, transform.position.y), Vector2.down, offsetY, LayerMask.GetMask("Ground"));
            if (!hitGround_1 && !hitGround_2)
            {
                isOnGround = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 cameraPosition = mainCamera.transform.position;
                hit = Physics2D.Raycast(cameraPosition, mainCamera.ScreenToWorldPoint(Input.mousePosition) - cameraPosition, Mathf.Infinity, LayerMask.GetMask("Slime"));
                if (hit)
                {
                    isClicked = true;
                    ready2Fly = true;

                    hitPosition = hit.point;
                    referralDirection = hitPosition - transform.position;
                    screenSpace = Camera.main.WorldToScreenPoint(transform.position);

                    //Debug.Log("hit: " + hit.point);
                    //Debug.Log("ref: " + referralDirection);
                }
            }
        }

        if (isClicked)
        {
            // 弹弹弹有bug555
            OnElastic(hit);
            DataCalculate();
            if (CheckMousePosition())
            {
                DrawTrail();
            }
        }
        else
        {
            if (ready2Fly)
            {
                Fly(); // 对刚体操作 没放在fixedupdate
            }
        }

    }

    void DataCalculate()
    {
        // 拖拽位移计算
        Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(currentScreenSpace);
        // 位移
        Vector2 displacement = zoomout * (currentMousePosition - hitPosition);
        // 位移方向(1、0表示)、形变量(即速度反方向、大小)
        directionTag = Vector2.Dot(referralDirection, displacement) > 0 ? 1 : -1;
        deformationQuantity = Mathf.Clamp(displacement.magnitude, -maxDraggingDistance, maxDraggingDistance);
        // 速度
        float velocityValue = deformationQuantity * Mathf.Sqrt(elasticCoefficient / rb2d.mass);
        velocity = velocityValue * -displacement.normalized;
    }

    /// <summary>
    /// 弹弹弹
    /// s_rel(_Released) = 0，跟随鼠标拖拽形变
    /// s_rel(_Released) = 1，松开鼠标弹弹弹
    /// </summary>
    public void OnElastic(RaycastHit2D hit)
    {
        // 设置拖拽标志
        if (Input.GetMouseButton(0))
        {
            skinnedMeshRenderer.material.SetInt(s_rel, 0);
        }
        else
        {
            skinnedMeshRenderer.material.SetInt(s_rel, 1);
            // 重置时间（把shader 的中时间函数以现在的为基础计时）
            skinnedMeshRenderer.material.SetFloat(s_time, Time.time);
            isClicked = false;
        }

        // 反弹的坐标 (InverseTransformPoint 把世界坐标作为到自身坐标的位置)
        Vector4 v = transform.InverseTransformPoint(hitPosition);
        // 受影响顶点范围的半径
        v.w = referenceRange;
        skinnedMeshRenderer.material.SetVector(s_pos, v);
        // 法线方向，该值为顶点偏移方向，可自己根据需求传 (InverseTransformPoint 把世界法线坐标作为到自身法线坐标的位置)
        v = transform.InverseTransformDirection(directionTag * hit.normal.normalized);
        // 反弹力度
        v.w = deformationQuantity;
        skinnedMeshRenderer.material.SetVector(s_nor, v);
    }

    bool CheckMousePosition()
    {
        currentFrameMousePosition = Input.mousePosition;
        if (lastFrameMousePosition == Vector3.zero)
        {
            //空等一帧
        }
        else
        {
            offset = currentFrameMousePosition - lastFrameMousePosition;
        }
        lastFrameMousePosition = currentFrameMousePosition;

        return offset.magnitude > maxOffset;
    }

    void DrawTrail()
    {
        pointsList.Clear();
        for (int i = 0; i < pointsCount; i++)
        {
            float x = velocity.x * i * Time.deltaTime;
            float y = velocity.y * i * Time.deltaTime + .5f * Physics.gravity.y * Mathf.Pow(i * Time.deltaTime, 2);
            pointsList.Add(transform.position + new Vector3(x, y));
        }
        lr.positionCount = pointsList.Count;
        for (int i = 0; i < pointsList.Count; i++)
        {
            lr.SetPosition(i, pointsList[i]);
        }
    }

    void Fly()
    {
        rb2d.velocity = velocity;
        ready2Fly = false;
    }

    void CheckOnGround()
    {
        hitGround_1 = Physics2D.Raycast(new Vector2(transform.position.x - offsetX, transform.position.y), Vector2.down, offsetY, LayerMask.GetMask("Ground"));
        hitGround_2 = Physics2D.Raycast(new Vector2(transform.position.x + offsetX, transform.position.y), Vector2.down, offsetY, LayerMask.GetMask("Ground"));
        if (hitGround_1 && hitGround_2)
        {
            rb2d.GetComponent<Transform>().rotation = new Quaternion(0, 0, 0,0);
            rb2d.bodyType = RigidbodyType2D.Static;
            isOnGround = true;
        }
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }

    }

    void Rotate()
    {
        if (Input.GetMouseButton(0))
        {
            transform.eulerAngles += new Vector3(0, 0, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetMouseButton(1))
        {
            transform.eulerAngles -= new Vector3(0, 0, rotateSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector2(transform.position.x - offsetX, transform.position.y), new Vector2(transform.position.x - offsetX, transform.position.y - offsetY));
        Gizmos.DrawLine(new Vector2(transform.position.x + offsetX, transform.position.y), new Vector2(transform.position.x + offsetX, transform.position.y - offsetY));
    }
}
