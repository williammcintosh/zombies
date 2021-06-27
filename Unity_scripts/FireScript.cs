using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public GameObject firePos;
    public ParticleSystem prefabFire;
    bool burnMore = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (burnMore)
            StartCoroutine(Burn()); 
    }
    IEnumerator Burn()
    {
        burnMore = false;
        ParticleSystem aFire = Instantiate(prefabFire, firePos.transform.position, firePos.transform.rotation) as ParticleSystem;
        aFire.transform.localScale = Vector3.one * 3f;
        StartCoroutine(KillTimer(aFire.gameObject, 2f));
        yield return new WaitForSeconds(1f);
        burnMore = true;
    }
    public IEnumerator KillTimer(GameObject obj, float time)
    {
        if (time > 0)
            yield return new WaitForSeconds(time); 
        yield return null;
        Destroy(obj);
    }
}
