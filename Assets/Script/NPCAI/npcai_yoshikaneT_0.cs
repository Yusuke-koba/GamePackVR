using System.Collections.Generic;
using UnityEngine;

public class npcai_yoshikaneT_0 : NPCAIBase
{
    public override string Title() => "yoshikaRandomNPC";
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

        // ランダムに選択
        var randomIndex = Random.Range(0, targetList.Count);
        targetPos = targetList[randomIndex].transform.position;
        Debug.Log("★★★" + targetList[randomIndex].transform.name);
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
