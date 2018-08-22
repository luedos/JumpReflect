using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace LevelCreator
{
    public class FolderChooserButton : MonoBehaviour
    {


        string myPath;

        [Header("DIY")]
        public UnityEngine.UI.Image myImg;
        public RectTransform myTransform;
        public FolderChooser myFC;
        public GameObject BackImage;
        public UnityEngine.UI.Text myText;
        public Sprite FolderIcon;
        public Sprite FileIcon;

        bool isFolder;

        float Timer = 0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Timer > 0f)
            {
                Timer -= Time.deltaTime;
                if (Timer < 0)
                    Timer = 0;
            }
        }

        

        public void UpdatePath(string inPath, bool inFolder)
        {
            myPath = inPath;

            myText.text = System.IO.Path.GetFileName(inPath);

            isFolder = inFolder;

            if (isFolder)
                myImg.sprite = FolderIcon;
            else
                myImg.sprite = FileIcon;
        }

        public void SetAsActive()
        {
            if (Timer > 0)
            {
                myFC.MoveToFolder();
                return;
            }

            if(isFolder)
                Timer = 0.8f;

            myFC.DeactivateButtons();

            if (string.IsNullOrEmpty(myPath))
                return;

            if (isFolder)
                myFC.SetFolder(System.IO.Path.GetFileName(myPath));
            else
                myFC.SetFile(System.IO.Path.GetFileName(myPath));

            BackImage.SetActive(true);
        }

        public void Deactivate()
        {
            BackImage.SetActive(false);
        }
    }
}
