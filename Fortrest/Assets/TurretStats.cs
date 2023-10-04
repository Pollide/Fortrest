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
        public int damageTier;
        public int healthTier;
        public int rangeTier;
        public int rateTier;
    }
}
