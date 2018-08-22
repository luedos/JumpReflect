using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class MainPlayerMenu : RightClickMenu
    {

        public Blank_Player myBlank;
        // (SpawnPoint = 1|20, HP = 3, InvulDamageTime = 1.5, InvulHitTime = 0.7, HitReloadTime = 0.8, HitRadius = 0.5, Sprite = 1)
        [Header("MB params objects")]
        public Slider PlayerHPSlider;
        public Slider InvulDamageTimeSlider;
        public Slider InvulHitTimeSlider;
        public Slider HitReloadTimeSlider;
        public Slider HitRadiusSlider;

        public Text PlayerHPText;
        public Text InvulDamageTimeText;
        public Text InvulHitTimeText;
        public Text HitReloadTimeText;
        public Text HitRadiusText;

        [Header("Sprite props")]
        public Slider SliderPPU;
        public Slider SliderXPivot;
        public Slider SliderYPivot;
        public Text inPPUText;
        public Text inXPivotText;
        public Text inYPivotText;
        public TextureChooser myTC;

        public void OpenTextureChooser()
        {
            myTC.OpenMenu(myBlank);
        }

        public void UpdatePlayerHP(float inNum)
        {
            myBlank.PlayerHP = Mathf.FloorToInt(inNum);
            PlayerHPText.text = Mathf.FloorToInt(inNum).ToString();
        }
        public void UpdateInvulDamageTime(float inNum)
        {
            myBlank.InvulDamageTime = inNum;
            InvulDamageTimeText.text = inNum.ToString();
        }
        public void UpdateInvulHitTime(float inNum)
        {
            myBlank.InvulHitTime = inNum;
            InvulHitTimeText.text = inNum.ToString();
        }
        public void UpdateHitReloadTime(float inNum)
        {
            myBlank.HitReloadTime = inNum;
            HitReloadTimeText.text = inNum.ToString();
        }
        public void UpdateHitRadius(float inNum)
        {
            myBlank.HitRadius = inNum;
            HitRadiusText.text = inNum.ToString();
        }

        public void OpenMenu(Vector2 inScreenPos, Blank_Player inBlank)
        {
            myBlank = inBlank;

            SetPos(inScreenPos);

            gameObject.SetActive(true);
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

            PlayerHPSlider.value = myBlank.PlayerHP;
            PlayerHPText.text = myBlank.PlayerHP.ToString();

            InvulDamageTimeSlider.value = myBlank.InvulDamageTime;
            InvulDamageTimeText.text = myBlank.InvulDamageTime.ToString();

            InvulHitTimeSlider.value = myBlank.InvulHitTime;
            InvulHitTimeText.text = myBlank.InvulHitTime.ToString();

            HitReloadTimeSlider.value = myBlank.HitReloadTime;
            HitReloadTimeText.text = myBlank.HitReloadTime.ToString();

            HitRadiusSlider.value = myBlank.HitRadius;
            HitRadiusText.text = myBlank.HitRadius.ToString();
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
    }
}