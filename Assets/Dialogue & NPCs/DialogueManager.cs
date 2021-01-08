using System.Collections;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueManager : MonoBehaviour, IAnimatedUI {
    // Start is called before the first frame update
    [SerializeField] public CanvasGroup canvasGroup;
    public RectTransform transform;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private TextMeshProUGUI npcDescription;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject optionsPane;
    [SerializeField] private CanvasGroup optionsPaneGroup;
    [SerializeField] private GameObject optionPrefab;
    [SerializeField] private Vector2 hidePosition;
    [SerializeField] private Vector2 showPosition;
    [SerializeField] private bool fadeIn;
    [SerializeField] private bool moveIn;
    [SerializeField] private float time;
    [SerializeField] private float charactersPerSecond;
    [SerializeField] public DialogueManagerPosition currentPosition;

    private NPC npc;
    private Dialogue activeDialogue;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isAnimatingText;
    [HideInInspector] public UnityEvent hideEvent;
    private bool pressedInteract;
    private bool passedOption;
    private bool optionsAreShown;
    private bool waitToUpdatePlayerUI;
    private bool pauseUpdate;
    void Awake() {
        GameManager.dialogueManager = this;
        transform.anchoredPosition = hidePosition;
        currentPosition = DialogueManagerPosition.HiddenPosition;
        NPCManager.Initialize();
        SetNPC(NPCManager.npcs["Maya"]);
        if (fadeIn) {
            canvasGroup.alpha = 0;
        }

        Inventory.PickUp("mayastepstarter", 1);
    }

    // Update is called once per frame
    void Update() {

        if (waitToUpdatePlayerUI) {
            waitToUpdatePlayerUI = false;
            GameManager.playerMovement.isInUI = false;
        }
        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            Show();
        } else if (Input.GetKeyDown(KeyCode.Keypad8)) {
            Hide();
        }

        if (pauseUpdate) {
            pauseUpdate = false;
        } else if (Input.GetButtonDown("Interact") && 
                   currentPosition == DialogueManagerPosition.ShownPosition ) {
            
            
            pressedInteract = true;

            if (!passedOption) {
                if (activeDialogue != null && activeDialogue.HasNextLine() && !isAnimatingText) {
                    StartCoroutine(AnimateText(activeDialogue.GetNextLine(), 0));
                    pressedInteract = false;
                }
            } else {
                if ((activeDialogue == null) || (activeDialogue.IsLastLine() && !optionsAreShown && !isAnimatingText)) {
                    Hide();
                } else if (activeDialogue != null && activeDialogue.HasNextLine() && !isAnimatingText) {
                    StartCoroutine(AnimateText(activeDialogue.GetNextLine(), 0));
                    pressedInteract = false;
                }
            }

        } else if (Input.GetButtonUp("Interact")) {
            pressedInteract = false;
        }
        
        
    }

    public void Show() {
        activeDialogue = npc.ActivateDialogue();
        passedOption = false;
        optionsAreShown = false;
        optionsPaneGroup.alpha = 0;
        dialogueText.text = "";
        foreach (Transform transform in optionsPane.transform) {
            Destroy(transform.gameObject);
        }
        activeDialogue.ResetLines();
        if (MovePosition(DialogueManagerPosition.ShownPosition)) {
            GameManager.playerMovement.isInUI = true;
            if (activeDialogue != null && activeDialogue.HasNextLine() && !isAnimatingText) {
                StartCoroutine(AnimateText(activeDialogue.GetNextLine(), 0.5f));
            }
        }

    }
    public void Hide() {
        if (MovePosition(DialogueManagerPosition.HiddenPosition)) {
            waitToUpdatePlayerUI = true;
        }
        activeDialogue?.ResetLines();
        activeDialogue = null;
        hideEvent.Invoke();
    }

    private bool MovePosition(DialogueManagerPosition position) {
        if (isMoving) return false;
        if (!fadeIn && !moveIn) return false;
        if (currentPosition == position) return false;
        if (!moveIn) {
            transform.anchoredPosition = showPosition;
        }
        isMoving = true;
        currentPosition = position;
        Vector2 endPosition = position == DialogueManagerPosition.HiddenPosition ? hidePosition : showPosition;
        Vector2 startPosition = transform.anchoredPosition;
        float fadeStart = position == DialogueManagerPosition.HiddenPosition ? 1 : 0;
        if (fadeIn) {
            LeanTween.value(transform.gameObject, f => {
                canvasGroup.alpha = f;
            }, fadeStart, 1 - fadeStart, time).setOnComplete(ResetMoveBool);
        }

        if (moveIn) {
            LeanTween.value(transform.gameObject, f => {
                transform.anchoredPosition = new Vector2(Mathf.SmoothStep(startPosition.x, endPosition.x, f), Mathf.SmoothStep(startPosition.y, endPosition.y, f));
            }, 0, 1, time).setOnComplete(ResetMoveBool);
        }

        return true;
    }

    private void ResetMoveBool() {
        isMoving = false;
    }

    public bool SetNPC(NPC npc) {

        if (npc == null) {
            activeDialogue = null;
            return true;
        }
        this.npc = npc;
        activeDialogue = npc.ActivateDialogue();
        if (activeDialogue == null) return false;
        npcName.text = npc.name;
        npcDescription.text = npc.description;
        passedOption = false;
        dialogueText.text = "";
        return true;
    }

    private IEnumerator AnimateText(string text, float startWaitTime) {
        isAnimatingText = true;
        int totalCharacters = text.Length;
        
        yield return new WaitForSeconds(startWaitTime);
        for (int i = 0; i <= totalCharacters; i++) {
            if (pressedInteract && !pauseUpdate) {
                dialogueText.text = text;
                isAnimatingText = false;
                if (!passedOption && activeDialogue.IsLastLine()) {
                    ShowOptions();
                }
                yield break;
            } 
            dialogueText.text = text.Substring(0, i);
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
        
        if (!passedOption && activeDialogue.IsLastLine()) {
            ShowOptions();
        }
        isAnimatingText = false;
    }


    private void ChooseOption(TextMeshProUGUI textMeshProUgui) {
        if (!activeDialogue.EndDialogue(textMeshProUgui.text)) {
            HideOptions(false);
            return;
        }
        HideOptions(true);

    }

    private void ShowOptions() {
        if (activeDialogue.Options.Count == 0) {
            optionsAreShown = false;
            passedOption = true;
            return;
        }
        foreach (DialogueOption option in activeDialogue.Options) {
            GameObject optionObject = Instantiate(optionPrefab, optionsPane.transform, false);
            TextMeshProUGUI text = optionObject.GetComponentInChildren<TextMeshProUGUI>();
            // GameManager.eventSystem.firstSelectedGameObject = optionObject;
            Button button = optionObject.GetComponent<Button>();
            button.onClick.AddListener(delegate { ChooseOption(text); });
            button.Select();
            text.text = option.displayString;
        }

        optionsAreShown = true;
        passedOption = true;
        LeanTween.value(gameObject, f => {
            optionsPaneGroup.alpha = f;
        }, 0, 1, time);
    }

    private void HideOptions(bool flag) {
        optionsAreShown = false;
        foreach (Transform transform in optionsPane.transform) {
            Destroy(transform.gameObject);
        }
        if (flag) {
            StartCoroutine(AnimateText(activeDialogue.GetNextLine(), 0));
        } else {

            StartCoroutine(AnimateText(npc.autoDeclineMessage, 0));
            activeDialogue.ClearLines();
        }

        pauseUpdate = true;

    }
    public DialogueManagerPosition CurrentPosition {
        get => currentPosition;
        set => currentPosition = value;
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
        manager.transform.anchoredPosition = manager.CurrentPosition == 0 ? manager.HidePosition : manager.ShowPosition;
        manager.canvasGroup.alpha = manager.CurrentPosition == DialogueManagerPosition.HiddenPosition ? 0 : 1;
    }
}
#endif
