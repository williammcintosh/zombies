using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSpinnerScript : MonoBehaviour
{
    public float speed = 1.0f;
    public GameObject theSun;
    bool canRotate = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canRotate)
            StartCoroutine(PerformRotation(theSun.transform, Vector3.right+Vector3.up));
    }
    IEnumerator PerformRotation(Transform rotatingTrans, Vector3 direction) 
    {
        canRotate = false;
        Quaternion originalRoation = rotatingTrans.rotation;
        Quaternion targetRotation = originalRoation;
        targetRotation *= Quaternion.AngleAxis(90, direction);
        for (float t = 0; t <= 1; t += Time.deltaTime*speed) {
            rotatingTrans.rotation = Quaternion.Lerp(originalRoation, targetRotation, t);
            yield return null;
        }
        rotatingTrans.rotation = targetRotation;
        canRotate = true;
    }

}
