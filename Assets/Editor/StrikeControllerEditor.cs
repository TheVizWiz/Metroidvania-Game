using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StrikeController))]
public class StrikeControllerEditor : Editor {
    private static float radius;
    private static Vector3 center;
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        StrikeController controller = (StrikeController) target;
        SerializedObject serializedObject = new SerializedObject(controller);

        Vector3 movement = controller.GetTransform().position;
        Vector2 center2D = serializedObject.FindProperty("center").vector2Value;
        int upgradeLevel = serializedObject.FindProperty("upgradeLevel").intValue;
        
        if (upgradeLevel >= 6) {
            radius = serializedObject.FindProperty("level6Range").floatValue;
        } else if (upgradeLevel >= 3) {
            radius = serializedObject.FindProperty("level3Range").floatValue;
        } else {
            radius = serializedObject.FindProperty("level1Range").floatValue;
        }

        center = new Vector3(center2D.x + movement.x, center2D.y + movement.y, movement.z);




    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmos(StrikeController scr, GizmoType gizmoType)
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, radius);
    }
    
    
}