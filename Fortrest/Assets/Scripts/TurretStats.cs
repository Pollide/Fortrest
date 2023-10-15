using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
public class TurretStats : MonoBehaviour
{
    public Image fillImage;
    public Image selectImage;
    public Image iconImage;

    public Tier changeTier = new Tier();

    [System.Serializable]
    public class Tier
    {
        public float damageTier;
        public float healthTier;
        public float rangeTier;
        public float rateTier;


    }
}
