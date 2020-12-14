using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor {

    private static Vector2 center;
    
    public override void OnInspectorGUI() {

        base.OnInspectorGUI();
        //
        //
        // SerializedObject serializedObject = new SerializedObject(target);
        // LevelManager manager = (LevelManager) serializedObject.targetObject;
        // center = manager.playerSpawn;


    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    public static void DrawGizmos(LevelManager manager, GizmoType type) {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(new Vector3(manager.playerSpawn.x, manager.playerSpawn.y, 0), 0.15f);

    }

}