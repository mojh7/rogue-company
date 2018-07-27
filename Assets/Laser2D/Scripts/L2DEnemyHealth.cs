using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class L2DEnemyHealth : MonoBehaviour {

    public float Health = 50f;

    [Tooltip("Explosion Prefabs for Enemy")]
    public GameObject explosionFabs;


    private GameObject healthBar;

    void Start() {
        healthBar = transform.GetChild(0).gameObject;

    }


    public void giveDamage(float dmg) {

        float h = healthBar.transform.localScale.x;
        h -= (Time.deltaTime * dmg)/Health;

        if (h > 0) {
            healthBar.transform.localScale = new Vector2(h, healthBar.transform.localScale.y);
        }
        else {
            this.gameObject.SetActive(false);

            GameObject explode = Instantiate(explosionFabs, this.transform.position, Quaternion.identity) as GameObject;
            healthBar.transform.localScale = new Vector2(1f, healthBar.transform.localScale.y);          
        }
       
    }


}
