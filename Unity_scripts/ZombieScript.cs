using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    CapsuleCollider myHitCollider;
    public GameObject prefabMiniGun, prefabFlameThrower;
    public GameObject myNeck;
    public LayerMask whatIsPlayer;
    public bool dead = false;
    public float moveSpeed = 1.0f;
    public GameObject theSolider;
    SoldierScript soldierScript;
    public Animator myAnim;
    public ParticleSystem prefabFlame;
    bool burnMore = false;
    CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float gravityValue = -9.81f;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        myHitCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead && !soldierScript.dead) {
            Gravity();
            myAnim.SetFloat("MoveSpeed", 1.0f);
            HitDetection(this.transform.position,1f);
            Move();
            LookAtPlayer();
        } else {
            myAnim.SetFloat("MoveSpeed", 0.0f);
        }
        if (burnMore)
            StartCoroutine(ImActuallyOnFire());
    }
    public void Dead()
    {
        myAnim.SetTrigger("Dead");
        dead = true;
        MakePrefab();
        Destroy(myHitCollider);
        Destroy(controller);
    }
    public void Move()
    {
        Vector3 direction = theSolider.transform.position - this.transform.position;
        //this.transform.Translate(direction * Time.deltaTime * moveSpeed);
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(direction * Time.deltaTime * moveSpeed);
        if (direction != Vector3.zero)
            gameObject.transform.forward = direction;
    }
    public void Init(GameObject newSoldier)
    {
        theSolider = newSoldier;
        soldierScript = theSolider.GetComponent<SoldierScript>();
    }
    void HitDetection(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, whatIsPlayer);
        for (int i = 0; i < hitColliders.Length; ++i) {
            if (hitColliders[i].gameObject != null) {
                SoldierScript sScript = hitColliders[i].gameObject.GetComponent<SoldierScript>();
                sScript.Dead();
            }
        }
    }
    public void LookAtPlayer()
    {
        //Changes rotation of object
        Vector3 targetPoint = new Vector3(theSolider.transform.position.x, this.transform.position.y, theSolider.transform.position.z) - transform.position;
        //Vector3 targetPoint = theSolider.transform.position - this.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime);
    }
    public void Burning()
    {
        burnMore = true;
    }
    IEnumerator ImActuallyOnFire()
    {
        burnMore = false;
        ParticleSystem aFire = Instantiate(prefabFlame, myNeck.transform.position, Quaternion.identity) as ParticleSystem;
        aFire.transform.SetParent(myNeck.transform);
        aFire.transform.localScale = Vector3.one * 0.5f;
        StartCoroutine(KillTimer(aFire.gameObject, 10f));
        yield return new WaitForSeconds(5f);
        //burnMore = true;
        yield return null;
    }
    public IEnumerator KillTimer(GameObject obj, float time)
    {
        if (time > 0)
            yield return new WaitForSeconds(time); 
        yield return null;
        Destroy(obj);
    }
    public void MakePrefab()
    {
        int rollTheDie = Random.Range(0,10);
        if (rollTheDie == 4) {
            Vector3 pos = Vector3.up * 1.7f;
            Instantiate(prefabMiniGun, this.transform.position+pos, Quaternion.identity);
        }
        if (rollTheDie == 3) {
            Vector3 pos = Vector3.up * 1.7f;
            Instantiate(prefabFlameThrower, this.transform.position+pos, Quaternion.identity);
        }
    }
    public void Gravity()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

}
