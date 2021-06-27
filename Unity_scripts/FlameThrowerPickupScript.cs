using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerPickupScript : MonoBehaviour
{
    public LayerMask whatIsPlayer;
    bool canMove = true;
    float timeOfTravel = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) {
            float height = this.transform.position.y;
            if (height > 1.5f && height <= 2.5f) {
                StartCoroutine(MoveAndWait(0.75f));
            } else if (height >= 0.5f && height <= 1.5f)
                StartCoroutine(MoveAndWait(2.25f));
        }
        HitDetection(this.transform.position, 1f); 
    }
    public IEnumerator MoveAndWait(float theY)
    {
        canMove = false;
        Vector3 startPos = this.transform.position;
        Vector3 desPos = new Vector3(startPos.x, theY, startPos.z);
        timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) { 
            currentTime += Time.deltaTime; 
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 
            this.transform.position = Vector3.Slerp(startPos,desPos, normalizedValue); 
            yield return null;
        }
        this.transform.position = desPos;
        canMove = true;
        yield return null;
    }
    void HitDetection(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, whatIsPlayer);
        for (int i = 0; i < hitColliders.Length; ++i) {
            if (hitColliders[i].gameObject != null) {
                SoldierScript sScript = hitColliders[i].gameObject.GetComponent<SoldierScript>();
                if (sScript.FlameThrowerActivated())
                    Destroy(gameObject);
            }
        }
    }
}
