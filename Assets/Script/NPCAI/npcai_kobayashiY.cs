using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcai_kobayashiY : MonoBehaviour
{
    [SerializeField]
    private Transform OthelloStone; //石
    [SerializeField]
    private Transform ThrowStartTarget; //石を投げる開始地点
    [SerializeField]
    private List<Transform> ThrowTargetList; //石を投げる先の優先順


    public void Select(){
        Debug.Log ("★★★Select");
        //おける場所の一覧確認
        List<StoneAndTarget> list = null;
        List<int> impactCountList = new List<int>();
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
        //最大で取得できる石の数から順番にリストを作成
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
    public void Throw(){
        Debug.Log ("★★★Throw");
        Debug.Log ("★★★投げる先決定！　優先度１："+ThrowTargetList[0].name);
        var target = ThrowTargetList[0];
        //開始地点に石を移動させる
        OthelloStone.position = ThrowStartTarget.position;
        //距離によって投げる力を決める
        float angle = 45;
        Vector3 velocity = CalculateVelocity(ThrowStartTarget.position, target.position, angle);

        //放物線になるように投擲する
        Rigidbody stoneRb = OthelloStone.GetComponent<Rigidbody>();
        stoneRb.velocity = Vector3.zero;
        stoneRb.useGravity= true;
        OthelloStone.LookAt(target.transform);
        stoneRb.AddForce(velocity * stoneRb.mass, ForceMode.Impulse);
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    /// </summary>
    /// <param name="pointA">射出開始座標</param>
    /// <param name="pointB">標的の座標</param>
    /// <returns>射出速度</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

}
