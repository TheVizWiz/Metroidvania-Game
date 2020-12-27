using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CanvasGroup canvasGroup;
    public RectTransform transform;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Vector2 hidePosition;
    [SerializeField] private Vector2 showPosition;
    [SerializeField] private bool fadeIn;
    [SerializeField] private bool moveIn;
    [SerializeField] private float time;
    [SerializeField] private DialogueManagerPosition currentPosition;

    [HideInInspector] public bool isMoving;
    void Start() {
        transform.position = hidePosition;
        currentPosition = DialogueManagerPosition.HiddenPosition;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            Show();
        } else if (Input.GetKeyDown(KeyCode.Keypad8)) {
            Hide();
        }
    }

    public void Show() {
        if (MovePosition(DialogueManagerPosition.ShownPosition)) {
            GameManager.playerMovement.isInUI = true;
        }
    }

    public void Hide() {
        if (MovePosition(DialogueManagerPosition.HiddenPosition)) {
            GameManager.playerMovement.isInUI = false;
        }
    }

    public bool MovePosition(DialogueManagerPosition position) {
        if (isMoving) return false;
        if (!fadeIn && !moveIn) return false;
        if (currentPosition == position) return false;
        if (!moveIn) {
            transform.position = showPosition;
        }
        isMoving = true;
        currentPosition = position;
        Vector2 endPosition = position == DialogueManagerPosition.HiddenPosition ? hidePosition : showPosition;
        Vector2 startPosition = transform.position;
        float fadeStart = position == DialogueManagerPosition.HiddenPosition ? 1 : 0;
        if (fadeIn) {
            LeanTween.value(transform.gameObject, f => {
                canvasGroup.alpha = f;
            }, fadeStart, 1 - fadeStart, time).setOnComplete(ResetMoveBool);
        }

        if (moveIn) {
            LeanTween.value(transform.gameObject, f => {
                transform.position = new Vector2(Mathf.SmoothStep(startPosition.x, endPosition.x, f), Mathf.SmoothStep(startPosition.y, endPosition.y, f));
            }, 0, 1, time).setOnComplete(ResetMoveBool);
        }

        return true;
    }

    private void ResetMoveBool() {
        isMoving = false;
    }


    public Vector2 HidePosition => hidePosition;
    public Vector2 ShowPosition => showPosition;
}


public enum DialogueManagerPosition {
    HiddenPosition,
    ShownPosition
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueManager))]
internal class DialogueManagerCustomEditor : Editor {
    private DialogueManager manager;

    private void OnEnable() {
        manager = (DialogueManager) target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        // manager.currentPosition = EditorGUILayout.Popup("Position: ", position, new[] {"Hidden Position", "Shown Position"});
        // manager.transform.position = position == 0 ? manager.HidePosition : manager.ShowPosition;
    }
}
#endif
