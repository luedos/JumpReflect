using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    public interface ITextureDataHolder
    {
        TextureData GetTextureData();
        void SetTextureData(TextureData inTD);
        void ChangePivot(Vector2 inPivot);
        void ChangePPU(float PPU);
        Sprite GetSprite();
        void SetColor(Color inColor);
        Color GetColor();
    }

    public class TextureChooser : MonoBehaviour, ITextureHolder
    {

        public RectTransform TexturesContent;
        public TextureButton TBSample;

        public TextureButton SimpleWhiteBoxButton;
        public TextureButton SimpleWhiteCircleButton;

        public ITextureDataHolder myTextureDataHandler;

        public TextureLoader myTextureLoaderMenu;
        public UnityEngine.Events.UnityEvent OnTextureLoaderCloseEvent;

        TextureButton CurrentTB = null;

        List<TextureButton> TexturesList = new List<TextureButton>(5);

        public void SetCurrentTexture(TextureButton textureButton)
        {
            if (CurrentTB != null)
                CurrentTB.SetActivated(false);

            CurrentTB = textureButton;
            CurrentTB.SetActivated(true);
        }

        public void OpenTextureLoader()
        {
            myTextureLoaderMenu.OpenMenu(OnTextureLoaderCloseEvent);
        }

        public void OpenMenu(ITextureDataHolder onTDHandler)
        {
            myTextureDataHandler = onTDHandler;
            gameObject.SetActive(true);
        }

        protected void UpdateShot()
        {
            if (CurrentTB != null)
                CurrentTB.SetActivated(false);

            CurrentTB = null;

            List<TextureData> localD = LevelCreatorManager.Instance.myTextures;

            int m = Mathf.FloorToInt(TexturesContent.rect.width / (TBSample.myRectTransform.rect.width + 25));

            TexturesContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Ceil(((float)localD.Count + 2) / m) * (TBSample.myRectTransform.rect.height + 25) + 25);

            int localI = 0;
            int localm = 1;
            int localn = 0;



            for (; localI < localD.Count; ++localI)
            {
                if (++localm >= m)
                {
                    localm = 0;
                    localn++;
                }

                if (localI >= TexturesList.Count)
                {
                    TexturesList.Add(Instantiate<TextureButton>(TBSample));
                    TexturesList[localI].myTH = this;
                    TexturesList[localI].transform.SetParent(TexturesContent);
                    TexturesList[localI].myRectTransform.localScale = Vector3.one;
                }
                else
                    TexturesList[localI].gameObject.SetActive(true);

                TexturesList[localI].UpdateImage(localI);
                TexturesList[localI].myRectTransform.anchoredPosition = new Vector2((TBSample.myRectTransform.rect.width + 25) * localm + 25, -(TBSample.myRectTransform.rect.height + 25) * localn - 25);

                if (TexturesList[localI].myTextureData.Hash == myTextureDataHandler.GetTextureData().Hash)
                {
                    CurrentTB = TexturesList[localI];
                    CurrentTB.SetActivated(true);
                }


            }

            for (; localI < TexturesList.Count; ++localI)
                TexturesList[localI].gameObject.SetActive(false);

            if (CurrentTB == null)
            {
                if (SimpleWhiteBoxButton.myTextureData.Hash == myTextureDataHandler.GetTextureData().Hash)
                {
                    CurrentTB = SimpleWhiteBoxButton;
                }
                else if (SimpleWhiteCircleButton.myTextureData.Hash == myTextureDataHandler.GetTextureData().Hash)
                    CurrentTB = SimpleWhiteCircleButton;

                if (CurrentTB != null)
                    CurrentTB.SetActivated(true);
            }

        }

        public void SetTexture()
        {
            if (CurrentTB == null)
            {
                InfoMessage.ShowError("No current image choosed");
                return;
            }
            myTextureDataHandler.SetTextureData(CurrentTB.myTextureData);
            gameObject.SetActive(false);
        }

        private void Start()
        {
            SimpleWhiteBoxButton.myTH = this;

            SimpleWhiteCircleButton.myTH = this;

        }

        private void OnEnable()
        {
            if (myTextureDataHandler != null)
                UpdateShot();
        }

    }
}