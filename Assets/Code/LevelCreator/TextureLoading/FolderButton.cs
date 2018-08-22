using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class FolderButton : MonoBehaviour
    {

        public RectTransform myRectTransform;
        public Text myNameText;
        public string FolderPath;
        public TextureLoader myTL;

        public void UpdateFolder(string inFolderPath)
        {
            if (!System.IO.Directory.Exists(inFolderPath))
                return;

            FolderPath = inFolderPath;
            myNameText.text = System.IO.Path.GetFileName(System.IO.Path.GetFileName(inFolderPath));
        }

        public void SetPath()
        {
            myTL.MoveToPath(FolderPath);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}