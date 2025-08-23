using System;
using TMPro;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ResultUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _player1;
    [SerializeField]
    private GameObject _player2;
    [SerializeField]
    private GameObject _UI_Start;
    [SerializeField]
    private GameObject _DRAWPanel;
    [SerializeField]
    private GameObject _P1WINPanel;
    [SerializeField]
    private GameObject _P2WINPanel;
    [SerializeField]
    private Ranking _RankingUI;
    [SerializeField]
    private TextMeshProUGUI _player1_Type;
    [SerializeField]
    private TextMeshProUGUI _player1_Score;
    [SerializeField]
    private TextMeshProUGUI _player2_Type;
    [SerializeField]
    private TextMeshProUGUI _player2_Score;
    [SerializeField]
    private float[] _UISetUpScorePosY_Player1 = new float[]{0,100,-100};//0=DRAW,1=P1WIN,2=P2WIN
    [SerializeField]
    private float[] _UISetUpScorePosY_Player2 = new float[]{0,-100,100};//0=DRAW,1=P1WIN,2=P2WIN
    private int _result=0; //0=DRAW,1=P1WIN,2=P2WIN
    [SerializeField]
    private GameObject SameAIUI;

    public void Start()
    {
        OthelloGameManager.onGameOver += Open;
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        Debug.Log("★★★ResiltUI Open");
        if(_player1.GetComponent<OthelloPlayer>().StoneType == StoneAndTarget.Type.BlackStone)
            ResultUISet(true,OthelloGameManager.BlackStoneCount,OthelloGameManager.WhiteStoneCount);
        else
            ResultUISet(false,OthelloGameManager.WhiteStoneCount,OthelloGameManager.BlackStoneCount);
        UILayoutSet();
        this.gameObject.SetActive(true);
        RankingDataSave();
    }

    public void Close()
    {
        Debug.Log("★★★ResiltUI Close");
        Destroy(_player1.GetComponent<NPCAIBase>());
        Destroy(_player2.GetComponent<NPCAIBase>());
        this.gameObject.SetActive(false);
        _UI_Start.SetActive(true);
    }

    public void ResultUISet(bool player1isBlack,int player1Score,int player2Score)
    {
        if(player1isBlack)
        {
            _player1_Type.text = "Black";
            _player2_Type.text = "White";
        }
        else
        {
            _player1_Type.text = "White";
            _player2_Type.text = "Black";
        }
        _player1_Score.text = player1Score.ToString();
        _player2_Score.text = player2Score.ToString();
        if(player1Score == player2Score)
            _result=0;
        else if(player1Score > player2Score)
            _result=1;
        else
            _result=2;
    }

    public void UILayoutSet()
    {
        _DRAWPanel.SetActive(false);
        _P1WINPanel.SetActive(false);
        _P2WINPanel.SetActive(false);

        //スコアの位置を変更
        var p1rect = _player1_Score.GetComponent<RectTransform>();
        var p2rect = _player2_Score.GetComponent<RectTransform>();
        p1rect.anchoredPosition = new Vector2(p1rect.anchoredPosition.x,_UISetUpScorePosY_Player1[_result]);
        p2rect.anchoredPosition = new Vector2(p2rect.anchoredPosition.x,_UISetUpScorePosY_Player2[_result]);
        
        switch(_result)
        {
            case 0:
            Debug.Log ("★★★Draw");
            _DRAWPanel.SetActive(true);
            break;
            case 1:
            Debug.Log ("★★★player1 Win");
            _P1WINPanel.SetActive(true);
            break;
            case 2:
            Debug.Log ("★★★player2 Win");
            _P2WINPanel.SetActive(true);
            break;
        }
    }

    public void OpenRanking()
    {
        //同じAI同士の戦いはランキングに反映無し
        if(_player1.GetComponent<NPCAIBase>().Title().Equals(_player2.GetComponent<NPCAIBase>().Title()))
            StartCoroutine(SameAIUIEvent());
        _RankingUI.Open();
    }

    public void RankingDataSave()
    {
        string winNPCName="";
        int winScore=0;
        string loseNPCName="";
        int loseScore=0;
        winNPCName = _player1.GetComponent<NPCAIBase>().Title();
        winScore = int.Parse(_player1_Score.text);
        loseNPCName = _player2.GetComponent<NPCAIBase>().Title();
        loseScore = int.Parse(_player2_Score.text);
        if(_result == 2)
        {
            winNPCName = _player2.GetComponent<NPCAIBase>().Title();
            winScore = int.Parse(_player2_Score.text);
            loseNPCName = _player1.GetComponent<NPCAIBase>().Title();
            loseScore = int.Parse(_player1_Score.text);
        }
        _RankingUI.GameDataSave(winNPCName, winScore, loseNPCName, loseScore);
    }

    public IEnumerator SameAIUIEvent()
    {
        SameAIUI.SetActive(true);
        yield return new WaitForSeconds(10);
        SameAIUI.SetActive(false);
    }
}
