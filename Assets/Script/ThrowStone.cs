using UnityEngine;
using UnityEngine.Events;

public class ThrowStone : MonoBehaviour
{
    [SerializeField]
    private StoneAndTarget.Type type = StoneAndTarget.Type.None;
    [SerializeField]
    private Transform _stone;
    [SerializeField]
    private bool _throw = false;//投げる際はtrueにする
    public UnityAction ChangeTurnEvent { get; set; }
    public void ChangeTurn(bool isBlackTurn)
    {
        if (isBlackTurn)
        {
            type = StoneAndTarget.Type.BlackStone;
            _stone.localRotation = Quaternion.Euler(new Vector3(StoneAndTarget.BLACK_STONE_X, 0, 0));
        }
        else
        {
            type = StoneAndTarget.Type.WhiteStone;
            _stone.localRotation = Quaternion.Euler(new Vector3(StoneAndTarget.WHITE_STONE_X, 0, 0));
        }
    }

    /// <summary>
    /// 石の制約
    /// ・４マスぐらいの距離を飛ぶと急に墜落する　※最大飛距離
    /// </summary>
    /// <param name="startPos">開始地点</param>
    /// <param name="targetPos">狙い先</param>
    /// <param name="angle">角度</param>
    public void Throw(Vector3 startPos, Vector3 targetPos, float angle){
        //開始地点に石を移動させる
        transform.position = startPos;
        //石の判定をONにする
        _throw = true;
        Vector3 velocity = CalculateVelocity(startPos, targetPos, angle);//target.position, angle);
        //放物線になるように投擲する
        Rigidbody stoneRb = gameObject.GetComponent<Rigidbody>();
        stoneRb.velocity = Vector3.zero;
        stoneRb.useGravity= true;
        //transform.LookAt(target.transform);
        stoneRb.AddForce(velocity * stoneRb.mass, ForceMode.Impulse);
        //飛んだ距離を測る
            //飛んだ距離が一定を超えたら落とす
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(!_throw)
            return;
        if (collider.transform.name.Equals("Target"))
        {
            Debug.Log ("★★★OnTriggerEnter");
            var target = collider.transform.parent.GetComponent<StoneAndTarget>();
            var list = target.GetImpactList(type);
            if (list == null || list.Count == 0)
                return;
            Rigidbody stoneRb = gameObject.GetComponent<Rigidbody>();
            stoneRb.velocity = Vector3.zero;
            stoneRb.transform.position = stoneRb.transform.position + Vector3.up;
            stoneRb.AddForce(Vector3.up * 10, ForceMode.Impulse);
            //ぶつかることができた石が他に当たらないようにする
            _throw = false;
            //接触した石を変更
            target.Change(type);
            //接触した石から波及して変更
            foreach (var listItem in list)
            {
                listItem.Change(type);
            }
            ChangeTurnEvent.Invoke();
        }
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
