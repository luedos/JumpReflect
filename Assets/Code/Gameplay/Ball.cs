using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProccess
{
    public class Ball : MonoBehaviour
    {

        public float Speed = 5.0f;
        public float TriggerRadius = 0.55f;
        public float DamageWait = 0.08f;

        public float SpeedChanging = 1.0f;

        private float DamageTimer = 0.0f;

        protected Rigidbody2D myRB;

        // Use this for initialization
        public void Start()
        {
            if (!myRB)
            {
                myRB = GetComponent<Rigidbody2D>();
                if (!myRB)
                    print("out of rigid body : " + name);
            }
            //transform.rotation = Quaternion.Euler(0,0, Random.Range(0, 360));

            float a = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0).normalized;

            myRB.velocity = dir * Speed;

        }

        // Update is called once per frame
        public void Update()
        {

            if (DamageTimer > 0f)
            {
                DamageTimer -= Time.deltaTime;

                if (DamageTimer < 0f)
                {
                    Collider2D[] myColl = Physics2D.OverlapCircleAll(transform.position, TriggerRadius * transform.localScale.x);
                    int i = myColl.Length - 1;

                    for (; i > -1; --i)
                    {
                        if (myColl[i].tag == "Player")
                        {
                            Player myP = myColl[i].GetComponent<Player>();

                            if (!myP)
                                continue;

                            myP.Damage();
                            break;
                        }
                    }

                    DamageTimer = 0.0f;

                }
            }

        }



        //private void OnCollisionExit2D(Collision2D collision)
        //{
        //    DoCollision = true;
        //}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
                DamageTimer = DamageWait;

        }

        public void ChangeDirection(Vector3 dir, float localSpeedChanging = 1.0f)
        {
            dir.z = 0;

            transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            Speed = Speed * localSpeedChanging;

            myRB.velocity = dir.normalized * Speed;


        }


        public virtual void OnHit(Vector3 dir)
        {

        }

        public virtual void BallDestroy()
        {

        }

        public virtual void BallSpawn(Vector3 pos, Quaternion rot)
        {

        }
    }
}