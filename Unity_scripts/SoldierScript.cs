using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoldierScript : MonoBehaviour
{
    public GameObject leaderBoard;
    public Vector3 shellAngle;
    public float throwPower = 30f;
    public GameObject prefabShell;
    public bool fastShoot = false, canRotate = true;
    public float spinSpeed;
    public GameObject miniGun;
    bool burnMore = true, flameOn = false;
    public ParticleSystem littleFlame, prefabFlame;
    public GameObject flameThrower;
    public GameObject deadPanel;
    public bool dead = false;
    public TextMeshProUGUI counterText;
    public int counter;
    public LayerMask whatIsZombie;
    public float angleAdj = 90f;
    public bool canShoot = true;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 4.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    public ParticleSystem flashPrefab;
    public GameObject flashPos;
    public Animator myAnim;
    public LineRenderer prefabBulletTrail;
    private float angle;
    public bool invincible = false;
    public GameObject flamePos;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        flameThrower.SetActive(false);
    }
    void Update()
    {
        if (!dead) {
            Jump();
            Move();
            FollowMouse();
            Shoot();
            if (Input.GetKey(KeyCode.Space)) {
                Time.timeScale = 0.2f;
            } else {
                Time.timeScale = 1.0f;
            }
            if (flameOn && burnMore) {
                StartCoroutine(Burn()); 
            } else if (fastShoot && canRotate) {
                StartCoroutine(PerformRotation(miniGun.transform, Vector3.up));
            }
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            if (invincible)
                invincible = false;
            else
                invincible = true;
        }
    }
    public void Shoot()
    {
        if (Input.GetMouseButton(0) && canShoot) {
            if (flameOn)
                StartCoroutine(MakeFire());
            else if (fastShoot) 
                StartCoroutine(MakeBoom(0.0f));
            else
                StartCoroutine(MakeBoom(0.5f));
            myAnim.SetBool("Shoot", true);
        } else {
            myAnim.SetBool("Shoot", false);
        }
    }
    IEnumerator MakeBoom(float time)
    {
        ParticleSystem myFlash = null;
        canShoot = false;
        if (fastShoot) {
            Vector3 direction = flashPos.transform.TransformDirection(Vector3.right*1.7f);
            myFlash = Instantiate(flashPrefab, flashPos.transform.position+direction, Quaternion.identity) as ParticleSystem;
            myFlash.transform.localScale = Vector3.one * 2;
        } else {
            myFlash = Instantiate(flashPrefab, flashPos.transform.position, Quaternion.identity) as ParticleSystem;
        }
        LineRenderer myBulletTrail = Instantiate(prefabBulletTrail, flashPos.transform.position, Quaternion.identity) as LineRenderer;
        myBulletTrail.transform.SetPositionAndRotation(flashPos.transform.position, flashPos.transform.rotation);
        CheckForHit();
        StartCoroutine(KillTimer(myBulletTrail.gameObject, 2f));
        StartCoroutine(KillTimer(myFlash.gameObject, 0f));
        MakeShell();
        if (time > 0)
            yield return new WaitForSeconds(0.5f);
        yield return null;
        canShoot = true;
    }
    IEnumerator MakeFire()
    {
        canShoot = false;
        ParticleSystem flameTrail = Instantiate(prefabFlame, flamePos.transform.position, Quaternion.identity) as ParticleSystem;
        flameTrail.transform.localScale = Vector3.one * 2f;
        flameTrail.transform.SetParent(flamePos.transform);
        flameTrail.transform.SetPositionAndRotation(flamePos.transform.position, flamePos.transform.rotation);
        CheckForBurn();
        StartCoroutine(KillTimer(flameTrail.gameObject, 0.5f));
        yield return null;
        canShoot = true;
    }
    public IEnumerator KillTimer(GameObject obj, float time)
    {
        if (time > 0)
            yield return new WaitForSeconds(time); 
        yield return null;
        Destroy(obj);
    }
    public void CheckForHit()
    {
        RaycastHit hit;
        Vector3 directionOne = flashPos.transform.TransformDirection(Vector3.right) * 100;
        Debug.DrawRay(flashPos.transform.position, directionOne, Color.green, 2, false);

        if (Physics.Raycast(flashPos.transform.position, directionOne, out hit, whatIsZombie)) {
            ZombieScript zScript = hit.collider.gameObject.GetComponent<ZombieScript>();
            if (zScript != null && !zScript.dead) {
                zScript.Dead();
                counter++;
                counterText.text = counter.ToString();
                StartCoroutine(KillTimer(zScript.gameObject, 200f));
                Destroy(hit.collider.GetComponent<CapsuleCollider>());  
            }
        }
    }
    public void CheckForBurn()
    {
        RaycastHit hit;
        Vector3 directionOne = flashPos.transform.TransformDirection((Vector3.right*5)+Vector3.forward);
        Vector3 directionTwo = flashPos.transform.TransformDirection((Vector3.right*5)+Vector3.back);

        if (Physics.Raycast(flashPos.transform.position, directionOne, out hit, whatIsZombie)
        || Physics.Raycast(flashPos.transform.position, directionTwo, out hit, whatIsZombie)) {
            ZombieScript zScript = hit.collider.gameObject.GetComponent<ZombieScript>();
            if (zScript != null) {
                counter++;
                counterText.text = counter.ToString();
                zScript.Burning();
                zScript.Dead();
                StartCoroutine(KillTimer(zScript.gameObject, 200f));
                Destroy(hit.collider.GetComponent<CapsuleCollider>());  
            }
        }
    }
    public void Dead()
    {
        if (!invincible) {
            myAnim.SetTrigger("Dead");
            dead = true;
            deadPanel.SetActive(true);
            leaderBoard.GetComponent<LeaderBoardScript>().InsertNewLeader(counter);
        }
    }
    public void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
            myAnim.SetBool("Move", true);
        } else {
            myAnim.SetBool("Move", false);
        }

    }
    public void Jump()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }
    public void FollowMouse()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 myPos = Camera.main.WorldToScreenPoint(this.transform.position);
        mouse -= myPos;
        angle = Mathf.Atan2(-mouse.y,mouse.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(Vector3.up * (angle+angleAdj));

    }
    public bool FlameThrowerActivated()
    {
        if (!fastShoot && !flameOn) {
            StartCoroutine(FlameOn());
            return true;
        }
        return false;
    }
    IEnumerator FlameOn()
    {
        flameThrower.SetActive(true);
        flameOn = true;
        yield return new WaitForSeconds(20f);
        flameOn = false;
        flameThrower.SetActive(false);
        yield return null;
    }
    public bool MiniGunActivated()
    {
        if (!fastShoot && !flameOn) {
            StartCoroutine(FastShoot());
            return true;
        }
        return false;
    }
    IEnumerator FastShoot()
    {
        miniGun.SetActive(true);
        fastShoot = true;
        yield return new WaitForSeconds(20f);
        fastShoot = false;
        miniGun.SetActive(false);
        yield return null;
    }
    IEnumerator Burn()
    {
        burnMore = false;
        ParticleSystem aFire = Instantiate(littleFlame, flashPos.transform.position, flashPos.transform.rotation) as ParticleSystem;
        aFire.transform.SetParent(flashPos.transform);
        aFire.transform.localScale = Vector3.one * 0.5f;
        StartCoroutine(KillTimer(aFire.gameObject, 2f));
        yield return new WaitForSeconds(1f);
        burnMore = true;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 directionOne = flashPos.transform.TransformDirection((Vector3.right*5)+Vector3.forward);
        Vector3 directionTwo = flashPos.transform.TransformDirection((Vector3.right*5)+Vector3.back);
        Gizmos.DrawRay(flashPos.transform.position, directionOne);
        Gizmos.DrawRay(flashPos.transform.position, directionTwo);
        Gizmos.color = Color.green;
    }
    IEnumerator PerformRotation(Transform rotatingTrans, Vector3 direction) 
    {
        canRotate = false;
        Quaternion originalRoation = rotatingTrans.localRotation;
        Quaternion targetRotation = originalRoation;
        targetRotation *= Quaternion.AngleAxis(90, direction);
        for (float t = 0; t <= 1; t += Time.deltaTime*spinSpeed) {
            rotatingTrans.localRotation = Quaternion.Lerp(originalRoation, targetRotation, t);
            yield return null;
        }
        rotatingTrans.localRotation = targetRotation;
        canRotate = true;
    }
    public void MakeShell()
    {
        Vector3 directionBack = flashPos.transform.TransformDirection(Vector3.left*0.5f);
        Vector3 directionDown = flashPos.transform.TransformDirection(Vector3.down);
        GameObject myShell = Instantiate(prefabShell, flashPos.transform.position+directionBack+directionDown, flashPos.transform.rotation) as GameObject;
        Rigidbody myShellRigidbody = myShell.GetComponentInChildren<Rigidbody>();
        myShellRigidbody.velocity = Vector3.zero;
        Vector3 tossAngle = flashPos.transform.TransformDirection(Vector3.up+Vector3.left);
        myShellRigidbody.AddForce(tossAngle * throwPower, ForceMode.Impulse);

    }

}
