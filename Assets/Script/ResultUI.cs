using System;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField]
    private OthelloPlayer _Player1;
    [SerializeField]
    private GameObject UI_Start;
    [SerializeField]
    private GameObject _DRAWPanel;
    [SerializeField]
    private GameObject _P1WINPanel;
    [SerializeField]
    private GameObject _P2WINPanel;
    [SerializeField]
    private TextMeshProUGUI _player1_Type;
    [SerializeField]
    private TextMeshProUGUI _player1_Score;
    [SerializeField]
    private TextMeshProUGUI _player2_Type;
    [SerializeField]
    private TextMeshProUGUI _player2_Score;
    [SerializeField]
    private float[] UISetUpScorePosY_Player1 = new float[]{0,100,-100};//0=DRAW,1=P1WIN,2=P2WIN
    [SerializeField]
    private float[] UISetUpScorePosY_Player2 = new float[]{0,-100,100};//0=DRAW,1=P1WIN,2=P2WIN
    private int _result=0; //0=DRAW,1=P1WIN,2=P2WIN
    public void Start()
    {
        OthelloGameManager.onGameOver += Open;
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        Debug.Log("★★★ResiltUI Open");
        if(_Player1.StoneType == StoneAndTarget.Type.BlackStone)
            ResultUISet(true,OthelloGameManager.BlackStoneCount,OthelloGameManager.WhiteStoneCount);
        else
            ResultUISet(false,OthelloGameManager.WhiteStoneCount,OthelloGameManager.BlackStoneCount);
        UILayoutSet();
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        Debug.Log("★★★ResiltUI Close");
        this.gameObject.SetActive(false);
        UI_Start.SetActive(true);
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
        p1rect.anchoredPosition = new Vector2(p1rect.anchoredPosition.x,UISetUpScorePosY_Player1[_result]);
        p2rect.anchoredPosition = new Vector2(p2rect.anchoredPosition.x,UISetUpScorePosY_Player2[_result]);
        
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
}
