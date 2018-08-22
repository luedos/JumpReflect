using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MainMenues
{
    public class PlayerButton : MonoBehaviour
    {

        public string PlayerName;
        public Text TextName;
        public PlayerStatsMenu myStatsMenu;

        public Text LevelText;
        public Text ScoreText;

        public void UpdateHud()
        {

            TextName.text = PlayerName;

            if (DataManipulators.MainData.GetInstance().GetMainPlayer() == PlayerName)
                TextName.text = PlayerName + "*";

            string LevelName;

            ScoreText.text = DataManipulators.MainData.GetInstance().GetMaxScore(PlayerName, out LevelName).ToString();

            if (LevelName == null)
                LevelText.text = "None yet";
            else
                LevelText.text = LevelName;


        }

        public void ButtonClick()
        {
            if (myStatsMenu != null)
            {
                myStatsMenu.UpdateInfo(PlayerName);
                myStatsMenu.OpenMenu();
            }
        }
    }
}