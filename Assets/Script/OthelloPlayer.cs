using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloPlayer : MonoBehaviour
{
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private StoneAndTarget.Type _type = StoneAndTarget.Type.None;
    public StoneAndTarget.Type StoneType { get => _type; set => _type = value; }

    void Start()
    {
        // 自身の色をセット
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log ("★★★"+collision.collider.name);
        if(collision.collider.name == "Stone"){
            // 足元の石が自身と同じ色の場合、何もしない
            // 足元の石が自身と違う色の場合、適当な場所にリスポーン
            var footholdStoneType = collision.collider.transform.parent.GetComponent<StoneAndTarget>().StoneType;
            if(footholdStoneType != StoneType){
                Debug.Log ("★★★適当な場所にリスポーン");
                //リストの上から一番最初に見つかった自身の色の石を取得
                foreach(var targetStone in OthelloGameManager.TargetList){
                    Debug.Log ("★★★"+targetStone.name+"::"+targetStone.GetComponent<StoneAndTarget>().StoneType);
                    if(targetStone.GetComponent<StoneAndTarget>().StoneType == StoneType){
                        Debug.Log ("★★★ここへ移動");
                        // OthelloPlayer.transform.position = targetStone.StoneAndTarget.position;
                        break;
                    }
                }
                //そこに移動
            }
        }
    }
}
