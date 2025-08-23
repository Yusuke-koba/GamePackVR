using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAIBase : MonoBehaviour
{
    public virtual string Title() => "NPCAIBase";
    public virtual string Info() => "NPCAIBase";

    protected ThrowStone ThrowStone; //石
    protected Transform ThrowStartTarget; //石を投げる開始地点
    protected Transform _throwTarget; //石を投げる先
    // [SerializeField]
    // protected LayerMask _stoneLayerMask;

    public void Setup()
    {
        ThrowStone = GameObject.Find("ThrowStone").GetComponent<ThrowStone>();
        ThrowStartTarget = transform.Find("ThrowStartTarget");
    }

    //メインから呼ばれる
    public virtual void TurnStart()
    {
        Check();
        Select();
        Move();
        Throw();
    }

    /// <summary>
    /// ターン開始時、足元をチェックして自分の石でない場合、適当なところに飛ぶ
    /// </summary>
    protected virtual void Check()
    {
        Debug.Log("★★★Check");
        Transform footingStone = GetFootingStone();
        if (footingStone != null && this.GetComponent<OthelloPlayer>().StoneType == footingStone.GetComponent<StoneAndTarget>().StoneType)
        {
            //自分の石の上に立っている
            Debug.Log("★★★Check：自分の石の上に立っている");
        }
        else
        {
            //自分の石以外の所に立っているので強制移動
            Debug.Log("★★★Check：自分の石以外の所に立っているので強制移動");
            List<Transform> stones = new List<Transform>();
            if (this.GetComponent<OthelloPlayer>().StoneType == StoneAndTarget.Type.BlackStone)
                stones = OthelloGameManager.BlackStoneT;
            else
                stones = OthelloGameManager.WhiteStoneT;
            this.transform.position = new Vector3(stones[0].position.x, this.transform.position.y, stones[0].position.z);
            Debug.Log("★★★Check：自分の石以外の所に立っているので強制移動 →" + stones[0].name);
        }
    }

    /// <summary>
    /// 投げる場所を選ぶロジックを書く
    /// </summary>
    protected virtual void Select()
    {
        Debug.Log("★★★BaseSelect");
    }

    /// <summary>
    /// 選んだ所まで移動するロジックを書く
    /// </summary>
    protected virtual void Move()
    {
        Debug.Log("★★★BaseMove");
    }

    /// <summary>
    /// 移動したところから投げるロジックを書く
    /// </summary>
    protected virtual void Throw()
    {
        Debug.Log("★★★BaseThrow");
        // ThrowStone.Throw(ThrowStartTarget.position, _throwTarget.position, 45);
    }

    protected void GetTargetList(ref List<int> impactCountList)
    {
        List<StoneAndTarget> list = null;
        impactCountList = new List<int>();
        foreach (var t in OthelloGameManager.TargetList)
        {
            Debug.Log("★★★" + t.name);
            list = t.GetComponent<StoneAndTarget>().GetImpactList(this.GetComponent<OthelloPlayer>().StoneType);
            var count = 0;
            foreach (var u in list)
            {
                Debug.Log("★★★奪える：" + u.name);
                count++;
            }
            impactCountList.Add(count);
            Debug.Log("★★★奪える数：" + count);
        }
    }

    /// <summary>
    /// 足元の石のStoneAndTargetを返す
    /// </summary>
    protected Transform GetFootingStone()
    {
        Ray ray = new Ray(transform.position, transform.up * -1);
        RaycastHit hit; //レイが衝突したオブジェクト
        if (!Physics.Raycast(ray, out hit, 2f))
            return null;
        if (hit.collider.name.Equals("Stone"))
            return hit.collider.transform.parent;
        return null;
    }

    /// <summary>
    /// 移動できる石のリストを取得
    /// 　・移動は隣が自分の石の場合移動できる
    /// </summary>
    protected List<Transform> GetFootingStones(Transform stoneAndTarget, List<Transform> list)
    {
        //　今いる場所から「移動できる足場」の確認
        //足場を設定していない場合、自身から下にレイを飛ばし足場を設定する
        if (stoneAndTarget == null)
        {
            // 足元にレイを飛ばす
            Ray ray = new Ray(transform.position, transform.up * -1);
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
        foreach (var aroundStone in aroundStones)
        {
            if (aroundStone == null || aroundStone.StoneType != this.GetComponent<OthelloPlayer>().StoneType)
                continue;
            aroundStoneT = aroundStone.transform;
            if (!list.Contains(aroundStoneT))
            {
                list.Add(aroundStoneT);
                list = GetFootingStones(aroundStoneT, list);
            }
        }
        return list;
    }
}
