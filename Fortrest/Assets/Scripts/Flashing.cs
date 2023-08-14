using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashing : MonoBehaviour
{
    public EnemyController script;

    private void End()
    {
        script.flashing = false;
    }
}
