using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenues
{
    public class PlayerMenu : MenuScript
    {

        public PlayerStatsMenu myPlayerStats;
        public PlayerButton myPlayerButtonPreFab;
        public RectTransform AddButton;
        public RectTransform ButtonsContent;


        List<PlayerButton> myButtons = new List<PlayerButton>();

        public override void UpdateHUD()
        {
            RectTransform localTransform = myPlayerButtonPreFab.GetComponent<RectTransform>();

            if (!localTransform)
                return;

            string[] localStrings = DataManipulators.MainData.GetInstance().GetPlayers();

            int i = 0;


            for (; i < localStrings.Length && i < myButtons.Count; ++i)
            {
                myButtons[i].PlayerName = localStrings[i];
                myButtons[i].UpdateHud();
            }



            if (localStrings.Length != myButtons.Count)
            {
                if (localStrings.Length > myButtons.Count)
                {

                    for (; i < localStrings.Length; ++i)
                    {
                        PlayerButton localPB = Instantiate<PlayerButton>(myPlayerButtonPreFab, ButtonsContent);
                        localPB.PlayerName = localStrings[i];
                        localPB.myStatsMenu = myPlayerStats;
                        localPB.UpdateHud();
                        myButtons.Add(localPB);
                    }
                }
                else
                {

                    for (; i < myButtons.Count;)
                    {

                        Destroy(myButtons[i].gameObject);
                        myButtons.RemoveAt(i);
                    }
                }
            }



            i = 0;
            ButtonsContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 140f + localStrings.Length * 180f);

            Vector2 localVector = new Vector2();

            for (; i < localStrings.Length; ++i)
            {
                localTransform = myButtons[i].GetComponent<RectTransform>();

                localVector.y = -(40f + i * 180f);

                localTransform.anchoredPosition = localVector;

            }

            localVector.y = -(40f + i * 180f);

            AddButton.anchoredPosition = localVector;



        }

    }
}