using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine;

[EditorTool("Point Connector", typeof(PointConnector))]
public class PointConnectorTool : EditorTool
{
    [SerializeField] private Texture2D _icon;

    private Transform _oldTransform;
    private Point[] _targetPoints;
    private Point[] _allPoints;

    public override GUIContent toolbarIcon => new()
    {
        image = _icon,
        text = "Point connector move",
        tooltip = "Point connector move"
    };

    public override void OnToolGUI(EditorWindow window)
    {
        var targetTransform = ((PointConnector)target).transform;

        if (targetTransform != _oldTransform)
        {
            var stage = PrefabStageUtility.GetPrefabStage(targetTransform.gameObject);

            _targetPoints = targetTransform.GetComponentsInChildren<Point>();

            if (stage != null)
                _allPoints = stage.prefabContentsRoot.GetComponentsInChildren<Point>().Except(_targetPoints).ToArray();
            else
                _allPoints = FindObjectsOfType<Point>().Except(_targetPoints).ToArray();

            _oldTransform = targetTransform;
        }

        EditorGUI.BeginChangeCheck();
        var newPosition = Handles.PositionHandle(targetTransform.position, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetTransform, "Move With Point Connector Tool");
            MoveWithConnecting(targetTransform, newPosition);
        }
    }

    private void MoveWithConnecting(Transform targetTransform, Vector3 newPosition)
    {
        var position = newPosition;
        var minDistance = float.PositiveInfinity;

        foreach (var point in _allPoints)
        {
            foreach (var ownPoint in _targetPoints)
            {
                if (point.Type != ownPoint.Type)
                    continue;

                var targetPosition = point.transform.position - (ownPoint.transform.position - targetTransform.position);
                var distance = Vector3.Distance(targetPosition, newPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    position = targetPosition;
                }
            }
        }

        targetTransform.position = minDistance < 0.5f ? position : newPosition;
    }
}