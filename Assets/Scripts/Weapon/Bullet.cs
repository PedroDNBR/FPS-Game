using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class Bullet : MonoBehaviour
    {
        public Transform dist;
        public float speed = 300;
        public float lifeTime = 10;
        public float travelDistance = 800;
        public float gravityMultiplier = 1;
        public int damage;
        float expireTime;
        public AnimationCurve damageDropOff;
        Vector3 velocity;
        RaycastHit hit;
        public int marks;

        int layer = 9;

        private void Start()
        {
            velocity = transform.forward * speed;
            expireTime = Time.time + lifeTime;
        }

        private void Update()
        {
            if(Time.time > expireTime)
                Destroy(this.gameObject);

            UpdatePosition();
        }

        void UpdatePosition()
        {
            travelDistance += speed * Time.deltaTime;
            velocity += Physics.gravity * gravityMultiplier * Time.deltaTime;

            Vector3 delta = this.velocity * Time.deltaTime;
            Travel(delta);
        }

        void Travel(Vector3 delta)
        {
            Vector3 nextPosition = transform.position + delta;

            Debug.DrawLine(transform.position, nextPosition);

            if(Physics.Linecast(transform.position, nextPosition, out hit, layer))
            {
                Debug.Log("Acertou Algo");
            }

            transform.position += delta;
            transform.rotation = Quaternion.LookRotation(delta);
        }
    }
}
