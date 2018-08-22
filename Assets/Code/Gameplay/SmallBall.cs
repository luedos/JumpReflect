using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProccess
{
    public class SmallBall : Ball
    {

        public int MaxTouches = 3;

        int TouchTimer;

        // Use this for initialization
        new void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            --TouchTimer;

            if (TouchTimer < 1)
            {
                BallDestroy();
                return;
            }

            ContactPoint2D[] MyCP = collision.contacts;

            float inZ = transform.rotation.eulerAngles.z - 180;
            float n = Quaternion.FromToRotation(Vector2.up, MyCP[0].normal).eulerAngles.z;

            float outZ = n * 2 - inZ + Random.Range(-10, 10);

            transform.rotation = Quaternion.Euler(0, 0, outZ);

            outZ += 90f;
            outZ *= Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(outZ), Mathf.Sin(outZ)).normalized;

            myRB.velocity = dir * Speed;


        }

        public override void OnHit(Vector3 dir)
        {


            dir.z = 0;

            transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            Speed = Speed * SpeedChanging;

            myRB.velocity = dir.normalized * Speed;
        }

        public override void BallSpawn(Vector3 pos, Quaternion rot)
        {
            transform.position = pos;
            transform.rotation = rot;

            TouchTimer = MaxTouches;

            gameObject.SetActive(true);

            Start();
        }

        public override void BallDestroy()
        {
            gameObject.SetActive(false);
        }

    }
}