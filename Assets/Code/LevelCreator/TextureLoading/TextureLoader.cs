using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace LevelCreator
{
    public class TextureLoader : MonoBehaviour, ITextureHolder
    {

        public RectTransform foldersTransform;
        public RectTransform ImageTransform;

        public TextureButton TBSample;
        public FolderButton FBSample;

        public UnityEngine.UI.Text inPathText;

        [HideInInspector] public UnityEvent AfterClose = null;

        TextureButton currentButton;

        string CurrentPath = @"";

        List<TextureButton> TexturesList = new List<TextureButton>();
        List<FolderButton> FoldersList = new List<FolderButton>();

        public void OpenMenu(UnityEvent inEvent)
        {
            AfterClose = inEvent;
            gameObject.SetActive(true);
        }

        public void CloseMenu()
        {
            if (AfterClose != null)
                AfterClose.Invoke();

            gameObject.SetActive(false);
        }

        public void MoveToPath(string inPath)
        {
            if (!System.IO.Directory.Exists(inPath))
                return;
            CurrentPath = inPath;
            UpdateShot();
        }



        public void SetCurrentImage(TextureButton inButton)
        {

        }

        public void OneFolderBack()
        {
            string[] localArr = CurrentPath.Split('\\');

            if (localArr.Length < 2)
                CurrentPath = "";
            else
            {
                //CurrentPath = CurrentPath.TrimEnd(('\\' + localArr[localArr.Length - 1]).ToCharArray());
                localArr[localArr.Length - 1] = "";
                CurrentPath = string.Join(@"\", localArr, 0, localArr.Length - 1);
            }
            UpdateShot();

        }

        public void UpdateShot()
        {
            if (CurrentPath == "")
                inPathText.text = "Root";
            else
                inPathText.text = CurrentPath;

            if (currentButton != null)
                currentButton.SetActivated(false);

            currentButton = null;

            string[] localArr;

            if (CurrentPath == "")
            {
                string localSTR = System.IO.Directory.GetCurrentDirectory();


                localArr = System.IO.Directory.GetDirectories(localSTR);

                for (int i = 0; i < localArr.Length; ++i)
                {
                    localArr[i] = localArr[i].Remove(0, localSTR.Length + 1);
                }
            }
            else
                localArr = System.IO.Directory.GetDirectories(CurrentPath);



            if (FoldersList.Capacity < localArr.Length)
                FoldersList.Capacity = localArr.Length;

            int localI = 0;

            foldersTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localArr.Length * FBSample.myRectTransform.rect.height + 30);

            for (; localI < localArr.Length; ++localI)
            {
                if (localI >= FoldersList.Count)
                {
                    FoldersList.Add(Instantiate<FolderButton>(FBSample));
                    FoldersList[localI].myTL = this;
                    FoldersList[localI].myRectTransform.SetParent(foldersTransform);
                    FoldersList[localI].myRectTransform.localScale = Vector3.one;
                }
                else
                    FoldersList[localI].gameObject.SetActive(true);


                FoldersList[localI].UpdateFolder(localArr[localI]);
                FoldersList[localI].myRectTransform.anchoredPosition = new Vector2(0, -10 - localI * FoldersList[localI].myRectTransform.rect.height);

            }

            for (; localI < FoldersList.Count; ++localI)
            {
                FoldersList[localI].gameObject.SetActive(false);
            }

            localI = 0;


            if (CurrentPath == "")
                localArr = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.png");
            else
                localArr = System.IO.Directory.GetFiles(CurrentPath, "*.png");



            int m = Mathf.FloorToInt(ImageTransform.rect.width / (TBSample.myRectTransform.rect.width + 25));

            ImageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Ceil(((float)localArr.Length) / m) * (TBSample.myRectTransform.rect.height + 25) + 25);

            int localm = 0;
            int localn = 0;

            if (TexturesList.Capacity < localArr.Length)
                TexturesList.Capacity = localArr.Length;

            for (; localI < localArr.Length; ++localI)
            {
                if (localI >= TexturesList.Count)
                {
                    TexturesList.Add(Instantiate<TextureButton>(TBSample));
                    TexturesList[localI].myTH = this;
                    TexturesList[localI].transform.SetParent(ImageTransform);
                    TexturesList[localI].myRectTransform.localScale = Vector3.one;
                }
                else
                    TexturesList[localI].gameObject.SetActive(true);

                TexturesList[localI].UpdateImage(localArr[localI]);
                TexturesList[localI].myRectTransform.anchoredPosition = new Vector2((TBSample.myRectTransform.rect.width + 25) * localm + 25, -(TBSample.myRectTransform.rect.height + 25) * localn - 25);

                if (++localm >= m)
                {
                    localm = 0;
                    localn++;
                }
            }

            for (; localI < TexturesList.Count; ++localI)
                TexturesList[localI].gameObject.SetActive(false);

        }

        private void OnEnable()
        {
            UpdateShot();
        }

        public void SetCurrentTexture(TextureButton textureButton)
        {
            if (currentButton != null)
                currentButton.SetActivated(false);

            currentButton = textureButton;
            textureButton.SetActivated(true);
        }

        public void LoadTexture()
        {
            if (currentButton == null)
            {
                InfoMessage.ShowError("No current images choosed");
                return;
            }

            LevelCreatorManager.Instance.AddTexture(currentButton.myTextureData.Path);
        }
    }
}