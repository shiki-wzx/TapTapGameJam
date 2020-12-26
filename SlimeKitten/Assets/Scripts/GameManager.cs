using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static readonly GameManager instance = new GameManager();
    private static int currentLevel;
    private static int diedTime;

    public bool newLevelIsSet;

    static GameManager() 
    {
        currentLevel = 0;
        // 本地存储游戏记录？
        // currentLevel = PlayerPrefs.GetInt("currentLevl");
        // diedTime = PlayerPrefs.GetInt("diedTime");
    }

    private GameManager() { }

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void LevelPass()
    {
        currentLevel++;
        newLevelIsSet = false;
    }

    public void Die()
    {
        diedTime++;
        newLevelIsSet = false;
    }
}
