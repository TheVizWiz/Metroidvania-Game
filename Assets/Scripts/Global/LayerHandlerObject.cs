using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "Layer Handler Object",menuName = "Scriptable Objects/General/Layer Handler")]
public class LayerHandlerObject : ScriptableObject {

    [Header("LayerMasks")] 
    public LayerMask divable;
    public LayerMask strikable, slashable, carryable, enemy;

    private void OnEnable() {
        GameManager.Constants.DIVABLE_LAYERMASK = divable;
        GameManager.Constants.STRIKABLE_LAYERMASK = strikable;
        GameManager.Constants.SLASHABLE_LAYERMASK = slashable;
        GameManager.Constants.ENEMY_LAYERMASK = enemy;
        GameManager.Constants.CARRYABLE_LAYERMASK = carryable;
    }
}