using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DataManipulators;
using GameProccess;

namespace MainMenues
{
    public class PauseMenu : MenuScript
    {

        public Text ScoreText;
        public Text MaxScoreText;

        public GameObject MainPauseMenu;
        public GameObject Settings;

        public GameObject HUD;

        // Use this for initialization
        void Start()
        {

        }

        public void Exit()
        {
            MainData.GetInstance().WritePlayerData();
            Application.Quit();
        }

        public void GoMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }

        private void OnEnable()
        {
            Time.timeScale = 0f;

            if (HUD)
                HUD.SetActive(false);
            ScoreText.text = GameManager.Instance.GetScore().ToString();

            MainData.LevelProps localLP = MainData.GetInstance().GetCurrentLevel();

            MaxScoreText.text = MainData.GetInstance().GetMaxScore(
                                    MainData.GetInstance().GetMainPlayer(),
                                    localLP.LevelName, localLP.LevelHash).ToString();

            Settings.SetActive(false);
            MainPauseMenu.SetActive(true);
        }

        private void OnDisable()
        {
            GameManager myGM = GameManager.Instance;
            if (myGM)
                Time.timeScale = myGM.TimeSpeed;
            else
                Time.timeScale = 1f;

            if (HUD)
                HUD.SetActive(true);
        }


    }
}