using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProccess
{
    public class MainBall : Ball
    {

        public Transform StartPos;

        public CircleCollider2D OutCollider;
        public CircleCollider2D InCollider;

        [Header("Small balls parametrs")]
        public int AvarageHitNumber = 7;
        public int RandomRange = 2;
        public int NumberOfSB = 3;
        public float ExploadAngle = 90;



        private float personalSpeed;


        private int HitsBeforeExpload;

        // Use this for initialization
        new void Start()
        {

            base.Start();

            personalSpeed = Speed;

            HitsBeforeExpload = AvarageHitNumber + Random.Range(-RandomRange, RandomRange);

        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
        }

        public void ChangeCollisionRadius(float inRadius)
        {
            OutCollider.radius = inRadius;

            InCollider.radius = inRadius * 0.86f;
        }

        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    return;
        //
        //    ContactPoint2D[] MyCP = collision.contacts;
        //
        //    float inZ = transform.rotation.eulerAngles.z - 180;
        //    float n = Quaternion.FromToRotation(Vector2.up, MyCP[0].normal).eulerAngles.z;
        //
        //    float outZ = n * 2 - inZ + Random.Range(-10, 10);
        //
        //    transform.rotation = Quaternion.Euler(0, 0, outZ);
        //
        //    outZ += 90f;
        //    outZ *= Mathf.Deg2Rad;
        //
        //    Vector2 dir = new Vector2(Mathf.Cos(outZ), Mathf.Sin(outZ)).normalized;
        //
        //    myRB.velocity = dir * personalSpeed;
        //
        //
        //}

        public override void BallSpawn(Vector3 pos, Quaternion rot)
        {
            transform.position = pos;
            transform.rotation = rot;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            Start();

        }


        public override void OnHit(Vector3 dir)
        {
            dir.z = 0;

            transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            personalSpeed = personalSpeed * SpeedChanging;

            myRB.velocity = dir.normalized * personalSpeed;




            if (--HitsBeforeExpload < 1)
            {
                HitsBeforeExpload = AvarageHitNumber + Random.Range(-RandomRange, RandomRange);



                //Instantiate<SmallBall>(mySmallBall, null);

                float StartAngle = Quaternion.FromToRotation(Vector3.up, dir).eulerAngles.z - ExploadAngle / 2;

                if (DataManipulators.DataPool.Instance != null)
                    for (int i = 0; i < NumberOfSB; ++i)
                    {

                        GameObject localObj = DataManipulators.DataPool.Instance.GetInactiveObject(DataManipulators.DataPool.E_ObjectType.SmallBall);

                        if (localObj == null)
                        {
                            return;
                        }

                        Ball localComp = localObj.GetComponent<Ball>();

                        if (localComp)
                            localComp.BallSpawn(transform.position, Quaternion.Euler(0, 0, StartAngle + i * ExploadAngle / (NumberOfSB - 1)));



                    }
            }
        }

        public override void BallDestroy()
        {
            BallSpawn(StartPos.position, StartPos.rotation);
        }
    }
}