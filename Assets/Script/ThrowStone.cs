using UnityEngine;
using UnityEngine.Events;

public class ThrowStone : MonoBehaviour
{
    [SerializeField]
    private StoneAndTarget.Type type = StoneAndTarget.Type.None;
    [SerializeField]
    private Transform _stone;
    [SerializeField]
    private bool _ones = false;
    public bool Ones { get => _ones; set => _ones = value; }
    public UnityAction ChangeTurnEvent { get; set; }

    public void ChangeTurn(bool isBlackTurn)
    {
        Ones = false;
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

    private void OnTriggerEnter(Collider collider)
    {
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
            //1ターンで置けるのは1回
            Ones = true;
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
}
