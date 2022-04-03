using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class WeaponLoader : MonoBehaviour
    {
        PlayerController playerController;

        public Transform recoilCamera;
        public Transform weaponPivot;
        public GameObject currentWeaponModel;
        public WeaponController currentWeaponController;
        public WeaponItem[] weaponsInInventory;
        WeaponItem currentWeapon;

        public void Init(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public void SetWeapon(WeaponItem weaponItem)
        {
            currentWeapon = weaponItem;
            currentWeaponController = LoadWeaponModel(weaponItem);
            currentWeaponController.Init(weaponItem, recoilCamera);
        }

        WeaponController LoadWeaponModel(WeaponItem weaponItem)
        {
            GameObject model = Instantiate(weaponItem.weaponPrefab) as GameObject;

            if (model != null)
            {
                if (weaponPivot != null)
                {
                    model.transform.parent = weaponPivot;
                }
                else
                {
                    model.transform.parent = transform;
                }

                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            currentWeaponModel = model;
            return currentWeaponModel.GetComponent<WeaponController>();
        }

        public void Shot(float delta)
        {
            currentWeaponController.Shot(delta);
        }
    }
}
