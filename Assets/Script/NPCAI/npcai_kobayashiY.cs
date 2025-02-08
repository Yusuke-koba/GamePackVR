using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcai_kobayashiY : NPCAIBase
{
    public override string Title() => "kobaNPC";

    [SerializeField]
    public List<Transform> ThrowTargetList; //石を投げる先の優先順

    protected override void Select(){
        Debug.Log ("★★★Select");
        base.Select();
        //おける場所の一覧確認
        List<int> impactCountList = new List<int>();
        GetTargetList(ref impactCountList);

        //★沢山石を取得できるAIが強いと思うので、沢山おける順番にソートして上から優先度が高いとする
        //最大で取得できる石の数は相手の石の数
        int maxStone = 0;
        if(this.GetComponent<OthelloPlayer>().StoneType == StoneAndTarget.Type.WhiteStone){
            maxStone=OthelloGameManager.BlackStoneCount;
        }else{
            maxStone=OthelloGameManager.WhiteStoneCount;
        }

        List<Transform> sortTargetlist = new List<Transform>();
        int no = 0;
        //相手の石の数から多く取得できる順番にリストを作成
        for(int i = maxStone; i > 0; i--){
            Debug.Log ("★★★"+i);
            no = 0;
            foreach(var count in impactCountList){
                if(i==count){
                    sortTargetlist.Add(OthelloGameManager.TargetList[no]);
                    Debug.Log ("★★★　石："+OthelloGameManager.TargetList[no]+"　奪える：" + i);
                }
                no++;
            }
        }
        ThrowTargetList = sortTargetlist;
    }

    protected override void Move(){
        Debug.Log ("★★★Move_npcai_kobayashiY");
        base.Move();
        //石を投げる先の優先順から移動できる場所を選定
        //動ける足場の確認（ジャンプで移動想定：自分を中心に２マス目まで飛べる）
        //移動先を決定
        //ジャンプで移動
    }

    protected override void Throw(){
        Debug.Log ("★★★Throw");
        Debug.Log ("★★★投げる先決定！　優先度１："+ThrowTargetList[0].name);
        ThrowStone.Throw(ThrowStartTarget.position, ThrowTargetList[0].position, 45);
    }
}
