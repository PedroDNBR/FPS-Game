using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : ScriptableObject
    {
        [Header("Weapon Info")]
        public string name;
        public int maxMagazineAmmo;
        public float fireRate;
        public WeaponTypes weaponType = new WeaponTypes();
        public float baseDamage = 15;
        public GameObject weaponPrefab;
        public GameObject bullet;

        [Header("Weapon Recoil")]
        public float recoilResetSpeed = 5f;
        public float recoilMultiplier = 1f;
        public float kick = 1f;
        public float horizontalSway = 1f;
        public float verticalSway = 1f;
        public AnimationCurve recoilCurve;
    }

    public enum WeaponTypes
    {
        Rifle,
        Pistol,
    };
}
