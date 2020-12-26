using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static List<CameraPosition> cameraPositions = new List<CameraPosition>();

    public Transform slime;

    private CameraPosition cp;
    private Vector3 currentCameraPosition;

    private bool following;

    public float lerpTime = .1f;
    public float followingOffset = 5f;

    void Awake()
    {
        cameraPositions.Add(new CameraPosition(false, -16.43f));
        cameraPositions.Add(new CameraPosition(false, -1.03f));
        cameraPositions.Add(new CameraPosition(true, 16.81f, 24.81f));
        cameraPositions.Add(new CameraPosition(true, 42.08f, 50.08f));
        cameraPositions.Add(new CameraPosition(false, 66.89f));
        cameraPositions.Add(new CameraPosition(true, 82.74f, 100.74f));
        cameraPositions.Add(new CameraPosition(true, 115.09f, 132.51f));
    }

    void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameManager.Instance.LevelPass();
        }

        if (!GameManager.Instance.newLevelIsSet)
        {
            following = false;
            SetCameraPosition(GameManager.Instance.GetCurrentLevel());
        }
        else
        {
            if (cp.isWideLevel)
            {
                FollowAndClampCameraPosition();
            }
        }
    }

    void SetCameraPosition(int level)
    {
        cp = cameraPositions[level];
        currentCameraPosition = Camera.main.transform.position;
        Vector3 nextLevelPosition = new Vector3(cp.minX, currentCameraPosition.y, currentCameraPosition.z);
        GameManager.Instance.newLevelIsSet = (nextLevelPosition - currentCameraPosition).sqrMagnitude < .1f;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, nextLevelPosition, lerpTime);
    }

    void FollowAndClampCameraPosition()
    {
        if (currentCameraPosition.x - slime.position.x < followingOffset)
        {
            following = true;
        }

        if (following)
        {
            float x = slime.position.x + followingOffset;
            x = Mathf.Clamp(x, cp.minX, cp.maxX);
            Vector3 targetPosition = new Vector3(x, currentCameraPosition.y, currentCameraPosition.z);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, lerpTime);
        }
    }
}

public class CameraPosition
{
    public bool isWideLevel;
    public float minX;
    public float maxX;

    public CameraPosition(bool isWideLevel, params float[] Xs)
    {
        this.isWideLevel = isWideLevel;
        minX = Xs[0];
        if (isWideLevel)
        {
            maxX = Xs[1];
        }
    }
}
