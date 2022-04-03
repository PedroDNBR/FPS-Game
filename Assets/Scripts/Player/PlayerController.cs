using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        InputHandler inputHandler;
        PlayerMovement playerMovement;
        WeaponLoader weaponLoader;

        [Header("Status")]
        public bool isSprinting;
        public bool isCrouching;

        private void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            playerMovement = GetComponent<PlayerMovement>();
            weaponLoader = GetComponent<WeaponLoader>();
        }

        private void Start()
        {
            playerMovement.Init(this);
            weaponLoader.Init(this);
            weaponLoader.SetWeapon(weaponLoader.weaponsInInventory[0]);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }

        private void Update()
        {
            HideMouse();
            HandleSprint();
            HandleCrouch();
            HandleShot(inputHandler.delta);
            inputHandler.HandleInputs();
            playerMovement.SetStance(inputHandler.delta);
            weaponLoader.currentWeaponController.HandleRecoil(inputHandler.delta);
        }

        private void FixedUpdate()
        {
            playerMovement.Move(inputHandler.vertical, inputHandler.horizontal, inputHandler.delta);
            playerMovement.LookRotation(inputHandler.mouseX, inputHandler.mouseY, inputHandler.delta);
            playerMovement.WeaponSway(inputHandler.mouseX, inputHandler.mouseY, inputHandler.vertical,
                inputHandler.horizontal, inputHandler.delta);
        }

        void HideMouse()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
                else
                    Cursor.lockState = CursorLockMode.Locked;

                Cursor.visible = !Cursor.visible;
            }
        }

        void HandleSprint()
        {
            if (isCrouching)
            {
                isSprinting = false;
                return;
            }

            if (inputHandler.sprintFlag)
                playerMovement.SetSprinting();
            else
                isSprinting = false;
        }

        void HandleCrouch()
        {
            if (playerMovement.slideAmount > 0)
            {
                isCrouching = true;
                return;
            }

            isCrouching = false;
            if (inputHandler.crouchFlag)
            {
                if (isSprinting)
                {
                    isCrouching = true;
                    playerMovement.slideAmount = playerMovement.slideMaxTime;
                }
                else
                {
                    isCrouching = true;
                }
            }
        }

        void HandleShot(float delta)
        {
            if (inputHandler.shotFlag)
                weaponLoader.Shot(delta);

        }
    }
}
