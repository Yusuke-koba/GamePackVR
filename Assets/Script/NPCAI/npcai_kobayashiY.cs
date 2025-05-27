using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npcai_kobayashiY : NPCAIBase
{
    public override string Title() => "kobaNPC";
    public override string Info() => "NPCの情報\n石が沢山取れる場所を狙う。移動は隣り合う足場";


    private Transform _moveTarget; //移動先

    /// <summary>
    /// 投げる場所を選ぶ
    /// 　・投げる優先度を決める
    /// 　　→石を沢山取得できる順番に優先度が高い
    /// 　・移動込みで投げれる場所に絞り込み、一番優先度の高い場所を選ぶ
    /// </summary>
    protected override void Select(){
        Debug.Log ("★★★Select");
        base.Select();
        //-----------------------------------
        // 投げる場所の優先度を決める
        //-----------------------------------
        
        //置ける場所の一覧取得
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
Debug.Log ("★★★maxStone="+maxStone);
        //相手の石の数から多く取得できる順番にリストを作成（優先順リスト）
        List<Transform> sortTargetlist = new List<Transform>();
        int no = 0;
        for(int i = maxStone; i > 0; i--){
            no = 0;
            foreach(var count in impactCountList){
                if(i==count){
                    sortTargetlist.Add(OthelloGameManager.TargetList[no]);
                    Debug.Log ("★★★　石："+OthelloGameManager.TargetList[no]+"　奪える：" + i);
                }
                no++;
            }
        }

        _throwTarget = sortTargetlist[0];

        //-----------------------------------
        // 移動込みで投げれる場所に絞り込み、一番優先度の高い投げれる場所を選ぶ
        // →移動は自分の色の地続きのみ
        //-----------------------------------

        //再帰呼び出し：取得したいものは、次の自分の石の周辺の自分の石かつ重複無しで辿れるものすべての石
        //→取得結果は移動できる石のリスト
        List<Transform> footingStones = GetFootingStones(null, new List<Transform>());

        //「移動できる石リスト」の中から、３マス先投擲の範囲内で「優先順リスト」が一番高い投げれる場所を特定する
        //「移動できる石リスト」を順番に確認する
        Debug.Log ("★★★footingStones===========");
        int keepNo = 10000;
        foreach(var footingStoneT in footingStones){
            Debug.Log ("★★★移動できる足場："+footingStoneT.name);
            // １．３マス範囲内のターゲット石リストを取得
            List<Transform> targetList = footingStoneT.GetComponent<StoneAndTarget>().GetTargetTransformListByRange(3);
            // ２．範囲内の石から「優先順リスト」の一番高い石を取得
            // ３．「移動できる石から投げる最優先場所」を保持している場合、優先度を比較して高い方を残す
            //     保持してない場合、保持する
            if(GetHighPriority(ref keepNo,ref _throwTarget, targetList, sortTargetlist)){
                _moveTarget = footingStoneT;
                Debug.Log ("★★★更新：番号="+keepNo+", 移動先="+_moveTarget.name+", 移動先投げ先="+_throwTarget.name);
             }

            if(keepNo == 0)
                break;
        }

        //移動する場所と投げる場所決定
        Debug.Log ("★★★STONE===========");
        Debug.Log ("★★★更新...決定：番号="+keepNo+", 移動先="+_moveTarget.name+", 移動先投げ先="+_throwTarget.name);
    }

    /// <summary>
    /// 移動できる石のリストを取得
    /// 　・移動は隣が自分の石の場合移動できる
    /// </summary>
    private List<Transform> GetFootingStones(Transform stoneAndTarget, List<Transform> list){
        //　今いる場所から「移動できる足場」の確認
        //足場を設定していない場合、自身から下にレイを飛ばし足場を設定する
        if(stoneAndTarget == null){
            // 足元にレイを飛ばす
            Ray ray = new Ray(transform.position, transform.up*-1);
            RaycastHit hit; //レイが衝突したオブジェクト
            if (!Physics.Raycast(ray, out hit, 2f))
                return list;
            if (!hit.collider.name.Equals("Stone"))
                return list;
            stoneAndTarget = hit.collider.transform.parent;
            list.Add(stoneAndTarget);
        }
        // 足元の石から周囲の石を取得
        StoneAndTarget[] aroundStones = stoneAndTarget.GetComponent<StoneAndTarget>().GetAroundStoneAndTargetList();
        // 周囲の石から自分の石をリストに追加、再帰呼び出しでその先も追加していく
        Transform aroundStoneT;
        foreach(var aroundStone in aroundStones){
            if(aroundStone == null || aroundStone.StoneType != this.GetComponent<OthelloPlayer>().StoneType)
                continue;
            aroundStoneT = aroundStone.transform;
            if(!list.Contains(aroundStoneT)){
                list.Add(aroundStoneT);
                list = GetFootingStones(aroundStoneT, list);
            }
        }
        return list;
    }

    /// <summary>
    /// 調査対象リストにあるStoneAndTargetから順位リストの中で一番高いStoneAndTargetを選ぶ
    /// return：true=見つかった、false=無かった
    /// </summary>
    private bool GetHighPriority(ref int keepNo,ref Transform target,List<Transform> targetList ,List<Transform> criteriaList){
        int no = 0; // criteriaListの番号をカウント（小さいほど優先度が高い）
        //Debug.Log ("★★★targetList.Length="+targetList.Count+":::criteriaList.Length="+criteriaList.Count);
        foreach(var t in criteriaList){
            foreach(var t2 in targetList){
                Debug.Log ("★★★"+t.name+":比較:"+t2.name);
                if(t == t2){
                    target = t2;
                    keepNo = no;
                    return true;
                }
            }
            no++;
        }
        return false;
    }

    /// <summary>
    /// 移動する
    /// 　・Selectで決めた移動先に移動する
    /// </summary>
    protected override void Move(){
        base.Move();
        // bool isMove = true;
        // while(isMove){
        //     //特定した石の上に到着したら移動修了
        //     Debug.Log ("★★★");
        //     isMove = false;
        // }
        //★★★強制移動
        this.transform.position = new Vector3(_moveTarget.position.x,this.transform.position.y,_moveTarget.position.z);
        Debug.Log ("★★★Move完了");
    }

    protected override void Throw(){
        base.Throw();
        Debug.Log ("★★★投げる先決定！："+_throwTarget.name);
        //Debug.Log ("★★★Throw//☆☆☆一旦投げない");
        //return; //☆☆☆一旦投げない
        //移動時点で、投げれる場所を限定しているので、百発百中
        ThrowStone.Throw(ThrowStartTarget.position, _throwTarget.position, 45);
    }
}
