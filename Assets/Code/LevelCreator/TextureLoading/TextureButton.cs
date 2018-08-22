using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class TextureButton : MonoBehaviour
    {



        public RectTransform myRectTransform;

        public Image myImg;
        public ITextureHolder myTH;
        public GameObject BackGround;
        public Text ImgNameText;

        public TextureData myTextureData;

        public void UpdateImage(int IndexOfTextureDataInLevelCreatorManager)
        {
            myTextureData = LevelCreatorManager.Instance.myTextures[IndexOfTextureDataInLevelCreatorManager];

            float ratio = myTextureData.myTexture.width / myTextureData.myTexture.height;

            if (80 / 60 > ratio)
                myImg.rectTransform.sizeDelta = new Vector2(30 * ratio, 30);
            else
                myImg.rectTransform.sizeDelta = new Vector2(40, 40 / ratio);

            ImgNameText.text = System.IO.Path.GetFileName(myTextureData.Path);
            myImg.sprite = Sprite.Create(myTextureData.myTexture, new Rect(0, 0, myTextureData.myTexture.width, myTextureData.myTexture.height), new Vector2(0.5f, 0.5f));
        }

        public void UpdateImage(string inPath)
        {
            if (!System.IO.File.Exists(inPath) || !inPath.EndsWith(".png"))
                return;

            Texture2D Tex2D;
            byte[] FileData;

            FileData = System.IO.File.ReadAllBytes(inPath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (!Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return;            // If data = readable -> return texture

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(inPath);

            md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

            byte[] contentBytes = System.IO.File.ReadAllBytes(inPath);

            md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);


            myTextureData = new TextureData(Tex2D, inPath, System.BitConverter.ToString(md5.Hash).Replace("-", "").ToLower());

            float ratio = myTextureData.myTexture.width / myTextureData.myTexture.height;

            if (80 / 60 > ratio)
                myImg.rectTransform.sizeDelta = new Vector2(30 * ratio, 30);
            else
                myImg.rectTransform.sizeDelta = new Vector2(40, 40 / ratio);

            ImgNameText.text = System.IO.Path.GetFileName(inPath);
            myImg.sprite = Sprite.Create(myTextureData.myTexture, new Rect(0, 0, myTextureData.myTexture.width, myTextureData.myTexture.height), new Vector2(0.5f, 0.5f));
        }

        public void SetActivated(bool inState)
        {
            if (inState)
                BackGround.SetActive(true);
            else
                BackGround.SetActive(false);
        }

        public void SetCurrentImage()
        {
            myTH.SetCurrentTexture(this);
        }
    }
}