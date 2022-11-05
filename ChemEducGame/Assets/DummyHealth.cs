using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHealth : MonoBehaviour
{
    // Start is called before the first frame update

    public int dummyHealth = 200;
    public void TakeDamage(int damage)
    {
        dummyHealth -= damage;
    }

    private void Update() {
        if (dummyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
