using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcai_Kanon : NPCAIBase
{
    public override string Title() => "Kanon";
    public override string Info() => "隅っこ大好き型、箱や隅に入りたがる猫の習性から、四隅や端にばかり石を置きたがる。";
    Vector3 _targetPos = Vector3.zero;

    ///NPCAIのつくり方
    ///実装前半編「スクリプトを用意して、StartUIに自分のNPCAIを表示しよう」
    ///１．Npcai_test.csを複製して、npcai_(AIタイトル)
    ///２．Title() => "NPC";を変更 ※Inspecter上や対戦相手一覧に表示される
    ///３．StartUI.csに追加を行う「TODO：NPC選択用UIに表示されるようにしよう！」
    ///４．StartUI.csに追加を行う「TODO：NPCにコントローラーがアタッチされるようにしよう！」
    ///実装後半編「NPCAIを作りこんでいこう」
    ///１．「TODO：投げる先を決めよう！」の仕様を決めてコードを書く
    ///２．「TODO：投げよう！」の仕様を決めてコードを書く
    ///３．投擲距離には限界があるので「TODO：投げるために移動しよう！」の仕様を決めてコードを書く

     protected override void Select()
    {
        Debug.Log ("★★★Select_test");
        base.Select();
        List<Transform> TargetList = OthelloGameManager.TargetList; //置ける場所
        //おける場所の位置座標を取得
        //四隅に近い所を優先する(原点から一番離れるほど良い)

        // 下記を使うこと【★ルール：自分から３マス範囲内に投擲】
        List<Transform> footingStones = GetFootingStones(null, new List<Transform>());
        foreach(var footingStoneT in footingStones){
             List<Transform> targetList = footingStoneT.GetComponent<StoneAndTarget>().GetTargetTransformListByRange(3);
        }
        
        //TODO：投げる先を決めよう！
        _throwTarget = TargetList[0];
        Debug.Log ("★★★"+TargetList[0].transform.name);
        Debug.Log ("★★★投げる先決定！："+_throwTarget.name);
    }

    protected override void Move()
    {
        Debug.Log ("★★★Move_test");
        base.Move();
        //TODO：投げるために移動しよう！
    }

    protected override void Throw()
    {
        Debug.Log ("★★★Throw_test");
        base.Throw();
        //TODO：投げよう！
        //Throwで石を投げます。
        float angle = 45;
        ThrowStone.Throw(ThrowStartTarget.position, _throwTarget.position, angle);
    }
}
