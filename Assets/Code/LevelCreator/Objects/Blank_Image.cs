using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    public class Blank_Image : MonoBehaviour, ITextureDataHolder
    {
        public TextureData myTextureData;
        public UnityEngine.UI.Image myImage;

        public Vector2 DesireImageRatio;
        public float UnitRadius;

        private void Start()
        {
            SetTextureData(myTextureData);
        }

        public void ChangePivot(Vector2 inPivot)
        { }

        public void ChangePPU(float PPU)
        { }

        public Sprite GetSprite()
        {
            return myImage.sprite;
        }

        public TextureData GetTextureData()
        {
            return myTextureData;
        }

        public void SetTextureData(TextureData inTD)
        {
            myTextureData = inTD;

            float ratio = myTextureData.myTexture.width / myTextureData.myTexture.height;

            if (DesireImageRatio.x / DesireImageRatio.y > ratio)
                myImage.rectTransform.sizeDelta = new Vector2(DesireImageRatio.y * ratio, DesireImageRatio.y);
            else
                myImage.rectTransform.sizeDelta = new Vector2(DesireImageRatio.x, DesireImageRatio.x / ratio);

            myImage.sprite = Sprite.Create(myTextureData.myTexture, new Rect(0, 0, myTextureData.myTexture.width, myTextureData.myTexture.height), new Vector2(0.5f, 0.5f),
                ratio > 1 ? myTextureData.myTexture.width / (2 * UnitRadius) : myTextureData.myTexture.height / (2 * UnitRadius));
        }

        public void SetColor(Color inColor)
        {
            myImage.color = inColor;
        }

        public Color GetColor()
        {
            return myImage.color;
        }
    }
}