using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Level level = GameObject.Find("LevelController").GetComponent<LevelController>().GetCurrentLevel();
        SlimeMove sm = collision.GetComponent<SlimeMove>();
        if (level.needSlimePieces == sm.slimePiecesCount)
        {
            if (level.needKey)
            {
                if (sm.getKey)
                {
                    collision.GetComponent<SlimeMove>().getKey = false;
                    if (level.newSkill)
                    {
                        collision.GetComponentInChildren<SlimeDeform>().status++; ;
                    }
                    GameManager.Instance.LevelPass();
                    gameObject.SetActive(false);
                }
                
            }
            else
            {
                collision.GetComponent<SlimeMove>().getKey = false;
                if (level.newSkill)
                {
                    collision.GetComponentInChildren<SlimeDeform>().status++; ;
                }
                GameManager.Instance.LevelPass();
                gameObject.SetActive(false);
            }
        }
    }
}
