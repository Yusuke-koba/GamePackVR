using System;
using TMPro;
using UnityEngine;

public class ResiltUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ResiltText;

    public void Start()
    {
        OthelloGameManager.onGameOver += Open;
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        Debug.Log("★★★ResiltUI Open");
        var winner = "";
        if(OthelloGameManager.BlackStoneCount == OthelloGameManager.WhiteStoneCount)
        {
            winner = "Draw";
        }
        else if(OthelloGameManager.BlackStoneCount > OthelloGameManager.WhiteStoneCount)
        {
            winner = "Black Win";
        }
        else
        {
            winner = "White Win";
        }

        ResiltText.text = $"Resilt\n" +
            $"{winner}\n" +
            $"Black : {OthelloGameManager.BlackStoneCount}\n" +
            $"Whate : {OthelloGameManager.WhiteStoneCount}\n";

        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        Debug.Log("★★★ResiltUI Close");
    }
}
