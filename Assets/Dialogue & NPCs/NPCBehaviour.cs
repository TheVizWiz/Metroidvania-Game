using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour {

    public string name;
    [SerializeField] public Collider2D areaCollider;
    private NPC npc;

    private void Start() {
        npc = NPCManager.npcs[name];
    }

    public void RestartNPC() {
        if (npc.ActivateDialogue() != null) {
            GameManager.dialogueManager.SetNPC(npc);
            GameManager.playerMovement.canInteract = true;
        } else {
            GameManager.dialogueManager.SetNPC(null);
            GameManager.playerMovement.canInteract = false;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == GameManager.player.layer) {
            RestartNPC();
            GameManager.dialogueManager.hideEvent.AddListener(RestartNPC);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == GameManager.player.layer) {
            GameManager.playerMovement.canInteract = false;
            GameManager.dialogueManager.hideEvent.RemoveListener(RestartNPC);
        }
    }
    

}
