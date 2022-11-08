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
            
            if (this.gameObject.tag == "Target")
            {
                if (archeryManager.isLevel2 == false)
                {
                    archeryManager.playerChance -= 1;
                }
                else
                {
                    archeryManager.level2Targets -= 1;
                }
                Instantiate(checkMark, this.gameObject.transform.position, Quaternion.identity);
            }
            else
            {
                if (archeryManager.isLevel2 == true)
                {
                   archeryManager.playerChance -= 1;
                   if(archeryManager.playerChance <=0)
                   {
                    archeryManager.setToZero = true;
                   } 
                }
                else
                {
                    archeryManager.playerChance -= 1;
                    archeryManager.setToZero = true;
                }
                Instantiate(wrongMark, this.gameObject.transform.position, Quaternion.identity);  
            }
            
            Destroy(gameObject);
        }
    }
}
