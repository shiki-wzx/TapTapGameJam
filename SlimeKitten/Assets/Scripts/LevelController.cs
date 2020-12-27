using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public List<Level> levels = new List<Level>();

    public Transform slime;

    public float lerpTime = .1f;
    public float followingOffset = 5f;
    public float followingLerpTime = .15f;

    private CameraPosition cp;
    private Vector3 currentCameraPosition;

    private bool following;

    void Update()
    {
        if (!GameManager.Instance.newLevelIsSet)
        {
            following = false;
            SetLevel(GameManager.Instance.GetCurrentLevel());
        }
        else
        {
            if (cp.isWideLevel)
            {
                FollowAndClampCameraPosition();
            }
        }
    }

    public Level GetCurrentLevel()
    {
        return levels[GameManager.Instance.GetCurrentLevel()];
    }

    void SetLevel(int level)
    {
        // 动画问题
        slime.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        slime.position = levels[level].slimePosition + new Vector3(0, .3f, 0); // 给的坐标存在偏差
        slime.eulerAngles = Vector3.zero;
        slime.GetComponentInChildren<Animator>().SetBool("StandBy", true);
        slime.GetComponent<SlimeMove>().slimePiecesCount = 0;
        slime.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        // 环境
        foreach (GameObject go in levels[level].gameObjects)
        {
            go.SetActive(true);
        }

        // Set Camera Position
        cp = levels[level].cameraPosition;
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
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, followingLerpTime);
        }
    }
}

[Serializable]
public class Level
{
    public List<GameObject> gameObjects;
    public Vector3 slimePosition;
    public CameraPosition cameraPosition;
    public int needSlimePieces;
    public bool needKey;
    public bool newSkill;
}

[Serializable]
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
