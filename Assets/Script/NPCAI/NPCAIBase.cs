using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAIBase : MonoBehaviour
{
    public virtual string Title() => "";

    [SerializeField]
    protected ThrowStone ThrowStone; //石
    [SerializeField]
    protected Transform ThrowStartTarget; //石を投げる開始地点

    //メインから呼ばれる
    public virtual void TurnStart(){
        Select();
        Move();
        Throw();
    }

    protected virtual void Select(){
        Debug.Log ("★★★BaseSelect");
    }

    protected virtual void Move(){
        Debug.Log ("★★★BaseMove");
    }

    protected virtual void Throw(){
        Debug.Log ("★★★BaseThrow");
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
}
