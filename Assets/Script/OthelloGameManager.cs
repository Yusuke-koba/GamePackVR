using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class OthelloGameManager : MonoBehaviour
{
    private const float INTERVAL_TURN_CHANGE = 3.0f; //秒
    [SerializeField]
    private Transform _pcCamera;
    [SerializeField]
    private Transform _stones;
    [SerializeField]
    private ThrowStone _throwStone;
    [SerializeField]
    private WindowMonitor _windowMonitor;
    [SerializeField]
    private TextMeshPro _monitorText;
    [SerializeField]
    private bool _isTurnBlack;
    [SerializeField]
    private Timer _timerUI;
    [SerializeField]
    private Transform _player1;
    [SerializeField]
    private Transform _player2;
    [SerializeField]
    private int _turn = 0; //経過ターン数
    private float _intervalTurnChange = 100000;
    private int _noTargetPlayerCount = 0;

    //置けるTarget石のリスト(GO:StoneAndTarget)
    public static List<Transform> TargetList { get; private set; } = new List<Transform>();
    //残りの数
    public static int RemainingCount { get; private set; } = 0;
    //白の数
    public static int WhiteStoneCount { get => WhiteStoneT.Count;}
    public static List<Transform> WhiteStoneT{get; private set;} = new List<Transform>();
    //黒の数
    public static int BlackStoneCount { get => BlackStoneT.Count;}
    public static List<Transform> BlackStoneT{get; private set;} = new List<Transform>();

    public delegate void CallbackGameOver();
    public static event CallbackGameOver onGameOver;

    public Transform Stones { get => _stones; }
    //開始
    public void GameStart()
    {
        AllSetAroundStoneAndTargetList();
        _turn = 0;
        //_isTurnBlack = false; //黒先手で始める
        _isTurnBlack = GetRandBool(); //ランダム
        foreach (Transform t in _stones)
        {
            t.GetComponent<StoneAndTarget>().Init();
        }
        ChangeTurn();
        _throwStone.ChangeTurnEvent = () => ChangeTurn();
        _timerUI.ChangeTurnEvent = () => ChangeTurn();
        _player1.GetComponent<NPCAIBase>().Setup();
        _player2.GetComponent<NPCAIBase>().Setup();

        //INTERVAL_TURN_CHANGE秒経過すると開始する ※スタート演出内でスタート演出分の時間をとる
        _intervalTurnChange = INTERVAL_TURN_CHANGE;

        //スタート演出をしてから開始する
        StartCoroutine(StartPerformanceEvent());
    }

    void Update(){
        StoneAndTarget.Type turnType = _isTurnBlack ? StoneAndTarget.Type.BlackStone : StoneAndTarget.Type.WhiteStone;
        _intervalTurnChange -= Time.deltaTime;
        if(_intervalTurnChange <= 0)
        {
            if(_player1.GetComponent<OthelloPlayer>().StoneType == turnType)
            {
                Debug.Log ("★★★P1");
                _player1.GetComponent<NPCAIBase>().TurnStart();
                _monitorText.text = _player1.GetComponent<NPCAIBase>().Title();
            }
            else
            {
                Debug.Log ("★★★P2");
                _player2.GetComponent<NPCAIBase>().TurnStart();
                _monitorText.text = _player2.GetComponent<NPCAIBase>().Title();
            }
            _intervalTurnChange = INTERVAL_TURN_CHANGE;
        }
    }

    //ターン変更
    public void ChangeTurn()
    {
        _isTurnBlack = !_isTurnBlack;

        //ボード
        SetBoardCount();

        //ゲームオーバー
        if (RemainingCount == 0)
        {
            GameOver();
            return;
        }

        //ターゲット作成
        CreateTarget();

        //ターン変更手続き
        _throwStone.ChangeTurn(_isTurnBlack);
        _timerUI.Restart(_isTurnBlack);
        _turn++;

        // ターン開始時の情報を記録
        GameLogSave(); 

        // 置ける場所がない場合ターン終了
        // ２回連続だった（２人とも置けなかった）場合Gameover
        if (TargetList.Count == 0)
        {
            Debug.Log("★★★NoTarget Count="+_noTargetPlayerCount);
            _noTargetPlayerCount++;
            if(_noTargetPlayerCount == 2)
                GameOver();
            else
                ChangeTurn();
        }
        else
            _noTargetPlayerCount = 0;
    }

    private void SetBoardCount(string boardLog = "")
    {
        RemainingCount = 0;
        BlackStoneT = new List<Transform>();
        WhiteStoneT = new List<Transform>();

        foreach (Transform t in _stones)
        {
            var stone = t.GetComponent<StoneAndTarget>();
            if (stone.StoneType == StoneAndTarget.Type.Target)
            {
                stone.Change(StoneAndTarget.Type.None);
                RemainingCount++;
            }
            else if (stone.StoneType == StoneAndTarget.Type.None)
                RemainingCount++;
            else if (stone.StoneType == StoneAndTarget.Type.BlackStone)
                BlackStoneT.Add(t);
            else if (stone.StoneType == StoneAndTarget.Type.WhiteStone)
                WhiteStoneT.Add(t);
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
                continue;

            //何も置いてない場所から置けるか判定
            if (_isTurnBlack)
                list = stone.GetImpactList(StoneAndTarget.Type.BlackStone);
            else
                list = stone.GetImpactList(StoneAndTarget.Type.WhiteStone);

            if (list == null)
                continue;

            if (list.Count > 0)
            {
                //置けるところを追加
                stone.Change(StoneAndTarget.Type.Target);
                TargetList.Add(t);
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("★★★GameOver");
        _intervalTurnChange = 100000;
        onGameOver?.Invoke();
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

    [Serializable]
    public class GameLogData
    {
        public int turn = 0;                        // 経過ターン数
        public bool IsTurnBlack = false;            // ターン
        public Vector3 CharacterPos1 = Vector3.zero;// キャラクター位置
        public Vector3 CharacterPos2 = Vector3.zero;// キャラクター位置
        public string BoardLog = "";                // 盤面の状況
    }

    string _path =null;
    [SerializeField]
    private int _loadLine = 0;
    public void GameLogSave()
    {
        try
        {
            GameLogData gameLogData = new GameLogData();
            gameLogData.turn = _turn;
            gameLogData.IsTurnBlack = _isTurnBlack;
            gameLogData.CharacterPos1 = _player1.position;
            gameLogData.CharacterPos2 = _player2.position;
            StringBuilder sb = new StringBuilder();
            foreach (Transform t in _stones)
            {
                sb.Append((int)t.GetComponent<StoneAndTarget>().StoneType);
            }
            gameLogData.BoardLog = sb.ToString();
            Debug.Log ("SAVE★★★:::"+JsonUtility.ToJson(gameLogData));
            _path = Path.Combine(new string[2] { Application.persistentDataPath, "GameLogFile.log" });
            Debug.Log ("SAVEPath★★★:::"+_path);
            File.AppendAllText(_path, JsonUtility.ToJson(gameLogData)+"|"+Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.Log("★★★" + e.ToString());
        }
    }

    public void GameLogLoad()
    {
        try
        {
            string readText = File.ReadAllText(_path);
            if (readText.Length <= 0)
                return;
            string[] readTexts = readText.Split('|');
            GameLogData gameLogData = new GameLogData();
            gameLogData = JsonUtility.FromJson<GameLogData>(readTexts[_loadLine -1]);

            //読み込み
            _turn = gameLogData.turn;
            _isTurnBlack = gameLogData.IsTurnBlack;
            _player1.position = gameLogData.CharacterPos1;
            _player2.position = gameLogData.CharacterPos2;
            int i = 0; 
            foreach (Transform t in _stones)
            {
                //Debug.Log(gameLogData.BoardLog[i]);
                t.GetComponent<StoneAndTarget>().Change((StoneAndTarget.Type)Int32.Parse(gameLogData.BoardLog[i].ToString()));
                i++;
            }
            //セットアップ
            SetBoardCount();
            CreateTarget();
            _throwStone.ChangeTurn(_isTurnBlack);
            _timerUI.Restart(_isTurnBlack);
        }
        catch (Exception e)
        {
            Debug.Log("★★★Error:GameLogLoad:" + e.Message);
        }
    }

    private bool GetRandBool()
    {
        if (UnityEngine.Random.Range(0,2) == 0) //※出力に２は含まれない
            return true;
        return false;
    }

    private IEnumerator StartPerformanceEvent()
    {
        //設定したスタート演出時間が経過すると開始
        _intervalTurnChange = 6;
        _timerUI.Stop();

        //PCのカメラをモニタが見えるところへ移動、後に元に戻す
        var keepPCCameraPos = _pcCamera.position;
        var keepPCCameraRot = _pcCamera.localRotation;
        Camera pcCamera = _pcCamera.gameObject.GetComponent<Camera>();
        var keepPCCameraOrthographicSize  = pcCamera.orthographicSize;
        var bestPosTarget = _windowMonitor.transform.Find("PCCameraBestPosition");
        pcCamera.orthographicSize = 3;
        _pcCamera.transform.position = bestPosTarget.position;
        _pcCamera.transform.localRotation = bestPosTarget.localRotation;

        //P1登場＋カメラ演出をモニタに投影
        _windowMonitor.CameraStart(0);
        _monitorText.text = _player1.GetComponent<NPCAIBase>().Title();
        yield return new WaitForSeconds(3);
        //P2登場＋カメラ演出をモニタに投影
        _windowMonitor.CameraStart(1);
        _monitorText.text = _player2.GetComponent<NPCAIBase>().Title();
        yield return new WaitForSeconds(3);


        //演出終了、ゲーム開始=========
        //モニターのカメラを真上からのところに持っていく
        _windowMonitor.CameraStart(3);
        //PCカメラをもとに戻す　※一旦戻さない
        // pcCamera.orthographicSize = keepPCCameraOrthographicSize;
        // _pcCamera.transform.position = keepPCCameraPos;
        // _pcCamera.transform.localRotation = keepPCCameraRot;
        _timerUI.Restart(_isTurnBlack);
    }
}
