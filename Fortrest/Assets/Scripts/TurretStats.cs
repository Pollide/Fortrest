using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
public class TurretStats : MonoBehaviour
{
    public Image fillImage;
    public Image selectImage;
    public Image iconImage;

    public Tier changeTier = new Tier();
    public Tier maxTier = new Tier();

    private void Awake()
    {
        maxTier.damageTier = changeTier.damageTier * 10;
        maxTier.healthTier = changeTier.healthTier * 10;
        maxTier.rangeTier = changeTier.rangeTier * 10;
        maxTier.rateTier = changeTier.rateTier * 10;
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
