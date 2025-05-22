using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAIBase : MonoBehaviour
{
    public virtual string Title() => "";
    public virtual string Info() => "";

    [SerializeField]
    protected ThrowStone ThrowStone; //石
    [SerializeField]
    protected Transform ThrowStartTarget; //石を投げる開始地点
    [SerializeField]
    protected Transform _throwTarget; //石を投げる先
    [SerializeField]
    protected LayerMask _stoneLayerMask;

    //メインから呼ばれる
    public virtual void TurnStart(){
        Check();
        Select();
        Move();
        Throw();
    }

    /// <summary>
    /// ターン開始時、足元をチェックして自分の石でない場合、適当なところに飛ぶ
    /// </summary>
    protected virtual void Check(){
        Debug.Log ("★★★Check");
        Transform footingStone = GetFootingStone();
        if(footingStone != null && this.GetComponent<OthelloPlayer>().StoneType == footingStone.GetComponent<StoneAndTarget>().StoneType)
        {
            //自分の石の上に立っている
        }
        else
        {
            //自分の石以外の所に立っているので強制移動
            List<Transform> stones = new List<Transform>();
            if(this.GetComponent<OthelloPlayer>().StoneType == StoneAndTarget.Type.BlackStone)
                stones = OthelloGameManager.BlackStoneT;
            else
                stones = OthelloGameManager.WhiteStoneT;
            this.transform.position = new Vector3(stones[0].position.x,this.transform.position.y,stones[0].position.z);
        }
    }

    /// <summary>
    /// 投げる場所を選ぶロジックを書く
    /// </summary>
    protected virtual void Select(){
        Debug.Log ("★★★BaseSelect");
    }

    /// <summary>
    /// 選んだ所まで移動するロジックを書く
    /// </summary>
    protected virtual void Move(){
        Debug.Log ("★★★BaseMove");
    }

    /// <summary>
    /// 移動したところから投げるロジックを書く
    /// </summary>
    protected virtual void Throw(){
        Debug.Log ("★★★BaseThrow");
        ThrowStone.Throw(ThrowStartTarget.position, _throwTarget.position, 45);
    }

    protected void GetTargetList(ref List<int> impactCountList){
        List<StoneAndTarget> list = null;
        impactCountList = new List<int>();
        foreach(var t in OthelloGameManager.TargetList){
            Debug.Log ("★★★"+t.name);
            list = t.GetComponent<StoneAndTarget>().GetImpactList(this.GetComponent<OthelloPlayer>().StoneType);
            var count = 0;
            foreach(var u in list){
                Debug.Log ("★★★奪える："+u.name);
                count++;
            }
            impactCountList.Add(count);
            Debug.Log ("★★★奪える数："+count);
        }
    }

    /// <summary>
    /// 足元の石のStoneAndTargetを返す
    /// </summary>
    protected Transform GetFootingStone()
    {
        Ray ray = new Ray(transform.position, transform.up*-1);
        RaycastHit hit; //レイが衝突したオブジェクト
        if (!Physics.Raycast(ray, out hit, 2f, _stoneLayerMask))
            return null;
        if (hit.collider.name.Equals("Stone"))
            return hit.collider.transform.parent;
        return null;
    }
}
