using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TempPropHandlerMain))]
public abstract class ITempPropHandler : MonoBehaviour {

    protected TempPropHandlerMain main;
    protected string entityPrefString;

    
    // Start is called before the first frame update
    protected void Start() {
        main = GetComponent<TempPropHandlerMain>();
        entityPrefString = "sv" + GameManager.saveNumber + "bi" + SceneManager.GetActiveScene().buildIndex + "" +
                     main.handlerString;
    }

    protected void CheckStart() {
        if (PlayerPrefs.HasKey(entityPrefString)) {
            StartCondition();
        }
    }

    private void OnDestroy() {
        PlayerPrefs.SetString(entityPrefString, "");
        if (GameManager.isLoading) {
            LeaveSceneCondition();
        } else {
            DestroyCondition();
        }
    }

    public abstract void StartCondition();

    public abstract void LeaveSceneCondition();

    public abstract void DestroyCondition();


}
