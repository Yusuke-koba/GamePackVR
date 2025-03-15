using System.Collections.Generic;
using UnityEngine;

public class npcai_yoshikaneT_2 : NPCAIBase
{
    public override string Title() => "yoshikaAvoidAroundCornerAndEdgeNPC";
    private Vector3 targetPos = Vector3.zero;

    ///NPCAIのつくり方
    ///１．npcai_test.csを複製して、npcai_(AIタイトル)
    ///２．Title() => "NPC";を変更 ※Inspecter上や対戦相手一覧に表示される
    ///３．「TODO：投げる先を決めよう！」の仕様を決めてコードを書く
    ///４．「TODO：投げる角度を決めよう！」の仕様を決めてコードを書く
    ///５．投擲距離には限界があるので「TODO：投げるために移動しよう！」の仕様を決めてコードを書く

    protected override void Select()
    {
        Debug.Log("★★★Select_test");
        base.Select();
        var targetList = OthelloGameManager.TargetList; //置ける場所
        if (targetList.Count == 0)
        {
            Debug.Log("★★★投げる先がないのでパスします。");
            return;
        }

        // aroundList
        var scoreList = new List<(int score, StoneAndTarget item)>();
        foreach (var target in targetList)
        {
            var targetStoneAndTarget = target.GetComponent<StoneAndTarget>();
            var aroundList = targetStoneAndTarget.GetAroundStoneAndTargetList();
            var score = targetStoneAndTarget.GetAroundStoneAndTargetCount() * 100;  // 角・エッジ優先
            foreach (var around in aroundList)
            {
                score += around.GetAroundStoneAndTargetCount(); // 角・エッジが周りにある場合点を低く
            }
            scoreList.Add((score, targetStoneAndTarget));
        }
        scoreList.Sort((a, b) => b.score - a.score);
        var maxScore = scoreList[0].score;
        var maxScoreList = scoreList.FindAll((score) => score.score == maxScore);
        var randomIndex = Random.Range(0, maxScoreList.Count);
        targetPos = maxScoreList[randomIndex].item.transform.position;

        Debug.Log("★★★" + scoreList[0].item.transform.name);
        Debug.Log("★★★投げる先決定！：" + targetPos);
    }

    protected override void Move()
    {
        Debug.Log("★★★Move_test");
        base.Move();
        //TODO：投げるために移動しよう！
    }

    protected override void Throw()
    {
        Debug.Log("★★★Throw_test");
        base.Throw();
        //TODO：投げる角度を決めよう！
        float angle = 45;
        //石を投げます。
        ThrowStone.Throw(ThrowStartTarget.position, targetPos, angle);
    }
}
