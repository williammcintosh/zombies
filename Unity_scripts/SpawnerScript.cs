using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public float duration = 5f;
    int count = 0;
    public GameObject zombie, spawnSpotOne, spawnSpotTwo, spawnSpotThree, spawnSpotFour;
    public GameObject soldier;
    SoldierScript sScript;
    public bool canMake = false, stop = false;
    float setMoveSpeed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(WaitToBegin());
       sScript = soldier.GetComponent<SoldierScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMake && !stop && !sScript.dead) {
            if (count % 10 == 0 && count > 0) {
                duration -= 0.2f;
                setMoveSpeed += 0.1f;
            }
            GameObject spot = PickASpot();
            StartCoroutine(MakeAZombie(spot));
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            if (stop)
                stop = false;
            else
                stop = true;
        }
    }
    public GameObject PickASpot()
    {
        GameObject pos = spawnSpotOne;
        int rollTheDie = Random.Range(1,5);
        switch (rollTheDie) {
            case 1:
                pos = spawnSpotOne;
                break;
            case 2:
                pos = spawnSpotTwo;
                break;
            case 3:
                pos = spawnSpotThree;
                break;
            case 4:
                pos = spawnSpotFour;
                break;
            case 5:
                pos = spawnSpotFour;
                break;
                
        }
        return pos;
    }
    IEnumerator MakeAZombie(GameObject pos)
    {
        canMake = false; 
        GameObject myZombie = Instantiate(zombie, pos.transform.position, Quaternion.identity) as GameObject;
        ZombieScript zScript = myZombie.GetComponent<ZombieScript>();
        zScript.moveSpeed = setMoveSpeed;
        count++;
        zScript.Init(soldier);
        yield return new WaitForSeconds(duration);
        canMake = true;
        yield return null;
    }
    IEnumerator WaitToBegin()
    {
        yield return new WaitForSeconds(3); 
        canMake = true;
        yield return null;
    }
}
