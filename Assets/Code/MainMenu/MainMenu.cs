using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataManipulators;

namespace MainMenues
{
    public class MainMenu : MonoBehaviour
    {

        [Header("Menus")]
        public MenuScript PlayerSelectMenu;
        public MenuScript MapSelectMenu;
        public MenuScript SettingsMenu;
        //public GameObject PlayerCreationMenu;
        //public PlayerStatsMenu PlayerStatsMenu;
        public MenuScript MapsMenu;

        [Header("Main player objs")]
        public Text MainPlayerName;
        public Text MainPlayerScore;
        public Text MainPlayerMap;
        public GameObject MainPlayerInfo;

        [Header("Buttons")]
        public MenuScript BackOnClickPlane;

        public GameObject SomethingThing;

        private static bool Something;
        public static void DoSomething()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level_TestLevel")
                Something = true;
        }

        public enum MenuState
        {
            MainMenu = 0,
            PlayerSelect = 1,
            MapSelect = 2,
            Settings = 3
        }

        MenuState CurrentState = MenuState.MainMenu;

        public void UpdateHUD()
        {
            PlayerSelectMenu.UpdateHUD();

            string strMainPlayer = MainData.GetInstance().GetMainPlayer();

            if (strMainPlayer != null)
            {
                MainPlayerInfo.SetActive(true);

                MainPlayerName.text = strMainPlayer;

                string mapName;
                MainPlayerScore.text = MainData.GetInstance().GetMaxScore(strMainPlayer, out mapName).ToString();

                if (mapName == null)
                    MainPlayerMap.text = "None yet";
                else
                    MainPlayerMap.text = mapName;

            }
            else
                MainPlayerInfo.SetActive(false);

            MapsMenu.UpdateHUD();
        }

        private void Start()
        {
            if(Something)
            {
                SomethingThing.SetActive(true);
                Something = false;
            }

            UpdateHUD();

            MainData.GetInstance().ReadLevelData();
        }

        public void ChangeMenu(MenuState onState, bool CloseLastHard = false)
        {
            switch (onState)
            {
                case MenuState.MapSelect:
                    if (MainData.GetInstance().GetMainPlayer() == null)
                    {
                        ChangeMenu(MenuState.PlayerSelect, true);
                        InfoMessage.ShowMessage("No current player");
                        return;
                    }

                    MapSelectMenu.OpenMenu();
                    BackOnClickPlane.OpenMenu();
                    break;
                case MenuState.PlayerSelect:
                    PlayerSelectMenu.OpenMenu();
                    BackOnClickPlane.OpenMenu();
                    break;
                case MenuState.Settings:
                    SettingsMenu.OpenMenu();
                    break;
            }

            HideMenu(CurrentState, CloseLastHard);
            CurrentState = onState;
        }

        public void ChangeMenu(int inState)
        {
            if (inState < 4 && inState >= 0)
                ChangeMenu((MenuState)inState);
        }

        public void HideMenu(MenuState inState, bool closeHard)
        {
            switch (inState)
            {
                case MenuState.MapSelect:
                    MapSelectMenu.CloseMenu(closeHard);
                    BackOnClickPlane.CloseMenu(closeHard);
                    break;
                case MenuState.PlayerSelect:
                    PlayerSelectMenu.CloseMenu(closeHard);
                    BackOnClickPlane.CloseMenu(closeHard);
                    break;
                case MenuState.Settings:
                    SettingsMenu.CloseMenu(closeHard);
                    break;
            }
        }


        public bool AddPlayer(string inName)
        {
            if (inName.Length > 15 || inName == "" || inName == string.Empty)
                return false;

            if (!MainData.GetInstance().AddPlayer(inName))
                return false;

            UpdateHUD();

            return true;

        }

        public void ChangePlayer(string inPlayer)
        {
            MainData.GetInstance().ChangeMainPlayer(inPlayer);
            UpdateHUD();
        }

        public void DeletePlayer(string inPlayer)
        {
            MainData.GetInstance().DeletePlayer(inPlayer);
            UpdateHUD();
        }

        public void OpenLevelCreator()
        {
            LevelCreator.LevelCreatorManager.FileName = "";
            LevelCreator.LevelCreatorManager.SavePath = "";

            UnityEngine.SceneManagement.SceneManager.LoadScene("Level_LevelCreator");
        }


        public void ExitGame()
        {
            Application.Quit();
        }


        [Header("For test")]
        public Button testButton;
        private int TestInt = 0;

        public void PressSecretButton()
        {
            if (!testButton.gameObject.activeSelf)
                if (++TestInt > 10)
                {
                    InfoMessage.ShowMessage("Testing mode activated", 1.5f);
                    testButton.gameObject.SetActive(true);
                    MainData.isTestingMode = true;
                }
        }


    }
}