using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NewBehaviourScript : MonoBehaviour
{

    public GameObject objetCloner;
    private float delay = 1f;
    // Start is called before the first frame update
    void Start()
    {

        InvokeRepeating("cloner", 0, delay);
    }

    
    void cloner()
    {
       GameObject objetClone = Instantiate(objetCloner);
       objetClone.SetActive(true);
        objetClone.transform.position = new Vector2(Random.Range(-50,50),0);

    }
    
}
