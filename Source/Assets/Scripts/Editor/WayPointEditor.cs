using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlyingEnemyController))]
public class WayPointEditor : Editor
{
    private void OnSceneGUI()
    {
        FlyingEnemyController feController = (FlyingEnemyController)target;
        if (feController.wayPoints.Length > 1)
        {
            if (!EditorApplication.isPlaying)
            {
                feController.wayPoints[0] = feController.transform.position;
            }
            else
            {
                feController.wayPoints[0] = Handles.PositionHandle(feController.wayPoints[0], Quaternion.identity);
            }
            for (int i = 1; i< feController.wayPoints.Length; i++)
            {
                Handles.DrawAAPolyLine(new Vector3[] { feController.wayPoints[i-1], feController.wayPoints[i] });
                feController.wayPoints[i] = Handles.PositionHandle(feController.wayPoints[i], Quaternion.identity);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(feController);
            if (feController.loop)
            {
                Handles.DrawAAPolyLine(new Vector3[]{ feController.wayPoints[0], feController.wayPoints[feController.wayPoints.Length - 1] });
            }
        }
    }
}
