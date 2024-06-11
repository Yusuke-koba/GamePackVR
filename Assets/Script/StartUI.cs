using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    public OthelloGameManager OthelloGameManager;

    public void GameStart()
    {
        OthelloGameManager.GameStart();
        gameObject.SetActive(false);
    }
}
