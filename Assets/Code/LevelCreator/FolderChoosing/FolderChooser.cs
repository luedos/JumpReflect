using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{

    public class FolderChooser : MonoBehaviour
    {

        public RectTransform myContent;

        public FolderChooserButton ButtonSample;

        List<FolderChooserButton> myButtons = new List<FolderChooserButton>();

        public Text PathText;
        public InputField FileText;
        public InputField FolderText;
        public float Space = 15f;
        public Text SinghText;

        public string PathStr;
        public string FolderStr;
        public string FileStr;

        public UnityEngine.Events.UnityEvent onSuccessEvent = new UnityEngine.Events.UnityEvent();

        public bool SaveOrLoad = true;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            UpdateShot();
        }

        public void OpenMenu(bool inSaveOrLoad)
        {
            SaveOrLoad = inSaveOrLoad;

            if (SaveOrLoad)
            {
                FolderText.interactable = true;
                FileText.interactable = true;
                SinghText.text = "Saving file..";
            }
            else
            {
                FolderText.interactable = false;
                FileText.interactable = false;
                SinghText.text = "Loading file..";
            }

            FileStr = "";
            FolderStr = "";

            gameObject.SetActive(true);
        }

        public void UpdateShot()
        {
            int i = 0;

            string[] localFolders;
            if (string.IsNullOrEmpty(PathStr))
            {

                string localSTR = System.IO.Directory.GetCurrentDirectory();

                localFolders = System.IO.Directory.GetDirectories(localSTR);

                for (; i < localFolders.Length; ++i)
                {
                    localFolders[i] = localFolders[i].Remove(0, localSTR.Length + 1);
                }
            }
            else
                localFolders = System.IO.Directory.GetDirectories(PathStr);

            if (myButtons.Capacity < localFolders.Length)
                myButtons.Capacity = localFolders.Length;

            int m = Mathf.FloorToInt(myContent.rect.width / (ButtonSample.myTransform.rect.width + Space));
            

            int localm = 0;
            int localn = 0;

            i = 0;
            for (; i < localFolders.Length && i < myButtons.Count; ++i)
            {
                myButtons[i].Deactivate();
                myButtons[i].UpdatePath(localFolders[i], true);

                if (++localm >= m)
                {
                    localm = 0;
                    localn++;
                }

                myButtons[i].myTransform.anchoredPosition = new Vector2(Space + localm * (ButtonSample.myTransform.rect.width + Space),
                                                                            -(Space + localn * (ButtonSample.myTransform.rect.height + Space)));

                myButtons[i].gameObject.SetActive(true);

            }

            for(; i < localFolders.Length; ++i)
            {
                myButtons.Add(Instantiate<FolderChooserButton>(ButtonSample, myContent));

                myButtons[i].myFC = this;

                myButtons[i].Deactivate();
                myButtons[i].UpdatePath(localFolders[i], true);

                if (++localm >= m)
                {
                    localm = 0;
                    localn++;
                }

                myButtons[i].myTransform.anchoredPosition = new Vector2(Space + localm * (ButtonSample.myTransform.rect.width + Space),
                                                                            -(Space + localn * (ButtonSample.myTransform.rect.height + Space)));

                myButtons[i].gameObject.SetActive(true);
            }


            if (string.IsNullOrEmpty(PathStr))
            {

                string localSTR = System.IO.Directory.GetCurrentDirectory();

                localFolders = System.IO.Directory.GetFiles(localSTR, "*.jrl");

                for (; i < localFolders.Length; ++i)
                {
                    localFolders[i] = localFolders[i].Remove(0, localSTR.Length + 1);
                }
            }
            else
                localFolders = System.IO.Directory.GetFiles(PathStr, "*.jrl");

            //print(localFolders.Length);

            int startCount = i;

            for (; i < myButtons.Count && i < localFolders.Length + startCount; ++i)
            {
                myButtons[i].Deactivate();
                myButtons[i].UpdatePath(localFolders[i - startCount], false);

                if (++localm >= m)
                {
                    localm = 0;
                    localn++;
                }

                myButtons[i].myTransform.anchoredPosition = new Vector2(Space + localm * (ButtonSample.myTransform.rect.width + Space),
                                                                            -(Space + localn * (ButtonSample.myTransform.rect.height + Space)));

                myButtons[i].gameObject.SetActive(true);
            }

            for (; i < localFolders.Length + startCount; ++i)
            {
                myButtons.Add(Instantiate<FolderChooserButton>(ButtonSample, myContent));

                myButtons[i].myFC = this;

                myButtons[i].Deactivate();
                myButtons[i].UpdatePath(localFolders[i - startCount], false);

                if (++localm >= m)
                {
                    localm = 0;
                    localn++;
                }

                myButtons[i].myTransform.anchoredPosition = new Vector2(Space + localm * (ButtonSample.myTransform.rect.width + Space),
                                                                            -(Space + localn * (ButtonSample.myTransform.rect.height + Space)));

                myButtons[i].gameObject.SetActive(true);
            }

            

            for (; i < myButtons.Count; ++i)
                myButtons[i].gameObject.SetActive(false);


            myContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Space + (localn + 1) * (ButtonSample.myTransform.rect.height + Space));


            FolderText.text = FolderStr;
            PathText.text = PathStr;
            FileText.text = FileStr;
        }

        public void OneFolderBack()
        {
            if (string.IsNullOrEmpty(PathStr))
                return;
            else
                PathStr = System.IO.Path.GetDirectoryName(PathStr);

            FolderStr = "";
            FileStr = "";
            UpdateShot();
        }

        public void SetFolder(string inFolder)
        {
            FolderStr = inFolder;
            FolderText.text = inFolder;
        }

        public void SetFile(string inFile)
        {
            FileStr = inFile;
            FileText.text = inFile;
        }

        public void MoveToFolder()
        {
            if (string.IsNullOrEmpty(FolderStr))
                return;

            if (string.IsNullOrEmpty(PathStr))
                PathStr = FolderStr;
            else
                PathStr = PathStr + @"\" + FolderStr;

            FolderStr = "";
            FileStr = "";

            UpdateShot();

            
        }

        

        public void MoveToFolder(string inFolder)
        {
            if (string.IsNullOrEmpty(inFolder))
                return;

            if (string.IsNullOrEmpty(PathStr))
                PathStr = inFolder;
            else
                PathStr = PathStr + @"\" + inFolder;

            FolderStr = "";
            FileStr = "";
            UpdateShot();
        }

        public void DeactivateButtons()
        {
            for (int i = 0; i < myButtons.Count; ++i)
                if (myButtons[i].gameObject.activeSelf)
                    myButtons[i].Deactivate();
                else
                    break;
        }

        public void Accept()
        {
            if (SaveOrLoad)
            {
                
                if(string.IsNullOrEmpty(FileStr) && string.IsNullOrEmpty(FolderStr))
                {
                    InfoMessage.ShowError("Choose folder and file to save in.");
                    UpdateShot();
                    return;
                }

                if (!string.IsNullOrEmpty(FileStr))
                {
                    if (System.IO.Path.GetExtension(FileStr) != ".jrl")
                        LevelCreatorManager.FileName = FileStr + ".jrl";
                    else
                        LevelCreatorManager.FileName = FileStr;
                }
                else
                    LevelCreatorManager.FileName = LevelCreatorManager.Instance.LevelName + ".jrl";

                LevelCreatorManager.SavePath = PathStr + (string.IsNullOrEmpty(FolderStr) ? "" : @"\" + FolderStr);
            }
            else
            {
                if(!System.IO.File.Exists((string.IsNullOrEmpty(PathStr) ? "" : PathStr + @"\") + FileStr))
                {
                    InfoMessage.ShowError("File: \"" + FileStr + "\" doesn't exist");
                    UpdateShot();
                    return;
                }

                LevelCreatorManager.FileName = FileStr;
                LevelCreatorManager.SavePath = PathStr;


            }

            onSuccessEvent.Invoke();

            gameObject.SetActive(false);
        }
    }
}
