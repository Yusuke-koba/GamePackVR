using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class Ranking : MonoBehaviour
{
    private const int RankItemHeight = 140;
    [Serializable]
    public class RankItemInfo
    {
        public int rank = 0;       //ランク
        public string name = "";   //名前
        public int winP = 0;       //勝ち数
        public int loseP = 0;      //負け数
        public int score = 0;      //得点
    }

    [SerializeField]
    private GameObject RankContent;
    [SerializeField]
    private GameObject Rank_1;
    [SerializeField]
    private GameObject Rank_2;
    [SerializeField]
    private GameObject Rank_3;
    [SerializeField]
    private GameObject Rank_Current;
    [SerializeField]
    private GameObject SameAIUI;

    // void Start(){
    //     //TEST
    //     // Setup("NPC_WIN",10000,"NPC_LOSERRRR",2000);
    //     // Setup("NPC_WIN",20000,"NPC_WINDRAW",20000);
    //     // Setup("NPC_WIN",20000,"NPC_WIN",2000);
    //     // Open();
    // }

    public void GameSetOpen(string winNPCName,int winScore,string loseNPCName,int loseScore)
    {
        gameObject.SetActive(true);
        string path = Path.Combine(new string[2] { Application.persistentDataPath, "GameRankingLogFile.log" });
        Debug.Log ("Path★★★:::"+path);
        Debug.Log ($"★★★winNPCName={winNPCName},winScore={winScore},loseNPCName={loseNPCName},loseScore={loseScore}");
        List<RankItemInfo> rankItemInfos = Load(path);

        //集計
        RankCalk(ref rankItemInfos, winNPCName, winScore, loseNPCName, loseScore);

        //順位付け、Win数Asc
        rankItemInfos.Sort((a, b) => b.winP - a.winP);
        int rank = 0;
        int previousWinP = -1;
        foreach(var T in rankItemInfos)
        {
            //同じ勝ち数は同順位
            if(T.winP != previousWinP)
                rank++;
            T.rank = rank;
            previousWinP = T.winP;
        }

        //UI_Rankingにセット
        SetUIRanking(rankItemInfos);
        
        Save(path,rankItemInfos);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        string path = Path.Combine(new string[2] { Application.persistentDataPath, "GameRankingLogFile.log" });
        Debug.Log ("Path★★★:::"+path);
        List<RankItemInfo> rankItemInfos = Load(path);
        SetUIRanking(rankItemInfos);
    }

    private void RankCalk(ref List<RankItemInfo> rankItemInfos, string winNPCName,int winScore,string loseNPCName,int loseScore)
    {
        //同じAI同士の戦いはランキングに反映無し
        if(winNPCName.Equals(loseNPCName))
        {
            StartCoroutine(SameAIUIEvent());
            return;
        }

        bool isWinAdd = false;
        bool isLoseAdd = false;
        //ランク合算
        //引き分けの場合、勝ち数/負け数加算はしない
        if(rankItemInfos.Count > 1)
        {
            //同じNPCの場合は加算、新規の場合は追加
            foreach(RankItemInfo T in rankItemInfos)
            {
                if(winNPCName.Equals(T.name))
                {
                    if(winScore != loseScore)
                        T.winP += 1;
                    T.score += winScore;
                    isWinAdd = true;
                }
                if(loseNPCName.Equals(T.name))
                {
                    if(winScore != loseScore)
                        T.loseP += 1;
                    T.score += loseScore;
                    isLoseAdd = true;
                }
            }
        }

        //ランク追加
        //引き分けの場合、勝ち数/負け数加算はしない
        RankItemInfo rankItemInfo = new RankItemInfo();
        if(!isWinAdd)
        {
            rankItemInfo = new RankItemInfo();
            rankItemInfo.name = winNPCName;
            if(winScore != loseScore)
                rankItemInfo.winP = 1;
            rankItemInfo.score = winScore;
            rankItemInfos.Add(rankItemInfo);
        }
        
        if(!isLoseAdd)
        {
            rankItemInfo = new RankItemInfo();
            rankItemInfo.name = loseNPCName;
            if(winScore != loseScore)
                rankItemInfo.loseP = 1;
            rankItemInfo.score = loseScore;
            rankItemInfos.Add(rankItemInfo);
        }
    }

    private void Save(string path,List<RankItemInfo> rankItemInfos)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            foreach (RankItemInfo rankItemInfo in rankItemInfos)
                sb.Append(JsonUtility.ToJson(rankItemInfo)).Append("|");
            Debug.Log ("SAVE★★★:::"+sb.ToString());
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(sb.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("★★★" + e.ToString());
        }
    }

    private List<RankItemInfo> Load(string path)
    {
        try
        {
            List<RankItemInfo> rankItemInfos = new List<RankItemInfo>();
            using (StreamReader sr = new StreamReader(path))
            {
                string readText = sr.ReadToEnd();
                if (readText.Length <= 0)
                    return new List<RankItemInfo>();
                string[] readTexts = readText.Split('|');
                for(int i = 0; i < readTexts.Length -1; i++)
                    rankItemInfos.Add(JsonUtility.FromJson<RankItemInfo>(readTexts[i]));
            }
            return rankItemInfos;
        }
        catch (Exception e)
        {
            Debug.LogError("★★★" + e.ToString());
            return new List<RankItemInfo>();
        }
    }

    private void SetUIRanking(List<RankItemInfo> rankItemInfos)
    {
        RankItemInfoSet(Rank_1,rankItemInfos[0]);
        if(rankItemInfos.Count >= 2)
            RankItemInfoSet(Rank_2,rankItemInfos[1]);
        if(rankItemInfos.Count >= 3)
            RankItemInfoSet(Rank_3,rankItemInfos[2]);
        if(rankItemInfos.Count >= 4)
        {
            //ランキングのアイテム追加
            for(int i = 3; i < rankItemInfos.Count -1; i++)
                CreateRankItem(rankItemInfos[i],RankContent);

            //ランキング表示外の最後をスクロール外で表示する
            if(rankItemInfos.Count >= 8)
            {
                Rank_Current.SetActive(true);
                RankItemInfoSet(Rank_Current,rankItemInfos[rankItemInfos.Count-1]);
            }
            else{
                Rank_Current.SetActive(false);
                CreateRankItem(rankItemInfos[rankItemInfos.Count-1],RankContent);
            }
        }

        var height = RankContent.transform.childCount * RankItemHeight + 20;
        RankContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    }

    private void RankItemInfoSet(GameObject go, RankItemInfo rankItemInfo)
    {
        Transform rankTra = go.transform.Find("Rank");
        if(rankTra != null)
            rankTra.GetComponent<TextMeshProUGUI>().text = rankItemInfo.rank.ToString();
        go.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = rankItemInfo.name;
        go.transform.Find("WinP").GetComponent<TextMeshProUGUI>().text = rankItemInfo.winP.ToString();
        go.transform.Find("LoseP").GetComponent<TextMeshProUGUI>().text = rankItemInfo.loseP.ToString();
        go.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = rankItemInfo.score.ToString();
    }

    private GameObject CreateRankItem(RankItemInfo rankItemInfo, GameObject rankContent)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/SCI-FI UI Pack Pro/RankItem");
        prefab = (GameObject)Instantiate(prefab, rankContent.transform.position, Quaternion.identity);
        prefab.transform.SetParent(rankContent.transform);
        prefab.transform.localScale = new Vector3(1, 1, 1);
        RankItemInfoSet(prefab, rankItemInfo);
        return prefab;
    }

    private IEnumerator SameAIUIEvent()
    {
        SameAIUI.SetActive(true);
        yield return new WaitForSeconds(10);
        SameAIUI.SetActive(false);
    }
}
