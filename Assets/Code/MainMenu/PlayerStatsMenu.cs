using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenues
{
    public class PlayerStatsMenu : MenuScript
    {

        public Text TextPlayerName;
        public Text TextMaxScore;
        public Text TextMapName;

        private string CurrentPlayer;

        public void UpdateInfo(string CharName)
        {
            CurrentPlayer = CharName;

            TextPlayerName.text = CharName;


            string localMapName;
            TextMaxScore.text = DataManipulators.MainData.GetInstance().GetMaxScore(CharName, out localMapName).ToString();



            if (localMapName == null)
                TextMapName.text = "None yet";
            else
                TextMapName.text = localMapName;


        }

        public void ChoosePlayer()
        {
            DataManipulators.MainData.GetInstance().ChangeMainPlayer(CurrentPlayer);
        }

        public void DeletePlayer()
        {
            DataManipulators.MainData.GetInstance().DeletePlayer(CurrentPlayer);
        }

    }
}