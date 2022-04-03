using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class InputHandler : MonoBehaviour
    {
        public float vertical;
        public float horizontal;

        public float mouseX;
        public float mouseY;

        public float delta;

        public bool sprintFlag;
        public bool crouchFlag;
        public bool shotFlag;

        public void HandleInputs()
        {
            delta = Time.deltaTime;

            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            sprintFlag = Input.GetButton("Sprint");
            crouchFlag = Input.GetButton("Crouch");

            shotFlag = Input.GetButton("Fire1");
        }
    }
}
