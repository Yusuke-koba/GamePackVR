using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartUI : MonoBehaviour
{
    private const int ButtonHeightSelectUI = 200;
    public OthelloGameManager OthelloGameManager;
    [SerializeField]
    private Transform _playerA;
    [SerializeField]
    private Transform _playerB;
    [SerializeField]
    private Transform _playerASelectUI;
    [SerializeField]
    private Transform _playerBSelectUI;
    [SerializeField]
    private GameObject _playerASelectButton;
    [SerializeField]
    private GameObject _playerBSelectButton;
    private NPCAIType _playerANPCAIType = NPCAIType.None;
    private NPCAIType _playerBNPCAIType = NPCAIType.None;
    private NPCAIBase[] _npcAIList;

    public enum NPCAIType
    {
        None,
        Player,
        NPCAIBase,
        Npcai_kobayashiY,
    }

    void Start()
    {
        SetupNPCAI();
        //左側の選択は_PlayerA（人orNPC）
        CreateNPCAISelectButton(_playerASelectUI, true);
        //右側の選択は_playerB（NPC）
        CreateNPCAISelectButton(_playerBSelectUI, false);
    }

    private void SetupNPCAI()
    {
        var obj = new GameObject("NPCAIComponentList");
        obj.transform.parent = transform;
        _npcAIList = new NPCAIBase[2];
        _npcAIList[0]=(NPCAIBase)obj.AddComponent(typeof(NPCAIBase));
        _npcAIList[1]=(NPCAIBase)obj.AddComponent(typeof(Npcai_kobayashiY));
    }

    /// <summary>
    /// UIにボタンをセットする
    /// </summary>
    public void CreateNPCAISelectButton(Transform playerSelectUI,bool isPlayerA)
    {
        GameObject contentGo = playerSelectUI.Find("Viewport/Content").gameObject;
        if(isPlayerA)
            CreatePlayerNPCAISelectButton("Player",NPCAIType.None,contentGo,isPlayerA);
        CreatePlayerNPCAISelectButton(_npcAIList[1].Title(),NPCAIType.Npcai_kobayashiY,contentGo,isPlayerA);


        CreatePlayerNPCAISelectButton(_npcAIList[0].Title(),NPCAIType.NPCAIBase,contentGo,isPlayerA);
        contentGo.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contentGo.transform.childCount * ButtonHeightSelectUI);
    }

    /// <summary>
    /// OthelloPlayerのAIを選択する
    /// </summary>
    public void SetPlayerNPCAITypeButton(string title, NPCAIType playerNPCAIType, bool isPlayerA){
        Debug.Log ("★★★SetPlayerNPCAITypeButton押下：isPlayerA="+isPlayerA+",NPC="+playerNPCAIType);
        if(isPlayerA)
        {
            _playerANPCAIType = playerNPCAIType;
            _playerASelectButton.SetActive(true);
            _playerASelectUI.gameObject.SetActive(false);
            _playerASelectButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = title;
        }
        else
        {
            _playerBNPCAIType = playerNPCAIType;
            _playerBSelectButton.SetActive(true);
            _playerBSelectUI.gameObject.SetActive(false);
            _playerBSelectButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = title;
        }
    }

    /// <summary>
    /// ゲームを開始する
    /// </summary>
    public void GameStart()
    {
        if(_playerANPCAIType == NPCAIType.None || _playerBNPCAIType == NPCAIType.None )
            return;
        //プレイヤーやNPCAIをセットする
        SetPlayerNPCAI(_playerA,_playerANPCAIType);
        SetPlayerNPCAI(_playerB,_playerBNPCAIType);
        //ゲーム開始通知
        OthelloGameManager.GameStart();
        gameObject.SetActive(false);
    }

    //２体のキャラクターにプレイヤーかNPCAIのコントローラーをセットする
    private void SetPlayerNPCAI(Transform player,NPCAIType playerNPCAIType){
        switch (playerNPCAIType)
        {
            case NPCAIType.Player:
                //プレイヤー
                break;
            case NPCAIType.Npcai_kobayashiY:
                player.gameObject.AddComponent(typeof(Npcai_kobayashiY));
                break;
            case NPCAIType.NPCAIBase:
                player.gameObject.AddComponent(typeof(NPCAIBase));
                break;
        }
    }

    public void PlayerVSNPCMode(){

    }

    public void NPCVSNPCMode(){

    }

    private void SetNPCAI(){

    }

    /// <summary>
    /// ボタンの生成
    /// </summary>
    private GameObject CreatePlayerNPCAISelectButton(string name, NPCAIType aiType, GameObject targetPanel, bool isPlayerA)
    {
        GameObject buttonPrefab = (GameObject)Resources.Load("Prefabs/ButtonPlayerNPCAISelect");
        //追加
        buttonPrefab = (GameObject)Instantiate(buttonPrefab, targetPanel.transform.position, Quaternion.identity);
        buttonPrefab.transform.SetParent(targetPanel.transform);
        buttonPrefab.transform.localScale = new Vector3(1, 1, 1);
        buttonPrefab.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = name;
        Button button = buttonPrefab.GetComponent<Button>();
        button.onClick.AddListener(() => SetPlayerNPCAITypeButton(name, aiType, isPlayerA));
        return buttonPrefab;
    }
 
}
