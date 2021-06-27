using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrailScript : MonoBehaviour
{
    public float travelSpeed = 230f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.right * Time.deltaTime * travelSpeed);
    }
}
