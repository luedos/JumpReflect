using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    public class Blank_MB : Blank
    {
        public Scalable ScaleObj;
        public GameObject RotObj;

        public GameObject Arrow;
        public GameObject myOutline;

        public Scalable CollisionScalable;
        public GameObject CollisionOutline;

        public CircleCollider2D myColl;

        [HideInInspector] public float StartSpeed = 5f;
        [HideInInspector] public float SpeedHitChange = 1.05f;
        [HideInInspector] public int HitAverage = 7;
        [HideInInspector] public int HitDelta = 2;
        [HideInInspector] public int SmallBalls = 3;
        [HideInInspector] public float ExpAngle = 90f;


        public override void ActivateBlank()
        {
            base.ActivateBlank();
            ScaleObj.gameObject.SetActive(true);
            CollisionScalable.gameObject.SetActive(false);
            CollisionOutline.SetActive(false);
            RotObj.SetActive(true);
            myOutline.SetActive(true);
            Arrow.SetActive(true);
            active = true;
        }

        public override void HideBlank()
        {
            base.HideBlank();
            ScaleObj.gameObject.SetActive(false);
            CollisionScalable.gameObject.SetActive(false);
            CollisionOutline.SetActive(false);
            RotObj.SetActive(false);
            myOutline.SetActive(false);
            Arrow.SetActive(false);
            active = false;
        }

        protected override void UpdateStaf()
        {
            ScaleObj.Start();
            //active = false;
            

            myColl.radius = mySR.sprite.rect.width > mySR.sprite.rect.height ? mySR.sprite.rect.width / mySR.sprite.pixelsPerUnit / 2 : mySR.sprite.rect.height / mySR.sprite.pixelsPerUnit / 2;

            //CollisionOutline.transform.localScale = new Vector3(myColl.radius, myColl.radius, 1f);


        }

        public void OpenCollisionOutline()
        {
            LevelCreatorManager.Instance.SetAsActiveBlank(this);

            HideBlank();

            CollisionOutline.gameObject.SetActive(true);
            CollisionScalable.Start();
            CollisionScalable.gameObject.SetActive(true);
        }

        public override void OnRightMouseButtonDown()
        {
            LevelCreatorManager.Instance.OpenMBMenu(this);
        }

        public override Blank CloneBlank()
        {
            return LevelCreatorManager.Instance.CreateBlank(E_LevelObjType.MainBall);
        }

        public override bool DeleteBlank()
        {
            if (LevelCreatorManager.Instance.MBList.Remove(this))
            {
                gameObject.SetActive(false);
                Destroy(this.gameObject);
                return true;
            }
            else
            {
                gameObject.SetActive(false);
                Destroy(this.gameObject);
                return false;
            }



        }



        public override string ToString()
        {
            // [0](Name = "some", SpawnPoint = 5|1.5|-1, StartRot = 30, Scale = 1.0|1.0, 
            //Color = 1|0|1|1, StartSpeed = 5, SpeedHitChange = 1.01, HitsAvarage = 7, HitsDelta = 2, SmallBalls = 3, ExpAngle = 90)

            Vector2 scale = ScaleObj.GetComponent<Scalable>().GetScale();

            return "SpawnPoint = " + transform.position.x.ToString() + "|" + transform.position.y.ToString() +
                    "|-1, StartRot = " + transform.rotation.eulerAngles.z.ToString() +
                    ", Scale = " + scale.x.ToString() + "|" + scale.y.ToString() +
                    ", Color = " + mySR.color.r.ToString() + "|" + mySR.color.g.ToString() + "|" + mySR.color.b.ToString() + "|" + mySR.color.a.ToString() +
                    ", StartSpeed = " + StartSpeed.ToString() + ", SpeedHitChange = " + SpeedHitChange.ToString() + ", HitsAvarage = " + HitAverage.ToString() +
                    ", HitsDelta = " + HitDelta.ToString() + ", SmallBalls = " + SmallBalls.ToString() + ", ExpAngle = " + ExpAngle.ToString() + 
                    ", CollisionRadius = " + CollisionOutline.transform.localScale.x.ToString();
        }
    }
}