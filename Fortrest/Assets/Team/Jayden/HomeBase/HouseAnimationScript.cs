using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseAnimationScript : MonoBehaviour
{
    
    
    public Animator HouseAnimator; //Get the chosen animator of the target
    
    
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            HouseAnimator.SetBool("activateHouse", true);
            Debug.Log("youmadeitthisfar");
        }
        
    }

    // Update is called once per frame
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Its aintm");
    }
}
