using UnityEngine;

[AddComponentMenu("Temporary Handlers/Destruction Handler")]
public class TempDestructionHandler : ITempPropHandler {
    
    private string prefString;
    
    //0 = alive; 1 = dead;
    private new void Start() {
        base.Start();
        prefString = entityPrefString + "destr";
        CheckStart();
    }
    public override void StartCondition() {
        if (PlayerPrefs.GetInt(prefString) == 1) {
            Destroy(gameObject);
        }
    }

    public override void LeaveSceneCondition() {
        PlayerPrefs.SetInt(prefString, 0);
    }

    public override void DestroyCondition() {
        PlayerPrefs.SetInt(prefString, 1);
    }
}