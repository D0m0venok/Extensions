using System;
using UnityEngine;

public class Point : MonoBehaviour
{
    public enum PointType
    {
        Type1,
        Type2,
        Type3,
    }

    [SerializeField] private PointType _type;

    public PointType Type => _type;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = _type switch
        {
            PointType.Type1 => Color.red,
            PointType.Type2 => Color.green,
            PointType.Type3 => Color.blue,
            _ => throw new ArgumentOutOfRangeException()
        };
        Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
    }
}