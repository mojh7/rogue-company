using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public enum LASER_DIRECTION
{
     Top,Down,Right,Left
};

public class LaserBeam : MonoBehaviour {
	
    [Tooltip("Laser instantiate position")]
	public Transform rayInstantiatePos; // laser instantiate postion 

    [Tooltip("Laser Hit Layer")]
    public LayerMask enemyHitLayer; // which layer laser should hit 

    [Tooltip("Allocate the The Laser Size")]
    [Range(2,50f)]
	public float raySize = 10f; // casting ray size

    [Range(1,50f)]
    public float laserSize = 5f; // laser ray size 

    [Range(1,100f)]
    public float laserGlowSize = 4f; // laser glow size, adjust with lasersize
    
    [Tooltip("Laser Damage amount for enemy")]
	public int laserDamage; // laser damage amout for enemy 

    [Tooltip("Laser Firing Duration")]
    public float rayDuration; // laser firing duration

    [Space(15)]

    /*------------------------------------------------------------------- */
    [Tooltip("Emitter when laser collide with Enemy/EnemyHitLayer")]
    public GameObject laserHitEmitter; // emitter when laser collide with Enemy/EnemyHitLayer

    [Tooltip("Emitter when laser start firing")]
    public GameObject laserMeltEmitter;  // emitter when laser start firing 

    [Tooltip("The Laser Glow Sprite")]
    public Transform laserGlow;  // laser glow transform 

    //[HideInInspector]
    public bool laserOn = false; // switching variable 
    public LASER_DIRECTION LaserDir = LASER_DIRECTION.Top;
    /*----------------------------------------------------------------- */
	private LineRenderer lineRenderer; // line renderer 
	private Animator theAnimator; // to animate laser beginning animation 
    private float length = 0; // length of linerender 
    private float lerpTime = 1f; // used to lerp 
    private float currentLerpTime = 0; // used to lerp
    private GameObject hitParticle = null;  // you can use object pooler here 
    private GameObject meltParticle = null; // you can use object pooler here 
    private Vector2 endPos; // end position of the laser 
    private L2DEnemyHealth theEnemy; // used to cache EnemyHealth component 
    private bool canFire = true; // can fire laser beam or not 
    private Vector2 laserDir = Vector2.up; // initial direction set to bottom to up
    /*----------------------------------------------------------------- */


    void Start(){

        laserGlow.gameObject.SetActive(false); // initially laser glow is set to off
		lineRenderer = GetComponent <LineRenderer> (); // cache linerrenderer component
		lineRenderer.enabled = false; // at the beginning the linerenderer should disable 
		lineRenderer.sortingOrder = 5; // the laser should be visible top of the enemy layer

		theAnimator = GetComponent <Animator> (); // cache the animator 

        // set laser direction 
        setLaserDirection();
    }

    /// <summary>
    ///  get call from animation event 
    /// </summary>
    void actvieLaser(){
		laserOn = true;
        Invoke("deactiveLaser", rayDuration);
	}

    ///<summary>
    /// used to set laser direction 
    ///</summary>
    public void setLaserDirection(){
        if(LaserDir == LASER_DIRECTION.Top) {
             laserDir = Vector2.up;
             laserGlow.localRotation = Quaternion.Euler(new Vector3(0,0,0.0f));
        }
        else if(LaserDir == LASER_DIRECTION.Down) {
             laserDir = Vector2.down; 
             laserGlow.localRotation = Quaternion.Euler(new Vector3(0,0,0.0f));
        }
        else if(LaserDir == LASER_DIRECTION.Right) {
             laserDir = Vector2.right;
             laserGlow.localRotation = Quaternion.Euler(new Vector3(0,0,-90.0f));
        }
        else if(LaserDir == LASER_DIRECTION.Left) {
             laserDir = Vector2.left;
             laserGlow.localRotation = Quaternion.Euler(new Vector3(0,0,270.0f)); 
         }
    }


	void LateUpdate() {

        // laser Firing
        FireLaser();

        // laser is on 
        if (laserOn) {

            // end position of the layer just make sure its out of game screen
            endPos = new Vector2(rayInstantiatePos.position.x, transform.position.y * laserDir.y + raySize); 

            /*----------------------------------------------------------------- */
             //Debug.DrawRay(rayBeginPos.position, laserDir, Color.black);    
            //Debug.Log(laserDir);
            /*----------------------------------------------------------------- */

            lineRenderer.enabled = true;
            // cast ray 
            RaycastHit2D hit = Physics2D.Raycast(rayInstantiatePos.position, laserDir, raySize, enemyHitLayer); 

            // laser melting particle 
            if (meltParticle == null) {
                meltParticle = Instantiate(laserMeltEmitter, rayInstantiatePos.position, Quaternion.identity) as GameObject;
                meltParticle.transform.parent = this.transform;
            }

            // laser hit a physics body 
            if (hit.collider != null) {

                lineRenderer.SetPosition(0, rayInstantiatePos.position);
                lineRenderer.SetPosition(1, hit.point);

                hit = laserColGlow(hit);

                // hit emitter 
                if (hitParticle == null) {
                    hitParticle = Instantiate(laserHitEmitter, hit.point, Quaternion.identity) as GameObject;
                }
                if (hitParticle != null) {
                    hitParticle.transform.position = hit.point;    
                }

                /*-------------------- Enemy Damage -------------------- */
                if (hit.collider.tag == "Enemy") {
                    hit.collider.gameObject.GetComponent<L2DEnemyHealth>().giveDamage(laserDamage);
                }
                /*------------------------------------------------------ */
            }
            // laser not hiting any physics body 
             else { 

                if (hitParticle != null) {
                    Destroy(hitParticle);
                }

                
                lineRenderer.SetPosition(0, rayInstantiatePos.position);
                       
                // Laser Not colliding Glowing Effect 
                laserNotColGlow();

                if(LaserDir == LASER_DIRECTION.Top || LaserDir == LASER_DIRECTION.Down){
                    
                    Vector2 ld = new Vector2( (float)Math.Round(lineRenderer.GetPosition(0).x, 2), (float)Math.Round(laserDir.y * laserSize, 2));

                    lineRenderer.SetPosition(1, ld);
                    //laserNotColGlow();
                }
                else{
                    Vector2 ld = new Vector2((float)Math.Round(laserDir.x * laserSize, 2), (float) Math.Round(lineRenderer.GetPosition(0).y,2));
                    lineRenderer.SetPosition(1, ld);
                }

            }
        }
            else if (laserOn == false) {

                if (hitParticle != null) {
                    Destroy(hitParticle);
                }

                if (meltParticle != null) {
                   Destroy(meltParticle);
                }

                // turn of laser ray smoothly (If you don't like smooth turning of just comment out the method)
                //turnOfLaser(); 
                 lineRenderer.enabled = false;
                 lineRenderer.SetPosition(0, Vector3.zero);
                 lineRenderer.SetPosition(1, Vector3.zero);
                
                // turn of laser glow 
                laserGlow.gameObject.SetActive(false);
            }

    } //end update

    

    ///<summary>
    /// use this method for starting Laser
    ///</summary>
    public void FireLaser() {
        // switching laser and player power
        if (Input.GetMouseButtonDown(1) && canFire == true) {
            theAnimator.SetBool("startLaser", true);
            canFire = false;
            Invoke("enableFiring", rayDuration + 2f); // duration + plus 2 seconds for starting animations 
        }
    }

    /// <summary>
    /// deactiavte laser after certain time
    /// </summary>
    void deactiveLaser() {
        theAnimator.SetBool("startLaser", false);
        laserOn = false;
    }

    /// <summary>
    /// this glow effect when laser did not collide with any Enemy Object
    /// </summary>
    private void laserNotColGlow() {
        
        Vector2 p0 = lineRenderer.GetPosition(0);
        Vector2 p1 = lineRenderer.GetPosition(1);
        float dist = Vector2.Distance(p0, p1);
        dist += laserGlowSize;

        if (!laserGlow.gameObject.activeInHierarchy) {
            laserGlow.gameObject.SetActive(true);
        }

        if(LaserDir == LASER_DIRECTION.Top || LaserDir == LASER_DIRECTION.Down){
            laserGlow.localScale = new Vector2(laserGlow.localScale.x, dist * laserDir.y);
        }
        else{
            laserGlow.localScale = new Vector2(laserGlow.localScale.x, dist * laserDir.x);;
        }
        
    } // end 


    /// <summary>
    /// this glow effect when laser collide with any Enemy Object
    /// </summary>
    private RaycastHit2D laserColGlow(RaycastHit2D hit) {
        if (!laserGlow.gameObject.activeInHierarchy) {
            laserGlow.gameObject.SetActive(true);
        }
        if(LaserDir == LASER_DIRECTION.Right || LaserDir == LASER_DIRECTION.Left){
            laserGlow.localScale = new Vector2(laserGlow.localScale.x, hit.distance * 4.0f * laserDir.x);
        }
        else{
            laserGlow.localScale = new Vector2(laserGlow.localScale.x, hit.distance * 4.0f * laserDir.y);
        }
        return hit;
    }

    void enableFiring() {
        canFire = true;
    }

    /// <summary>
    /// this method used to lerp Vector
    /// </summary>
    /// <returns>
    /// value between 0 to 1
    /// </returns>
    private float _lerpLaser() {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime) {
            currentLerpTime = lerpTime;
        }

        float perc = currentLerpTime / lerpTime;
        return perc;
    } // end 


    /// <summary>
    /// turn of laser smoothly 
    /// </summary>
    void  turnOfLaser() {

        Vector2 startPos = lineRenderer.GetPosition(0);
        Vector2 endPos = lineRenderer.GetPosition(1);

        length = (endPos - startPos).magnitude;

        float perc = _lerpLaser();

        Vector2 lerp =  Vector2.Lerp(endPos, startPos, perc);

        if (length > 0.5f) {
            lineRenderer.SetPosition(1, lerp);
        }
        else {
           lineRenderer.enabled = false;
            currentLerpTime = 0;
        }

    } // end

    
}
