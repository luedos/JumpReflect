using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class WallMenu : RightClickMenu
    {

        public Blank_Wall myBlank;

        public Assets.SimpleColorPicker.Scripts.ColorPicker myColorPicker;

        [Header("Sprite props")]
        public Slider SliderPPU;
        public Slider SliderXPivot;
        public Slider SliderYPivot;
        public Text inPPUText;
        public Text inXPivotText;
        public Text inYPivotText;
        public TextureChooser myTC;
        [Header("Wall props")]
        public Text inWallTypeText;
        public Text CollisionTypeText;

        public void OpenTextureChooser()
        {
            myTC.OpenMenu(myBlank);
        }

        Camera myCamera;

        public void OpenMenu(Vector2 inScreenPos, Blank_Wall inBlank)
        {
            myBlank = inBlank;

            SetPos(inScreenPos);

            gameObject.SetActive(true);
        }

        public void ChangeCollType()
        {
            myBlank.ChangeCollision();

            string collStr = "";

            switch (myBlank.GetCollision())
            {
                case CollisionType.Box:
                    collStr = "Colliion Type: Box";
                    break;
                case CollisionType.Circle:
                    collStr = "Colliion Type: Circle";
                    break;
                default:
                    break;
            }

            CollisionTypeText.text = collStr;
        }

        public void OpenColor()
        {
            if (myBlank == null || myColorPicker == null)
                return;

            Rect parentRect = transform.root.GetComponent<RectTransform>().rect;

            RectTransform ColorTransform = myColorPicker.GetComponent<RectTransform>();

            float xTrans = parentRect.width / Screen.width;
            float yTrans = parentRect.height / Screen.height;

            Vector3 worldPos = myBlank.transform.position;
            worldPos.x += myBlank.mySR.bounds.extents.x;



            Vector2 ScreenPos = myCamera.WorldToScreenPoint(worldPos);
            ScreenPos.x *= xTrans;
            ScreenPos.y *= yTrans;

            ScreenPos.x += parentRect.width * 0.02f;

            if (ScreenPos.x + ColorTransform.rect.width * ColorTransform.localScale.x > parentRect.width)
            {
                worldPos.x -= 2 * myBlank.mySR.bounds.extents.x;
                ScreenPos = myCamera.WorldToScreenPoint(worldPos);
                ScreenPos.x *= xTrans;
                ScreenPos.y *= yTrans;
                ScreenPos.x -= ColorTransform.rect.width * ColorTransform.localScale.x + parentRect.width * 0.02f;

                if (ScreenPos.x < 0)
                {
                    ScreenPos.x = (parentRect.width - ColorTransform.rect.width * ColorTransform.localScale.x) / 2;
                }
            }

            ScreenPos.y -= ColorTransform.rect.height * ColorTransform.localScale.y / 2;

            if (ScreenPos.y < 0)
                ScreenPos.y = 0;

            if (ScreenPos.y + ColorTransform.rect.height * ColorTransform.localScale.y > parentRect.height)
                ScreenPos.y = parentRect.height - ColorTransform.rect.height * ColorTransform.localScale.y;

            ScreenPos.x -= ColorTransform.rect.x * ColorTransform.localScale.x;
            ScreenPos.y -= ColorTransform.rect.y * ColorTransform.localScale.y;


            ScreenPos.x /= xTrans;
            ScreenPos.y /= yTrans;

            ColorTransform.position = ScreenPos;

            myColorPicker.OpenOnTextureHolder(myBlank);
            myColorPicker.gameObject.SetActive(true);

            CloseMenu();
        }

        public void ApplySpriteChanges()
        {
            myBlank.ChangePivotAndPPU(new Vector2(SliderXPivot.value, SliderYPivot.value), SliderPPU.value);
        }

        public void XPivotUpdate(float inNum)
        {
            inXPivotText.text = inNum.ToString();
        }
        public void YPivotUpdate(float inNum)
        {
            inYPivotText.text = inNum.ToString();
        }
        public void PPUPivotUpdate(float inNum)
        {
            inPPUText.text = inNum.ToString();
        }
        public void ChangeWallType()
        {
            if (myBlank.myWallType == WallType.InWall)
            {
                myBlank.myWallType = WallType.OutWall;
                inWallTypeText.text = "OutWall";
            }
            else
            {
                myBlank.myWallType = WallType.InWall;
                inWallTypeText.text = "InWall";
            }
        }

        private void OnEnable()
        {
            if (myBlank.myWallType == WallType.InWall)
            {
                inWallTypeText.text = "InWall";
            }
            else
            {
                inWallTypeText.text = "OutWall";
            }

            string collStr = "";

            switch (myBlank.GetCollision())
            {
                case CollisionType.Box:
                    collStr = "Colliion Type: Box";
                    break;
                case CollisionType.Circle:
                    collStr = "Colliion Type: Circle";
                    break;
                default:
                    break;
            }

            CollisionTypeText.text = collStr;

            Vector2 pivot = new Vector2(myBlank.mySR.sprite.pivot.x / myBlank.mySR.sprite.rect.width,
               myBlank.mySR.sprite.pivot.y / myBlank.mySR.sprite.rect.height);

            SliderXPivot.value = pivot.x;
            SliderYPivot.value = pivot.y;

            inXPivotText.text = pivot.x.ToString();
            inYPivotText.text = pivot.y.ToString();

            pivot.x = myBlank.mySR.sprite.pixelsPerUnit;

            SliderPPU.value = pivot.x;

            inPPUText.text = pivot.x.ToString();
        }

        // Use this for initialization
        void Start()
        {
            myCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OpenCollisionOutline()
        {
            myBlank.OpenCollisionOutline();
            CloseMenu();
        }
    }
}