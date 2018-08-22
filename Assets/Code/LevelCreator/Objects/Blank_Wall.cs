using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    public enum WallType : byte
    { InWall, OutWall }

    

    public class Blank_Wall : Blank
    {
        
        public GameObject ScaleObj;
        public GameObject RotObj;
        public GameObject myOutline;

        public Scalable CollisionScalable;
        public SpriteRenderer CollisionOutline;

        public BoxCollider2D myColl;

        public Sprite BoxCollSprite;
        public Sprite CircleCollSprite;

        public CollisionType myCollType = CollisionType.Box;

        public WallType myWallType = WallType.InWall;

        public override void HideBlank()
        {
            base.HideBlank();
            ScaleObj.SetActive(false);
            CollisionScalable.gameObject.SetActive(false);
            CollisionOutline.gameObject.SetActive(false);
            RotObj.SetActive(false);
            myOutline.SetActive(false);
            active = false;
        }

        public override void ActivateBlank()
        {
            base.ActivateBlank();
            ScaleObj.SetActive(true);
            CollisionScalable.gameObject.SetActive(false);
            CollisionOutline.gameObject.SetActive(false);
            RotObj.SetActive(true);
            myOutline.SetActive(true);
            active = true;
        }

        protected override void UpdateStaf()
        {
            Scalable localScalable = ScaleObj.GetComponent<Scalable>();
            if (localScalable != null)
                localScalable.Start();
            //active = false;

            myColl.size = new Vector2(mySR.sprite.rect.width / mySR.sprite.pixelsPerUnit, mySR.sprite.rect.height / mySR.sprite.pixelsPerUnit);

            //CollisionOutline.transform.localScale = new Vector3(myColl.size.x, myColl.size.y, 1f);
        }

        public void OpenCollisionOutline()
        {
            LevelCreatorManager.Instance.SetAsActiveBlank(this);

            HideBlank();

            CollisionOutline.gameObject.SetActive(true);
            CollisionScalable.Start();
            CollisionScalable.gameObject.SetActive(true);
        }

        public void ChangeCollision()
        {
            switch (myCollType)
            {
                case CollisionType.Box:
                    myCollType = CollisionType.Circle;
                    CollisionOutline.sprite = CircleCollSprite;
                    CollisionScalable.ScaleEqualy = true;
                    break;
                case CollisionType.Circle:
                    myCollType = CollisionType.Box;
                    CollisionOutline.sprite = BoxCollSprite;
                    CollisionScalable.ScaleEqualy = false;
                    break;
                default:
                    break;
            }
            CollisionScalable.Start();

        }

        public void ChangeCollision(CollisionType inType)
        {
            myCollType = inType;

            switch (myCollType)
            {
                case CollisionType.Circle:
                    CollisionOutline.sprite = CircleCollSprite;
                    CollisionScalable.ScaleEqualy = true;
                    break;
                case CollisionType.Box:
                    CollisionOutline.sprite = BoxCollSprite;
                    CollisionScalable.ScaleEqualy = false;
                    break;
                default:
                    break;
            }
            CollisionScalable.Start();

        }

        public CollisionType GetCollision()
        {
            return myCollType;
        }

        public override void OnRightMouseButtonDown()
        {
            LevelCreatorManager.Instance.OpenWallMenu(this);
        }

        public override Blank CloneBlank()
        {
            return LevelCreatorManager.Instance.CreateBlank(E_LevelObjType.Wall);
        }

        public override bool DeleteBlank()
        {
            if (LevelCreatorManager.Instance.WallList.Remove(this))
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
            Vector2 scale = ScaleObj.GetComponent<Scalable>().GetScale();

            string collStr = "";

            switch (myCollType)
            {
                case CollisionType.Box:
                    collStr = "Box";
                    break;
                case CollisionType.Circle:
                    collStr = "Circle";
                    break;
                default:
                    break;
            }

            return "Pos = " + transform.position.x.ToString() + "|" + transform.position.y.ToString() +
                    ", Rot = " + transform.rotation.eulerAngles.z.ToString() +
                    ", Scale = " + scale.x.ToString() + "|" + scale.y.ToString() +
                    ", Color = " + mySR.color.r.ToString() + "|" + mySR.color.g.ToString() + "|" + mySR.color.b.ToString() + "|" + mySR.color.a.ToString() + 
                    ", Collision = " + collStr +
                    ", CollisionSize = " + CollisionOutline.transform.localScale.x.ToString() + "|" + CollisionOutline.transform.localScale.y.ToString();
        }
    }
}