using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    private const float TIME = 60; //ターン内の時間制限

    [SerializeField]
    private TextMeshProUGUI TimerText;
    [SerializeField]
    private TextMeshPro SubTextMeshPro;
    public UnityAction ChangeTurnEvent { get; set; }
    private float _countdownSeconds = 60;
    private bool _isStop = true;

    public void Start() => OthelloGameManager.onGameOver += Stop;
    public void Stop() => _isStop = true;

    public void Restart(bool isBlackTurn)
    {
        _countdownSeconds = TIME;
        _isStop = false;
        gameObject.SetActive(true);
        if(SubTextMeshPro != null)
            SubTextMeshPro.gameObject.SetActive(true);
        if (isBlackTurn)
        {
            TimerText.color = Color.black;
            TimerText.outlineColor = Color.white;
        }
        else
        {
            TimerText.color = Color.white;
            TimerText.outlineColor = Color.black;
        }
    }


    private void Update()
    {
        if (_isStop){
            gameObject.SetActive(false);
            SubTextMeshPro.gameObject.SetActive(false);
            return;
        }
        _countdownSeconds -= Time.deltaTime;
        var span = new TimeSpan(0, 0, (int)_countdownSeconds);
        TimerText.text = span.ToString(@"mm\:ss");
        if(SubTextMeshPro != null)
            SubTextMeshPro.text = span.ToString(@"ss");
        if (_countdownSeconds <= 0)
        {
            ChangeTurnEvent.Invoke();
        }
    }
}
