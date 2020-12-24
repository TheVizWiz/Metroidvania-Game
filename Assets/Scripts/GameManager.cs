using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager{
    
    public static GameObject player;
    public static SceneAnimator sceneAnimator;
    public static LevelManager levelManager;
    public static CameraController cameraController;
    public static DialogueManager dialogueManager;

    public static LayerMask enemyLayerMask;
    public static LayerMask strikableLayerMask;
    public static LayerMask slashableLayerMask;

    public static int saveNumber;
    public static bool isInitialized = false;

    private static int oldIndex;
    private static bool isLoading;

    public LayerMask enemyMask;
    public LayerMask strikableMask;
    public LayerMask slashableMask;

    public static IEnumerator LoadScene(int newIndex) {
        if (isLoading) yield break;
        else isLoading = true;

        float time = 0;
        oldIndex = levelManager.areaIndex;
        sceneAnimator.ExitScene();
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(newIndex);
        operation.allowSceneActivation = false;


        while (operation.progress < 0.9 && time < 0.5) {
            time += Time.deltaTime;
            yield return null;
        }
        
        // Debug.Log("finished load");
        operation.allowSceneActivation = true;

    }
    
    public static void EnterScene(LevelManager levelManager) {
        GameManager.levelManager = levelManager;
        foreach (EnterExitArea area in levelManager.areas) {
            if (area.nextSceneIndex == oldIndex) {
                levelManager.StartCoroutine(area.Enter());
                break;
            }
        }
        sceneAnimator.EnterScene();
        isLoading = false;
    }

    public static string GetPath() {
        return Path.Combine(Application.persistentDataPath, "Save" + saveNumber);
    }
    
    
}
