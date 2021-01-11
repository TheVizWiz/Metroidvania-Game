using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SpawnAreaType {
    Left,
    Right,
    Top,
    Bottom,
    Ascendant
}

public class EnterExitArea : MonoBehaviour {

    private const float moveTime = 0.75f;
    private static readonly Vector2 verticalPos = new Vector2(0, 5);
    private static readonly Vector2 horizontalPosStart= new Vector2(5, 0);
    private static readonly Vector2 horizontalPosFinish = new Vector2(4, 0);
    private static readonly Vector2 horizontalVAddition = new Vector2(0, 0.75f);

    private new Transform transform;
    private new Collider2D collider;
    private PlayerMovement player;
    private LevelManager levelManager;
    private bool hasExited;
    
    public int nextSceneIndex;
    public SpawnAreaType spawnAreaType;

    public void Setup() {
        transform = GetComponent<Transform>();
        collider = GetComponent<Collider2D>();
        player = GameManager.player.GetComponent<PlayerMovement>();
        hasExited = false;
        collider.isTrigger = true;
        collider.enabled = true;
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            player.transform.position = Vector2.zero;
        }
    }

    public IEnumerator Enter() { 
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;
        Vector3 position = transform.position;
        Vector2 areaPos = new Vector2(position.x, position.y);
        player.SetMobility(false);

        collider.enabled = false;

        switch (spawnAreaType) {
            
            case SpawnAreaType.Left:
                startPos = areaPos - horizontalPosStart + horizontalVAddition;
                endPos = areaPos + horizontalPosFinish + horizontalVAddition;
                player.SetLookDirection(PlayerMovement.LOOK_DIRECTION_RIGHT);
                player.SetAnimationBool(PlayerMovement.walkString, true);
                break;
            
            case SpawnAreaType.Right:
                startPos = areaPos + horizontalPosStart + horizontalVAddition;
                endPos = areaPos - horizontalPosFinish + horizontalVAddition;
                player.SetLookDirection(PlayerMovement.LOOK_DIRECTION_LEFT);
                player.SetAnimationBool(PlayerMovement.walkString, true);
                break;
            
            case SpawnAreaType.Top:
                startPos = areaPos + verticalPos;
                endPos = areaPos;
                player.SetAnimationBool(PlayerMovement.fallString, true);
                break;
            
            case SpawnAreaType.Bottom:
                startPos = areaPos - verticalPos;
                endPos = areaPos;
                player.SetAnimationBool(PlayerMovement.jumpString, true);
                break;
            
            case SpawnAreaType.Ascendant:
                player.SetAnimationTrigger(PlayerMovement.ascendantString);
                break;
        }

        float timeLeft = 0;
        float conversionFactor = 1 / moveTime; 
        while (timeLeft <= moveTime) {
            timeLeft += Time.deltaTime * conversionFactor; 
            player.transform.position = Vector3.Lerp(startPos, endPos, timeLeft);
            yield return null;
        }

        collider.enabled = true;
        player.SetMobility(true);
    }

    public IEnumerator Exit() {
        
        switch (spawnAreaType) {
            case SpawnAreaType.Left:
                break;
            case SpawnAreaType.Right:
                break;
            case SpawnAreaType.Top:
                break;
            case SpawnAreaType.Bottom:
                break;
            case SpawnAreaType.Ascendant:
                break;
        }
        
        yield break;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(Constants.PLAYER_TAG)) {
            // print("collided");
            collider.enabled = false;
            StartCoroutine(GameManager.LoadScene(nextSceneIndex));
        }
    }

    public void OnCollisionExit2D(Collision2D other) {
    }
}
