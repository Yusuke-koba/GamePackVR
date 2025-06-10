using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WindowMonitor : MonoBehaviour
{
    [Serializable]
    struct RootPointStruct
    {
        public RootPoint rootPoint;
        public float time;
    }

    [SerializeField]
    private CameraMove CameraMove;

    [SerializeField, Header("開始地点のRootPointとNextRootPointまでの秒数")]
    private List<RootPointStruct> StartRootPoint = new List<RootPointStruct>();

    public void CameraStart(int rootID)
    {
        try
        {
            CameraMove.MoveStart(StartRootPoint[rootID].rootPoint, StartRootPoint[rootID].time);
        }
        catch (Exception e)
        {
            Debug.LogError("★★★CameraStart："+e.ToString());
            throw;
        }
    }
}
