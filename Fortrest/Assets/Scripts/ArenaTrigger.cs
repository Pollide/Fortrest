using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boar"))
        {
            if (Boar.global.mounted)
            {
                Boar.global.Mount();
            }
            Boar.global.cc.Move((transform.forward * -10));
        } 
    }
}
