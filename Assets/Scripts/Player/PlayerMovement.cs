using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        public Transform cameraPivotTransform;
        public Transform headTransform;
        public Transform weaponParent;
        CharacterController characterController;
        PlayerController playerController;

        [Header("Movement")]
        public float walkSpeed = 7f;
        public float sprintSpeed = 10f;
        public float crouchSpeed = 3f;
        public float slideMaxTime = .1f;
        public Vector3 crouchingPivotPosition;
        public float slideAmount;
        Vector3 normalPivotPosition;

        [Header("Camera")]
        public float rotationSpeed = .1f;
        float lookAngle;
        float tiltAngle;

        [Header("Head Bob")]
        public AnimationCurve footstepCurve;
        public float footstepMultiplier;
        public float idleCycleTime = 1;
        public float walkCycleTime = 1;
        public float sprintCycleTime = 1;
        public float crouchCycleTime = 1;
        public float idleValue = 1;
        float cyclePosition;
        float footstepTime = 1;

        [Header("Weapon Sway")]
        public float swayMoveSpeed = 1;
        public float swaySensitivity;
        public float swayLerpSpeed = 1;
        public float xSwayMoveAmout = 10;
        public float ySwayMoveAmout = 10;
        public float rotateAmout = 45f;
        public float tiltSwaySpeed = .4f;
        float factorX;
        float factorY;

        public void Init(PlayerController playerController)
        {
            characterController = GetComponent<CharacterController>();
            this.playerController = playerController;
            normalPivotPosition = cameraPivotTransform.localPosition;
        }

        public void Move(float vertical, float horizontal, float delta)
        {
            // Faz o personagem andar para frente e pra tras
            Vector3 direction = transform.forward * vertical;
            // Faz o personagem andar para as laterais
            direction += transform.right * horizontal;
            direction.Normalize();

            RaycastHit hit;
            Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hit,
                characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            bool freezeController = false;

            if(slideAmount > 0)
            {
                direction = transform.forward;
                slideAmount -= delta;

                if (slideAmount <= 0)
                    freezeController = true;
            }

            Vector3 desiredMove = Vector3.ProjectOnPlane(direction, hit.normal).normalized;

            if(!characterController.isGrounded)
            {
                desiredMove += Physics.gravity;
            }

            float actualSpeed = walkSpeed;

            if (playerController.isCrouching)
                actualSpeed = crouchSpeed;

            if (playerController.isSprinting || slideAmount > 0)
                actualSpeed = sprintSpeed;

            if (freezeController)
                actualSpeed = 0;

            if (characterController.isGrounded)
                FootstepsShaking(delta);

            characterController.Move(desiredMove * (delta * actualSpeed));
        }

        public void LookRotation(float mouseX, float mouseY, float delta)
        {
            lookAngle += mouseX * (rotationSpeed * delta);
            Vector3 camEulers = Vector3.zero;
            camEulers.y = lookAngle;
            transform.eulerAngles = camEulers;

            tiltAngle -= mouseY * (rotationSpeed * delta);
            tiltAngle = Mathf.Clamp(tiltAngle, -70, 70);

            Vector3 tiltEuler = Vector3.zero;
            tiltEuler.x = tiltAngle;
            cameraPivotTransform.localEulerAngles = tiltEuler;
        }

        void FootstepsShaking(float delta)
        {
            bool isMoving = characterController.velocity.sqrMagnitude > 0;

            if (isMoving)
            {
                if (!playerController.isSprinting && !playerController.isCrouching)
                    cyclePosition += (characterController.velocity.sqrMagnitude * delta) / walkCycleTime;
                else if (playerController.isSprinting)
                    cyclePosition += (characterController.velocity.sqrMagnitude * delta) / sprintCycleTime;
                else if (playerController.isCrouching)
                    cyclePosition += (characterController.velocity.sqrMagnitude * delta) / crouchCycleTime;
            }
            else
            {
                cyclePosition += (idleValue * delta) / idleCycleTime;
            }

            if (cyclePosition > footstepTime)
            {
                cyclePosition = cyclePosition - footstepTime;
            }

            Vector3 localPosition = headTransform.localPosition;

            if (slideAmount > 0)
                cyclePosition = 0;

            localPosition.y = footstepCurve.Evaluate(cyclePosition) * footstepMultiplier;
            

            headTransform.localPosition = Vector3.Lerp(headTransform.localPosition, localPosition, delta / .0005f);

        }
    
        public void WeaponSway(float mouseX, float mouseY, float vertical, float horizontal, float delta)
        {
            Vector3 targetPosition = Vector3.zero;

            factorX = Mathf.Lerp(factorX, -mouseX * swaySensitivity + (-horizontal), delta / swayLerpSpeed);
            factorY = Mathf.Lerp(factorY, -mouseY * swaySensitivity + (-vertical), delta / swayLerpSpeed);

            factorX = Mathf.Clamp(factorX, -xSwayMoveAmout, xSwayMoveAmout);
            factorY= Mathf.Clamp(factorY, -ySwayMoveAmout, ySwayMoveAmout);

            targetPosition.x = factorX;
            targetPosition.y = factorY;

            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetPosition, delta / swayMoveSpeed);

            Vector3 targetTilt = Vector3.zero;
            targetTilt.x = mouseY * rotateAmout;
            targetTilt.z = mouseX * rotateAmout;
            targetTilt.y = mouseX * rotateAmout;

            weaponParent.localRotation = Quaternion.Slerp(
                weaponParent.localRotation, 
                Quaternion.Euler(targetTilt),
                delta / tiltSwaySpeed
            );
        }

        public void SetSprinting()
        {
            if (slideAmount > 0)
                return;

            playerController.isSprinting = true;
            playerController.isCrouching = false;
        }

        public void SetStance(float delta)
        {
            Vector3 targetPosition = normalPivotPosition;
            if (playerController.isCrouching)
                targetPosition = crouchingPivotPosition;

            if (slideAmount > 0)
                targetPosition.y -= 0.1f;

            cameraPivotTransform.localPosition = Vector3.Lerp(cameraPivotTransform.localPosition,
                targetPosition, delta / .1f);

            characterController.height = cameraPivotTransform.position.y;
            Vector3 crouchCenter = Vector3.zero;
            crouchCenter.y = cameraPivotTransform.position.y / 2;
            characterController.center = crouchCenter;
        }
    }
}
