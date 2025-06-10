using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    const float WaitForSeconds = 0.05f;
    float _tick;
    Coroutine _moveCoroutine = null;
    
    /// <summary>
    /// sTraからeTraまでtime秒で向かう
    /// </summary>
    public void MoveStart(RootPoint rootPoint,float time)
    {
        Transform sTra = rootPoint.transform;
        Transform eTra = rootPoint.transform;
        //次のルートがセットされている場合は移動先にセットする
        if(rootPoint.NextPoint != null)
            eTra = rootPoint.NextPoint.transform;
        //開始している場合は一旦止める
        Stop();
        _tick = 0;
        _moveCoroutine = StartCoroutine(Move(sTra,eTra,time));
    }

    private IEnumerator Move(Transform sTra,Transform eTra,float time)
    {
        while (true)
        {
            transform.position = Vector3.Lerp(sTra.position, eTra.position, _tick);
            transform.localRotation = Quaternion.Lerp(sTra.localRotation, eTra.localRotation, _tick);
            yield return new WaitForSeconds(WaitForSeconds);
            _tick += WaitForSeconds;
            if (_tick >= time)
                break;
        }
    }

    public void Stop()
    {
        if(_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
    }
}
