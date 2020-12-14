using UnityEngine;

public class ElectroBall : Ball {
    // Start is called before the first frame update
    private Animator animator;
    public float charge;


    public new void Start() {
        base.Start();
        animator = GetComponent<Animator>();
    }
    public new void Update() {
        base.Update();
    }


    public void Use() {
        animator.SetTrigger("Use");
        GetComponent<Collider2D>().enabled = false;
        used = true;
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }
}