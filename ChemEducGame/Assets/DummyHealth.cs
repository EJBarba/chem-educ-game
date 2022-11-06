using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHealth : MonoBehaviour
{
    // Start is called before the first frame update

    public int dummyHealth = 200;
    private SpriteRenderer spriteRenderer;
    public ArcheryManager archeryManager;
    [SerializeField] GameObject checkMark;
    [SerializeField] GameObject wrongMark;

    public Animator animator;

    private float r = 1f;
    private float g = 1f;
    private float b = 1f;
    private float a = 1f;

    private void Start() {
        spriteRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        archeryManager = GameObject.FindObjectOfType<ArcheryManager>();
        animator = this.gameObject.GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        dummyHealth -= damage;
        g -= 0.00625f;
        b -= 0.00625f;
        spriteRenderer.color = new Color(r, g, b, a);
    }

    private void Update() {
        if (dummyHealth <= 0)
        {
            archeryManager.hasDestroyed = true;
            archeryManager.playerChance -= 1;
            if (this.gameObject.tag == "Target")
            {
                Instantiate(checkMark, this.gameObject.transform.position, Quaternion.identity);
            }
            else
            {
                archeryManager.setToZero = true;
                Instantiate(wrongMark, this.gameObject.transform.position, Quaternion.identity);  
            }
            
            Destroy(gameObject);
        }
    }
}
