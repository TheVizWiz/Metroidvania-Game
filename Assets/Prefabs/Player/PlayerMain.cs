using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMain : MonoBehaviour {

    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    
    public float health;
    public float mana;

    private AudioListener audioListener;

    private void Awake() {
        audioListener = GetComponent<AudioListener>();
        if (GameManager.player == null) GameManager.player = this.gameObject;
        else Destroy(this.gameObject);
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public bool UseMana() { 
        return true;
    }
    
    public void AddMana(float amount) {
        
    }

    public bool Damage(int damageMultiplier) {
        
        return false;
    }

    public bool Damage() {
        return false;
    }

    public void AddHealth(float amount) {
        
    }
}
