using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly GameManager instance = new GameManager();

    static GameManager() { }

    private GameManager() { }

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public int diedTime;

    void Start()
    {
        // 本地存储游戏记录？
        diedTime = PlayerPrefs.GetInt("diedTime");
    }


    void Update()
    {
        
    }
}
