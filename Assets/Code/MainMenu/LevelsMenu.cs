using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenues
{
    public class LevelsMenu : MenuScript
    {

        public RectTransform ScrollContent;
        public MapButton LevelButton;

        private MapButton[] myButtons;

        // Use this for initialization
        void Start()
        {
            RectTransform localTrans = LevelButton.GetComponent<RectTransform>();
            if (localTrans)
            {
                Dictionary<int, DataManipulators.MainData.LevelProps> localLevelData = DataManipulators.MainData.GetInstance().myLevelData;

                int[] keys = new int[localLevelData.Count];
                localLevelData.Keys.CopyTo(keys, 0);

                myButtons = new MapButton[localLevelData.Count];

                ScrollContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40f + keys.Length * 150f);
                for (int i = 0; i < keys.Length; ++i)
                {


                    GameObject localButton = Instantiate(LevelButton.gameObject, ScrollContent);
                    localTrans = localButton.GetComponent<RectTransform>();
                    if (localTrans)
                        localTrans.anchoredPosition = new Vector2(0, -(20f + i * 150f));

                    myButtons[i] = localButton.GetComponent<MapButton>();
                    myButtons[i].levelKey = keys[i];

                }

                UpdateHUD();

            }

        }

        public override void UpdateHUD()
        {
            if (myButtons != null)
                for (int i = 0; i < myButtons.Length; ++i)
                    myButtons[i].UpdateInfos();
        }


    }
}