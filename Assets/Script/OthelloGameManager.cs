using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloGameManager : MonoBehaviour
{
    [SerializeField]
    private Transform _stones;
    [SerializeField]
    private ThrowStone _throwStone;
    [SerializeField]
    private bool _isTurnBlack = false;
    [SerializeField]
    private Timer _timerUI;

    //置けるTarget石のリスト
    public static List<Transform> TargetList { get; private set; } = new List<Transform>();
    //残りの数
    public static int RemainingCount { get; private set; } = 0;
    //白の数
    public static int WhiteStoneCount { get; private set; } = 0;
    //黒の数
    public static int BlackStoneCount { get; private set; } = 0;

    public delegate void CallbackGameOver();
    public static event CallbackGameOver onGameOver;

    //開始
    public void GameStart()
    {
        AllSetAroundStoneAndTargetList();
        //黒先手、白後手
        _isTurnBlack = !_isTurnBlack;
        foreach (Transform t in _stones)
        {
            t.GetComponent<StoneAndTarget>().Init();
        }
        ChangeTurn();
        _throwStone.ChangeTurnEvent = () => ChangeTurn();
        _timerUI.ChangeTurnEvent = () => ChangeTurn();
    }

    //ターン変更
    public void ChangeTurn()
    {
        _isTurnBlack = !_isTurnBlack;
        RemainingCount = 0;
        WhiteStoneCount = 0;
        BlackStoneCount = 0;
        foreach (Transform t in _stones)
        {
            var stone = t.GetComponent<StoneAndTarget>();
            if (stone.StoneType == StoneAndTarget.Type.Target)
            {
                stone.Change(StoneAndTarget.Type.None);
                RemainingCount++;
            }
            else if (stone.StoneType == StoneAndTarget.Type.None)
            {
                RemainingCount++;
            }
            else if (stone.StoneType == StoneAndTarget.Type.BlackStone)
            {
                BlackStoneCount++;
            }
            else if (stone.StoneType == StoneAndTarget.Type.WhiteStone)
            {
                WhiteStoneCount++;
            }
        }

        //ゲームオーバー
        if (RemainingCount == 0)
        {
            Debug.Log("★★★GameOver");
            onGameOver?.Invoke();
            return;
        }

        //ターゲット作成
        CreateTarget();

        //ターン変更手続き
        _throwStone.ChangeTurn(_isTurnBlack);
        _timerUI.Restart(_isTurnBlack);

        //置けるところがない
        if (TargetList.Count == 0)
        {
            NoTarget();
        }
    }

    private void CreateTarget()
    {
        var list = new List<StoneAndTarget>();
        StoneAndTarget stone = null;
        TargetList.Clear();

        foreach (Transform t in _stones)
        {
            stone = t.GetComponent<StoneAndTarget>();

            //既に置いてる石は判定しない
            if (stone.StoneType == StoneAndTarget.Type.BlackStone || stone.StoneType == StoneAndTarget.Type.WhiteStone)
            {
                continue;
            }

            //何も置いてない場所から置けるか判定
            if (_isTurnBlack)
            {
                list = stone.GetImpactList(StoneAndTarget.Type.BlackStone);
            }
            else
            {
                list = stone.GetImpactList(StoneAndTarget.Type.WhiteStone);
            }

            if (list == null)
            {
                continue;
            }

            if (list.Count > 0)
            {
                //置けるところを追加
                stone.Change(StoneAndTarget.Type.Target);
                TargetList.Add(t);
            }
        }
    }

    private void NoTarget()
    {
        Debug.Log("★★★NoTarget");
    }

    public void AllSetAroundStoneAndTargetList()
    {
        foreach (Transform t in _stones)
        {
            t.GetComponent<StoneAndTarget>().SetAroundStoneAndTargetList();
        }
    }

    private int debugTargetNo = 0;
    public void DebugMoveThrwStone()
    {
        if (TargetList.Count == 0)
        {
            Debug.Log("★★★No Target");
            return;
        }
        //引き分けパターン
        debugTargetNo++;
        if (debugTargetNo > TargetList.Count)
        {
            debugTargetNo = 1;
        }
        Debug.Log($"★★★ TargetNo = {debugTargetNo}/{TargetList.Count}");
        _throwStone.transform.position = TargetList[debugTargetNo - 1].position;
    }
}
