using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
public class TurretStats : MonoBehaviour
{
    public Image fillImage;
    public Image selectImage;
    public Image iconImage;

    public Tier changeTier = new Tier();

    public Tier ReturnMaxTier()
    {
        return new Tier() { damageTier = changeTier.damageTier * 10, healthTier = changeTier.healthTier * 10, rangeTier = changeTier.rangeTier * 10, rateTier = changeTier.rateTier * 10 };
    }

    [System.Serializable]
    public class Tier
    {
        public float damageTier;
        public float healthTier;
        public float rangeTier;
        public float rateTier;


    }
}
