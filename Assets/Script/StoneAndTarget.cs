using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAndTarget : MonoBehaviour
{
    public const int WHITE_STONE_X = 90;
    public const int BLACK_STONE_X = 270;

    [SerializeField]
    private bool _isStartStone = false;
    [SerializeField]
    private Type _type = Type.None;
    [SerializeField]
    private Transform _thisTransform;
    [SerializeField]
    private GameObject _stone;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private BoxCollider _foothold; // ★OthelloPlayerに一任)自身の色の石の場合は足場兼リスポーンエリア、自身の色の石でない場合はキルゾーン
    [SerializeField]
    private GameObject _impactEffect;
    [SerializeField]
    private BoxCollider[] _checkColliders;
    [SerializeField]
    private StoneAndTarget[] _aroundStoneAndTargetList = new StoneAndTarget[10]; //まわりの石の情報
    [SerializeField]
    private LayerMask _stoneLayerMask;
    
    public Transform thisTransform {get => _thisTransform; private set => _thisTransform = value; }
    public Type StoneType { get => _type; set => _type = value; }

    public enum Type
    {
        None = 0,
        WhiteStone = 1,
        BlackStone = 2,
        Target = 3
    }

    public void Init()
    {
        if (!_isStartStone)
        {
            //最初に表示しておく石以外を非表示にする
            Change(Type.None);
        }
        thisTransform = transform;
    }

    public StoneAndTarget[] GetAroundStoneAndTargetList(){
        return _aroundStoneAndTargetList;
    }

    public void Change(Type type)
    {
        _type = type;
        switch (type)
        {
            case Type.WhiteStone:
                _stone.SetActive(true);
                _target.SetActive(false);
                _stone.transform.localRotation = Quaternion.Euler(new Vector3(WHITE_STONE_X, 0, 0));
                StartCoroutine(ImpactEvent());
                break;
            case Type.BlackStone:
                _stone.SetActive(true);
                _target.SetActive(false);
                _stone.transform.localRotation = Quaternion.Euler(new Vector3(BLACK_STONE_X, 0, 0));
                StartCoroutine(ImpactEvent());
                break;
            case Type.Target:
                _stone.SetActive(false);
                _target.SetActive(true);
                break;
            case Type.None:
                _stone.SetActive(false);
                _target.SetActive(false);
                break;
        }
    }

    public List<StoneAndTarget> GetImpactList(Type inpactType)
    {
        if(StoneType != inpactType)
        {
            var addlist = new List<StoneAndTarget>();
            var changeList = new List<StoneAndTarget>();
            for (int i = 0; i < _aroundStoneAndTargetList.Length; i++)
            {
                addlist = CheckChain(i, inpactType, new List<StoneAndTarget>());
                //Debug.Log("★★★addlist.Count=" + addlist.Count);
                foreach (var listItem in addlist)
                {
                    //Debug.Log("★★★listItem =" + listItem.name);
                    changeList.Add(listItem);
                }
            }
            //Debug.Log("★★★changeList.Count="+ changeList.Count);
            return changeList;
        }
        return null;
    }

    public List<StoneAndTarget> CheckChain(int impactNo, Type inpactType , List<StoneAndTarget> list)
    {
        var stone = _aroundStoneAndTargetList[impactNo];
        //獲得なし
        if (stone == null || stone.StoneType == Type.None || stone.StoneType == Type.Target)
            return new List<StoneAndTarget>();
        //Debug.Log("★★★stone =" + stone.name);
        //ここまで獲得
        if (stone.StoneType == inpactType)
            return list;
        //獲得対象
        list.Add(stone);
        list = stone.CheckChain(impactNo, inpactType, list);
        return list;
    }

    /// <summary>
    /// まわりの石を予め持っておく
    /// </summary>
    public void SetAroundStoneAndTargetList()
    {
        Collider[] hitColliders;
        int j = 0;
        _aroundStoneAndTargetList = new StoneAndTarget[10];
        for (int i = 0; i < _checkColliders.Length; i++)
        {
            if (_checkColliders[i] == null)
            {
                _aroundStoneAndTargetList[i] = null;
                continue;
            }
            hitColliders = Physics.OverlapBox(_checkColliders[i].transform.position, _checkColliders[i].transform.localScale / 2, Quaternion.identity, _stoneLayerMask);
            j = 0;
            while (j < hitColliders.Length)
            {
                //Debug.Log("Hit : " + hitColliders[j].name + ": j=" + j);
                if (hitColliders[j].name.Equals("Stone"))
                {
                    _aroundStoneAndTargetList[i] = hitColliders[j].transform.parent.GetComponent<StoneAndTarget>();
                    break;
                }
                j++;
            }
        }
    }

    /// <summary>
    /// 範囲指定で周囲のターゲット石を取得する
    /// range:マス
    /// </summary>
    public List<Transform> GetTargetTransformListByRange(int range)
    {
        List<Transform> result = new List<Transform>();
        List<StoneAndTarget> targetList = GetTargetListByRange(range);
        foreach(var t in targetList)
            result.Add(t.transform);
        return result;
    }
    /// <summary>
    /// 範囲指定で周囲のターゲット石を取得する
    /// range:マス
    /// </summary>
    public List<StoneAndTarget> GetTargetListByRange(int range)
    {
        if(range < -1) 
            range = 1;

        // MEMO：コリジョンのスケール＝7.5（１マス）
        // MEMO：コリジョンのスケール＝12.5（２マス）
        // MEMO：コリジョンのスケール＝17.5（３マス）
        float halfExtent = (2.5f + 5f * range)/2; //OverlapBoxは中心からの長さを指定

        List<StoneAndTarget> targetList = new List<StoneAndTarget>();
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(halfExtent,1,halfExtent), Quaternion.identity, _stoneLayerMask);
        int j = 0;
        while (j < hitColliders.Length){
            //ターゲットのGOを取得
            if (hitColliders[j].name.Equals("Target")){
                StoneAndTarget stoneAndTarget = hitColliders[j].transform.parent.GetComponent<StoneAndTarget>();
                if(!targetList.Contains(stoneAndTarget))
                    targetList.Add(stoneAndTarget);
            }
            j++;
        }
        return targetList;
    }

    /// <summary>
    /// エフェクトを再生する
    /// </summary>
    private IEnumerator ImpactEvent()
    {
        _impactEffect.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _impactEffect.SetActive(false);
    }
}
