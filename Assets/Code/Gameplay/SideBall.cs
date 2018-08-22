using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProccess
{
    public class SideBall : Ball
    {

        public float LiveTime = 10f;

        private float liveTimer = 0f;

        // Use this for initialization
        new void Start()
        {
            base.Start();

            liveTimer = LiveTime;
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();

            if (liveTimer > 0f)
            {
                liveTimer -= Time.deltaTime;
                if (liveTimer < 0f)
                {
                    liveTimer = 0;

                    BallDestroy();
                }
            }

        }

        public override void OnHit(Vector3 dir)
        {
            //BallDestroy();
        }

        public override void BallSpawn(Vector3 pos, Quaternion rot)
        {
            transform.position = pos;
            transform.rotation = rot;

            gameObject.SetActive(true);

            Start();
        }

        public override void BallDestroy()
        {
            gameObject.SetActive(false);
        }
    }
}