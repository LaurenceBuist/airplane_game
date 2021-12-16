using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// track the path of the bullet. Only in UnityEditor
/// </summary>
public class BulletDebugger : MonoBehaviour {

    private Transform me;
    private List<Vector3> Poses = new List<Vector3>();

    //color of the path
    public Color LineColor;
    //amount of frames being stored
    public int maxFrames;

    void Update()
    {
#if UNITY_EDITOR
        for (int i = 1; i < Poses.Count; i++)
        {
            Debug.DrawLine(Poses[i-1], Poses[i], LineColor);
        }
        if (Poses.Count > maxFrames)
        {
            Poses.RemoveRange(0, Poses.Count - maxFrames);
        }
#endif
    }

    public void AddPos(Vector3 pos)
    {
#if UNITY_EDITOR
        Poses.Add(pos);
#endif
    }
    
    public void Clear()
    {
#if UNITY_EDITOR
        Poses.Clear();
#endif
    }
}
