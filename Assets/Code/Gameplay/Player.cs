using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProccess
{
    public class Player : MonoBehaviour
    {

        public bool GodMode = false;

        [Header("PlayerStats")]
        public int StartHP = 3;

        [Header("Movement")]
        public float Speed = 5;
        [Range(0, 1)]
        public float AirControl = 0.5f;
        [Header("Jumping")]
        public float JumpForce = 5;
        public float FallMultiplier = 2.5f;
        public float MaxFallSpeed = 5.0f;
        public float LowJumpMultiplier = 2f;
        public float CoyoteJump = 0.08f;

        [Header("Hiting")]
        public float HitRadius = 1f;
        public float HitReloadTime = 0.7f;

        [Header("Invinsibility Settings")]
        public float InvulTimeAfterHit = 0.5f;
        public float InvulTimeOnDamage = 1f;

        private Material myMat;


        [Header("Animations")]
        public Animation MyAnimation;
        public string jumpAnim;
        public string landAnim;

        private int HP;
        private Rigidbody2D myRB;
        private bool isGrounded = true;
        private bool isJumped = false;
        private float coyoteTimer = 0f;
        private float HorizontalVector = 0.0f;

        private float invinsibleTimer = 0f;
        private float hitTimer = 0f;


        int TestInt = 0;

        public int GetHP() { return HP; }

        // Use this for initialization
        void Start()
        {


            SpriteRenderer myTempSR = GetComponentInChildren<SpriteRenderer>();
            if (myTempSR)
                myMat = myTempSR.material;



            Rigidbody2D localRB = GetComponent<Rigidbody2D>();

            if (localRB)
                myRB = localRB;
            else
                print("out of rigidbody : " + name);

            HP = StartHP;
        }

        // Update is called once per frame
        void Update()
        {


            // ---------//Timers//------------


            // hit timer
            if (hitTimer > 0)
            {
                hitTimer -= Time.deltaTime;

                //myMat.SetFloat("_Percent", (1 - hitTimer) / HitDeltaTime); 

                if (hitTimer < 0)
                {
                    myMat.SetFloat("_Reload", 0);
                    hitTimer = 0f;
                }
            }

            // invinsibility timer
            if (invinsibleTimer > 0)
            {
                invinsibleTimer -= Time.deltaTime;

                myMat.SetFloat("_InvulAlpha", Mathf.Sin(Time.time * 20) * 0.4f + 0.6f);


                if (invinsibleTimer < 0)
                {
                    //myMat.SetFloat("_Alpha", 1);

                    myMat.SetFloat("_Invul", 0);

                    invinsibleTimer = 0f;

                    BoxCollider2D mybc = GetComponent<BoxCollider2D>();
                    if (mybc)
                    {
                        Collider2D[] myColl = Physics2D.OverlapBoxAll(transform.position, mybc.size, 0f);

                        int i = myColl.Length - 1;

                        for (; i > -1; --i)

                            if (myColl[i].tag == "Ball")
                            {
                                Damage();
                                return;
                            }
                    }

                }
            }

            // coyoty timer
            if (coyoteTimer > 0f)
            {
                coyoteTimer -= Time.deltaTime;
                if (coyoteTimer < 0f)
                {
                    coyoteTimer = 0f;
                    isGrounded = false;
                }
            }


            // ----------//Controls//------------

            // jumping
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                isJumped = true;
                myRB.velocity = Vector2.up * JumpForce;
                //MyAnimation.Stop();

                MyAnimation.Play(jumpAnim, PlayMode.StopAll);

            }

            // movement
            if (Input.GetButton("Horizontal"))
            {
                //myRB.velocity = myRB.velocity.y * Vector2.up + Vector2.right * Input.GetAxis("Horizontal") * Speed;

                float HorAxis = Input.GetAxis("Horizontal");

                HorizontalVector += (HorAxis - HorizontalVector) * 35 * Time.deltaTime * (isGrounded ? 1 : AirControl);


            }
            else
            {
                HorizontalVector -= HorizontalVector * 20 * Time.deltaTime * (isGrounded ? 1 : AirControl * 0.2f);
            }
            myRB.velocity = myRB.velocity.y * Vector2.up + HorizontalVector * Vector2.right * Speed;

            // hiting
            if (hitTimer == 0 && Input.GetButtonDown("Hit"))
            {
                myMat.SetFloat("_Reload", 1);
                Hit();

                TestInt++;
                if (TestInt > 3)
                    TestInt = 0;

            }




            // -------------// Physics adjusting //---------------

            if (!isGrounded && myRB.velocity.y < 0 && Mathf.Abs(myRB.velocity.y) < MaxFallSpeed)
                myRB.velocity += Physics2D.gravity * (FallMultiplier - 1) * Time.deltaTime;

            if (isJumped && myRB.velocity.y > 0 && Input.GetButton("Jump"))
                myRB.velocity -= Physics2D.gravity * (LowJumpMultiplier - 1) * Time.deltaTime;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ContactPoint2D[] localPoints = collision.contacts;
            int i = localPoints.Length - 1;
            if (i >= 0)
                for (; i > -1; --i)
                    if (Vector2.Dot(localPoints[i].normal, Vector2.up) > 0.7f)
                    {
                        isGrounded = true;
                        coyoteTimer = 0f;
                        isJumped = false;
                        //MyAnimation.Stop();
                        MyAnimation.Play(landAnim, PlayMode.StopAll);
                        return;
                    }



        }

        private void OnDrawGizmos()
        {
            if (hitTimer > 0)
                Gizmos.DrawWireSphere(transform.position, HitRadius);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            BoxCollider2D mybc = GetComponent<BoxCollider2D>();
            if (mybc)
            {
                Vector2 Boxsize = mybc.size;
                Boxsize.y *= 0.9f;
                RaycastHit2D[] localRaycasts = Physics2D.BoxCastAll(transform.position, Boxsize, 0.0f, Vector2.down, Boxsize.y * 0.1f);

                // isGrounded = false;
                coyoteTimer = CoyoteJump;

                int i = localRaycasts.Length - 1;
                if (i >= 0)
                {
                    for (; i > -1; --i)
                        if (Vector2.Dot(localRaycasts[i].normal, Vector2.up) > 0.7)
                        {
                            //isGrounded = true;  
                            coyoteTimer = 0f;
                            return;
                        }
                }

            }

        }

        public void PlayerSpawn(Vector3 pos)
        {
            transform.position = pos;

            HP = StartHP;

            HorizontalVector = 0f;

            gameObject.SetActive(true);


            GameManager.Instance.UpdatePlayerStats(this);
        }

        public void PlayerDeath()
        {
            myRB.velocity = Vector2.zero;

            GameManager.Instance.GameOver(this);

            gameObject.SetActive(false);
        }

        private void Hit()
        {
            bool HasBall = false;

            Collider2D[] myColl = Physics2D.OverlapCircleAll(transform.position, HitRadius);

            int i = myColl.Length - 1;

            for (; i > -1; --i)
            {
                if (myColl[i].tag == "Ball")
                {
                    Ball myBall = myColl[i].GetComponent<Ball>();
                    if (!myBall)
                        continue;


                    GameManager.Instance.AddPoints(myRB.velocity.magnitude * myBall.GetComponent<Rigidbody2D>().velocity.magnitude * 0.2f);

                    myBall.OnHit(myColl[i].transform.position - transform.position);

                    HasBall = true;
                }
            }

            if (HasBall)
            {
                if (invinsibleTimer < InvulTimeAfterHit)
                {
                    invinsibleTimer = InvulTimeAfterHit;
                    myMat.SetFloat("_Invul", 1);
                }
            }
            else
            {
                if (invinsibleTimer < InvulTimeAfterHit * 0.5f)
                {
                    invinsibleTimer = InvulTimeAfterHit * 0.5f;
                    myMat.SetFloat("_Invul", 1);
                }
            }


            hitTimer = HitReloadTime;
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if(collision.tag == "Ball" && invinsibleTimer == 0f)
        //    {
        //         Damage();
        //    }
        //}

        public void Damage()
        {
            if (GodMode)
                return;

            if (invinsibleTimer == 0)
            {
                --HP;

                GameManager.Instance.UpdatePlayerStats(this);
                if (HP < 1)
                {

                    PlayerDeath();
                    return;
                }

                invinsibleTimer = InvulTimeOnDamage;
                myMat.SetFloat("_Invul", 1);
            }
        }

    }
}