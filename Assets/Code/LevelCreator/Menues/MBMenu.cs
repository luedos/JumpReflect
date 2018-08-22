using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class MBMenu : RightClickMenu
    {

        public Blank_MB myBlank;

        [Header("MB params objects")]
        public Slider StartSpeedSlider;
        public Slider SpeedHitChangeSlider;
        public Slider HitsAverageSlider;
        public Slider HitsDeltaSlider;
        public Slider SmallBallsSlider;
        public Slider ExpAngleSlider;

        public Text StartSpeedText;
        public Text SpeedHitChangeText;
        public Text HitsAverageText;
        public Text HitsDeltaText;
        public Text SmallBallsText;
        public Text ExpAngleText;

        [Header("Sprite props")]
        public Slider SliderPPU;
        public Slider SliderXPivot;
        public Slider SliderYPivot;
        public Text inPPUText;
        public Text inXPivotText;
        public Text inYPivotText;

        public Assets.SimpleColorPicker.Scripts.ColorPicker myColorPicker;

        public TextureChooser myTC;

        public void OpenTextureChooser()
        {
            myTC.OpenMenu(myBlank);
        }

        Camera myCamera;

        private void Start()
        {
            myCamera = Camera.main;
        }

        public void UpdateStartSpeed(float inNum)
        {
            myBlank.StartSpeed = inNum;
            StartSpeedText.text = inNum.ToString();
        }
        public void UpdateSpeedHitChange(float inNum)
        {
            myBlank.SpeedHitChange = inNum;
            SpeedHitChangeText.text = inNum.ToString();
        }
        public void UpdateHitsAvarege(float inNum)
        {
            myBlank.HitAverage = Mathf.FloorToInt(inNum);
            HitsAverageText.text = Mathf.FloorToInt(inNum).ToString();
        }
        public void UpdateHitsDelta(float inNum)
        {
            myBlank.HitDelta = Mathf.FloorToInt(inNum);
            HitsDeltaText.text = Mathf.FloorToInt(inNum).ToString();
        }
        public void UpdateSmallBalls(float inNum)
        {
            myBlank.SmallBalls = Mathf.FloorToInt(inNum);
            SmallBallsText.text = Mathf.FloorToInt(inNum).ToString();
        }
        public void UpdateExpAngle(float inNum)
        {
            myBlank.ExpAngle = inNum;
            ExpAngleText.text = inNum.ToString();
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

        public void OpenMenu(Vector2 inScreenPos, Blank_MB inBlank)
        {
            myBlank = inBlank;

            SetPos(inScreenPos);

            gameObject.SetActive(true);
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

        private void OnEnable()
        {

            Vector2 pivot = new Vector2(myBlank.mySR.sprite.pivot.x / myBlank.mySR.sprite.rect.width,
               myBlank.mySR.sprite.pivot.y / myBlank.mySR.sprite.rect.height);

            SliderXPivot.value = pivot.x;
            SliderYPivot.value = pivot.y;

            inXPivotText.text = pivot.x.ToString();
            inYPivotText.text = pivot.y.ToString();

            pivot.x = myBlank.mySR.sprite.pixelsPerUnit;

            SliderPPU.value = pivot.x;

            inPPUText.text = pivot.x.ToString();

            StartSpeedSlider.value = myBlank.StartSpeed;
            StartSpeedText.text = myBlank.StartSpeed.ToString();

            SpeedHitChangeSlider.value = myBlank.SpeedHitChange;
            SpeedHitChangeText.text = myBlank.SpeedHitChange.ToString();

            HitsAverageSlider.value = myBlank.HitAverage;
            HitsAverageText.text = myBlank.HitAverage.ToString();

            HitsDeltaSlider.value = myBlank.HitDelta;
            HitsDeltaText.text = myBlank.HitDelta.ToString();

            SmallBallsSlider.value = myBlank.SmallBalls;
            SmallBallsText.text = myBlank.SmallBalls.ToString();

            ExpAngleSlider.value = myBlank.ExpAngle;
            ExpAngleText.text = myBlank.ExpAngle.ToString();
        }

        public void OpenCollisionOutline()
        {
            myBlank.OpenCollisionOutline();
            CloseMenu();
        }



    }
}