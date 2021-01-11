using UnityEngine;

[AddComponentMenu("Temporary Handlers/Position Handler")]
public class TempPositionHandler : ITempPropHandler {

    private new Transform transform;
    private string prefString;
    private void Start() {
        base.Start();
        transform = GetComponent<Transform>();
        prefString = entityPrefString + "pos";
        base.CheckStart();

    }
    public override void StartCondition() {
        transform.position = new Vector3(PlayerPrefs.GetFloat(prefString + "x"), 
            PlayerPrefs.GetFloat(prefString + "y"), PlayerPrefs.GetFloat(prefString + "z"));
    }

    public override void LeaveSceneCondition() {
        Vector3 pos = transform.position;
        PlayerPrefs.SetFloat(prefString + "x", pos.x);
        PlayerPrefs.SetFloat(prefString + "y", pos.y);
        PlayerPrefs.SetFloat(prefString + "z", pos.z);
    }

    public override void DestroyCondition() {
        
    }
}