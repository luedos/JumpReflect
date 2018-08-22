using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelCreator
{
    public enum CollisionType : byte { Box, Circle }

    public class Blank : MonoBehaviour, ITextureDataHolder
    {

        public SpriteRenderer mySR;
        public TextureData myTextureData;
        public string Name;

        protected bool active = false;

        public bool Active { get { return active; } }


        public void ChangePivot(Vector2 inVector)
        {
            mySR.sprite = Sprite.Create(mySR.sprite.texture, new Rect(0, 0, mySR.sprite.texture.width, mySR.sprite.texture.height), inVector, mySR.sprite.pixelsPerUnit);
            UpdateStaf();
        }

        public void ChangePivotAndPPU(Vector2 inPivot, float inPPU)
        {
            mySR.sprite = Sprite.Create(mySR.sprite.texture, new Rect(0, 0, mySR.sprite.texture.width, mySR.sprite.texture.height), inPivot, inPPU);
            UpdateStaf();
        }

        public TextureData GetTextureData()
        {
            return myTextureData;
        }

        public void SetTextureData(TextureData inTextureData)
        {
            myTextureData = inTextureData;

            mySR.sprite = Sprite.Create(myTextureData.myTexture, new Rect(0, 0, myTextureData.myTexture.width, myTextureData.myTexture.height),
                new Vector2(mySR.sprite.pivot.x / mySR.sprite.rect.width, mySR.sprite.pivot.y / mySR.sprite.rect.height),
                mySR.sprite.pixelsPerUnit);

            UpdateStaf();
        }

        public void ChangePPU(float inPPU)
        {
            mySR.sprite = Sprite.Create(mySR.sprite.texture, new Rect(0, 0, mySR.sprite.texture.width, mySR.sprite.texture.height),
                new Vector2(mySR.sprite.pivot.x / mySR.sprite.rect.width, mySR.sprite.pivot.y / mySR.sprite.rect.height), inPPU);
            UpdateStaf();
        }

        public Sprite GetSprite()
        {
            return mySR.sprite;
        }

        public void SetColor(Color inColor)
        {
            mySR.color = inColor;
        }

        public Color GetColor()
        {
            return mySR.color;
        }

        public virtual void OnRightMouseButtonDown()
        {

        }

        public virtual void ActivateBlank()
        {
            Vector3 pos = transform.position;
            pos.z = -3;
            transform.position = pos;
        }

        public virtual void HideBlank()
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }

        protected virtual void UpdateStaf()
        { }

        public virtual Blank CloneBlank()
        {
            return null;
        }

        public virtual bool DeleteBlank()
        {
            return false;
        }

        public void OnMouseDown()
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                LevelCreatorManager.Instance.LeftClickOnObject(this);
            }
        }

    }
}
