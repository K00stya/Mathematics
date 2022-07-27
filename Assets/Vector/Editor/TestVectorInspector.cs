using UnityEditor;
using UnityEngine;

namespace Vector
{
[CustomEditor(typeof(TestVectors))]
public class TestVectorInspector : Editor
{
    private void OnSceneGUI()
    {
        TestVectors testVectors = target as TestVectors;
        DrawVector(testVectors, testVectors.A);
        DrawVector(testVectors, testVectors.B);
        
        //Matrix4x4
        if (testVectors.Sum)
        {
            Vector3 p0 = testVectors.transform.TransformPoint(
                testVectors.A.To.ToUnityVector3());
            Vector3 p1 = testVectors.transform.TransformPoint(
                testVectors.B.To.ToUnityVector3());
            Handles.color = Color.blue;
            Handles.DrawLine(p0, p1, testVectors.Thickness);
        }
    }

    private void DrawVector(TestVectors testVectors, VectorInfo vectorInfo)
    {
        Transform handleTransform = testVectors.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local
            ? handleTransform.rotation
            : Quaternion.identity;

        Vector3 p0 = handleTransform.TransformPoint(
            vectorInfo.From.ToUnityVector3());
        Vector3 p1 = handleTransform.TransformPoint(
            vectorInfo.To.ToUnityVector3());

        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(testVectors, "Move Point");
            EditorUtility.SetDirty(testVectors);
            vectorInfo.From.SetUnityVector3(handleTransform.InverseTransformPoint(p0));
        }

        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(testVectors, "Move Point");
            EditorUtility.SetDirty(testVectors);
            vectorInfo.To.SetUnityVector3(handleTransform.InverseTransformPoint(p1));
        }


        Handles.color = Color.white;
        Handles.DrawLine(p0, p1, testVectors.Thickness);

        if (vectorInfo.Normalize)
        {
            var vector = vectorInfo.To - vectorInfo.From;
            var normalize = vector.Normalize();
            Handles.color = Color.red;
            p1 = handleTransform.TransformPoint((vectorInfo.From + normalize).ToUnityVector3());
            Handles.DrawLine(p0, p1, testVectors.Thickness);
        }
    }
}
}
