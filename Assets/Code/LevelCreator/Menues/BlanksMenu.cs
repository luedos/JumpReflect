using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class BlanksMenu : MonoBehaviour
    {

        public Image WallsButton;
        public Image MBButton;
        public Text WallButtonText;
        public Text MBButtonText;


        public Color OpenButtonColor;
        public Color CloseButtonColor;

        public RectTransform Content;

        public BlanksMenuButton PlayerButton;

        public BlanksMenuButton buttonSample;

        public Animation myAnim;
        public string AnimName;

        public Image myImg;

        public bool StartClosed = true;

        //BlanksMenuButton myCurrent;

        float CloseTimer = 0f;

        List<BlanksMenuButton> myButtons = new List<BlanksMenuButton>();

        bool openWalls = false;
        bool openMBs = false;

        //public BlanksMenuButton GetCurrent { get { return myCurrent; } }

        private void OnEnable()
        {

            UpdateShot();
        }

        public void OpenCloseMenu()
        {


            if (!StartClosed)
            {


                CloseMenu();
            }
            else
            {
                myImg.rectTransform.rotation = Quaternion.Euler(0, 0, 0);

                CloseTimer = 0;
                myAnim[AnimName].speed = 1f;
                myAnim[AnimName].time = 0;

                gameObject.SetActive(true);
                StartClosed = false;
                myAnim.Play();
            }
        }

        public void CloseMenu()
        {
            if (StartClosed)
                return;

            myImg.rectTransform.rotation = Quaternion.Euler(0, 0, 180);

            CloseTimer = myAnim[AnimName].length;

            myAnim[AnimName].speed = -1f;
            myAnim[AnimName].time = myAnim[AnimName].length;

            StartClosed = true;

            myAnim.Play();
        }

        public void UpdateShot()
        {
            List<Blank_Wall> WallList = LevelCreatorManager.Instance.WallList;
            List<Blank_MB> MBList = LevelCreatorManager.Instance.MBList;

            RectTransform WallButtonTrans = WallsButton.rectTransform;
            RectTransform MBButtonTrans = MBButton.rectTransform;

            float buttonHeight = buttonSample.myImage.rectTransform.rect.height;

            Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, WallButtonTrans.rect.height + MBButtonTrans.rect.height +
                (openWalls ? WallList.Count * buttonHeight : 0) +
                (openMBs ? MBList.Count * buttonHeight : 0) +
                20);

            WallButtonTrans.anchoredPosition = new Vector2(0, -10);

            float CurrentHeight = WallButtonTrans.rect.height + 10;

            int i = 0;

            if (myButtons.Capacity < WallList.Count + MBList.Count)
                myButtons.Capacity = WallList.Count + MBList.Count;

            int WallsCount = 0;

            if (openWalls)
            {
                WallsButton.color = OpenButtonColor;



                for (; i < myButtons.Count && i < WallList.Count; ++i)
                {
                    if (!WallList[i].gameObject.activeSelf)
                    {
                        myButtons[i].gameObject.SetActive(false);
                        continue;
                    }
                    myButtons[i].myBlank = WallList[i];
                    myButtons[i].myImage.rectTransform.anchoredPosition = new Vector2(0, -CurrentHeight);
                    myButtons[i].gameObject.SetActive(true);

                    CurrentHeight += buttonHeight;

                    WallsCount++;
                }

                if (i < WallList.Count)
                {
                    for (; i < WallList.Count; ++i)
                    {
                        myButtons.Add(Instantiate<BlanksMenuButton>(buttonSample, Content));
                        myButtons[i].myMenu = this;


                        if (!WallList[i].gameObject.activeSelf)
                        {

                            myButtons[i].gameObject.SetActive(false);
                            continue;
                        }


                        myButtons[i].myBlank = WallList[i];
                        myButtons[i].myImage.rectTransform.anchoredPosition = new Vector2(0, -CurrentHeight);
                        myButtons[i].gameObject.SetActive(true);

                        CurrentHeight += buttonHeight;
                        WallsCount++;
                    }
                }


            }
            else
            {
                WallsButton.color = CloseButtonColor;


            }



            MBButtonTrans.anchoredPosition = new Vector2(0, -CurrentHeight);
            CurrentHeight += MBButton.rectTransform.rect.height;


            WallsCount = openWalls ? WallList.Count : 0;


            int MBCount = 0;
            if (openMBs)
            {
                MBButton.color = OpenButtonColor;

                for (; i < WallsCount + MBList.Count && i < myButtons.Count; ++i)
                {
                    if (!MBList[i - WallsCount].gameObject.activeSelf)
                    {
                        myButtons[i].gameObject.SetActive(false);
                        continue;
                    }

                    myButtons[i].myBlank = MBList[i - WallsCount];
                    myButtons[i].myImage.rectTransform.anchoredPosition = new Vector2(0, -CurrentHeight);
                    myButtons[i].gameObject.SetActive(true);

                    CurrentHeight += buttonHeight;
                    MBCount++;
                }

                if (i < WallsCount + MBList.Count)
                {
                    for (; i < WallsCount + MBList.Count; ++i)
                    {
                        myButtons.Add(Instantiate<BlanksMenuButton>(buttonSample, Content));

                        if (!MBList[i - WallsCount].gameObject.activeSelf)
                        {
                            myButtons[i].gameObject.SetActive(false);
                            continue;
                        }
                        myButtons[i].myMenu = this;
                        myButtons[i].myBlank = MBList[i - WallsCount];
                        myButtons[i].myImage.rectTransform.anchoredPosition = new Vector2(0, -CurrentHeight);
                        myButtons[i].gameObject.SetActive(true);

                        CurrentHeight += buttonHeight;
                        MBCount++;
                    }

                }
            }
            else
                MBButton.color = CloseButtonColor;




            if (i < myButtons.Count)
                for (; i < myButtons.Count; ++i)
                    myButtons[i].gameObject.SetActive(false);

            PlayerButton.myImage.rectTransform.anchoredPosition = new Vector2(0, -CurrentHeight);

            WallsCount = 0;

            for (i = 0; i < WallList.Count; ++i)
                if (WallList[i].gameObject.activeSelf)
                    ++WallsCount;

            //print("Before exeption");

            WallButtonText.text = "Walls (" + WallsCount.ToString() + ")";


            if (MBCount == 0)
            {
                for (i = 0; i < MBList.Count; ++i)
                    if (MBList[i].gameObject.activeSelf)
                        ++MBCount;

                MBButtonText.text = "Main Balls (" + MBCount.ToString() + ")";
            }
            else
                MBButtonText.text = "Main Balls (" + MBCount.ToString() + ")";
            UpdateButtons();
        }

        public void UpdateButtons()
        {
            for (int i = 0; i < myButtons.Count; ++i)
            {
                if (!myButtons[i].gameObject.activeSelf)
                    continue;

                myButtons[i].UpdateShot();
            }

            PlayerButton.UpdateShot();
        }

        public void OpenCloseWalls()
        {
            openWalls = !openWalls;
            UpdateShot();
        }

        public void OpenCloseMBs()
        {
            openMBs = !openMBs;
            UpdateShot();
        }

        // Update is called once per frame
        void Update()
        {
            if (CloseTimer > 0f)
            {
                CloseTimer -= Time.deltaTime;
                if (CloseTimer < 0f)
                {
                    CloseTimer = 0f;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}