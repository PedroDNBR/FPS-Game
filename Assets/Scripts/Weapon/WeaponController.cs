using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Weapon Status")]
        public int currentAmmo;
        public int maxAmmo;
        public int maxMagazineAmmo;
        Transform muzzle;

        WeaponItem weaponItem;

        Transform recoilCamera;

        bool recoilFlag;

        float nextShoot;

        public void Init(WeaponItem weaponItem, Transform recoilCamera)
        {
            this.weaponItem = weaponItem;
            this.recoilCamera = recoilCamera;
            muzzle = this.transform.Find("Muzzle").transform;
            currentAmmo = weaponItem.maxMagazineAmmo;
            maxMagazineAmmo = weaponItem.maxMagazineAmmo;
            maxAmmo = weaponItem.maxMagazineAmmo * 3;
        }

        public void Shot(float delta)
        {

            if (Time.time < weaponItem.fireRate + nextShoot)
                return;

            InstantiateProjectile();
            nextShoot = Time.time;
            recoilFlag = true;
            return;
        }

        float curveT;
        public void HandleRecoil(float delta)
        {
            Quaternion targetRotation = Quaternion.identity;
            Vector3 targetPosition = Vector3.zero;

            float lerpSpeed = delta * weaponItem.recoilResetSpeed;

            float horizontalRecoil = (weaponItem.recoilCurve.Evaluate(curveT) + 1) * weaponItem.recoilMultiplier;

            if (recoilFlag)
            {
                targetPosition.x += (Random.Range(-weaponItem.horizontalSway, weaponItem.horizontalSway) / 3) * weaponItem.recoilMultiplier;
                targetPosition.y += Random.Range(-weaponItem.verticalSway, weaponItem.verticalSway) * weaponItem.recoilMultiplier;
                targetPosition.z -= Random.Range(0, weaponItem.kick) * weaponItem.recoilMultiplier;

                targetRotation.x -= (horizontalRecoil + 1) * weaponItem.horizontalSway * weaponItem.recoilMultiplier;
                targetRotation.y += Random.Range(-weaponItem.kick, weaponItem.kick) * weaponItem.recoilMultiplier;
                targetRotation.z -= Random.Range(-weaponItem.verticalSway, weaponItem.verticalSway) * weaponItem.recoilMultiplier;

                if (curveT > 1)
                {
                    curveT = 0;
                }

                recoilFlag = false;
            }
            else
            {
                if (curveT > 1)
                {
                    curveT = 1;
                }


                if(curveT > 0)
                {
                    curveT -= delta / weaponItem.recoilResetSpeed;
                }
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, lerpSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, lerpSpeed);

            recoilCamera.localPosition = Vector3.Lerp(recoilCamera.localPosition, targetPosition, lerpSpeed / 4);
            recoilCamera.localRotation = Quaternion.Slerp(recoilCamera.localRotation, targetRotation, lerpSpeed / 4);
        }

        void InstantiateProjectile()
        {
            Instantiate(weaponItem.bullet, muzzle.transform.position, muzzle.transform.rotation);
        }
    }

}
