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
    private GameObject _stone;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private BoxCollider[] _checkColliders;
    [SerializeField]
    private StoneAndTarget[] _aroundStoneAndTargetList = new StoneAndTarget[10];
    [SerializeField]
    private LayerMask _stoneLayerMask;

    public Type StoneType { get => _type; set => _type = value; }

    public enum Type
    {
        WhiteStone,
        BlackStone,
        Target,
        None
    }

    public void Init()
    {
        if (!_isStartStone)
        {
            //最初に表示しておく石以外を非表示にする
            Change(Type.None);
        }
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
                break;
            case Type.BlackStone:
                _stone.SetActive(true);
                _target.SetActive(false);
                _stone.transform.localRotation = Quaternion.Euler(new Vector3(BLACK_STONE_X, 0, 0));
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
}
