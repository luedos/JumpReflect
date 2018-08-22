using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace GameProccess
{
    public class GameManager : MonoBehaviour
    {


        [Header("HUD")]
        public Text PointsText;
        public Text BestScoreText;
        public Text HPText;
        public GameObject myPauseMenu;
        public GameObject EndScreen;

        [Header("On new round")]
        public bool NewGameOnStart = true;
        public Transform spawnTransform;
        public Player myPlayer;

        [Header("Events")]
        public GameEvent[] MyEvents;
        public float AvarageEventTime = 40f;
        public float DeltaEventTime = 5f;

        [Header("Gamepaly")]
        public float TimeSpeed = 1f;
        public float ScoreMultiplyer = 1f;


        public bool TestMode = false;

        private float Points = 0f;
        private static GameManager StaticGM;
        private float EventTimer;

        private int lastEvent = 0;

        public static GameManager Instance { get { return StaticGM; } }



        public GameManager()
        {
            if (!StaticGM)
                StaticGM = this;
            else
                Destroy(this);
        }

        // Use this for initialization
        void Start()
        {
            PointsText.text = Mathf.FloorToInt(Points).ToString();

            if (!myPlayer)
                print("out of Player in : " + name);

            Time.timeScale = TimeSpeed;

            if (NewGameOnStart)
                NewGame();
        }

        // Update is called once per frame
        void Update()
        {

            if (EventTimer > 0f)
            {
                EventTimer -= Time.deltaTime;
                if (EventTimer <= 0)
                {
                    EventTimer = AvarageEventTime + Random.Range(-DeltaEventTime, DeltaEventTime);

                    // Calling some event
                    CallGameEvent();
                }
            }

            if (Input.GetButtonDown("Cancel"))
                myPauseMenu.SetActive(!myPauseMenu.activeSelf);

        }

        public void NewGame()
        {
            EndScreen.SetActive(false);
            if (myPlayer)
            {
                myPlayer.PlayerSpawn(spawnTransform.position);
                UpdatePlayerStats(myPlayer);
            }

            Points = 0;
            PointsText.text = "0";


            if (!TestMode)
            {
                DataManipulators.MainData.LevelProps localLP = DataManipulators.MainData.GetInstance().GetCurrentLevel();

                BestScoreText.text = DataManipulators.MainData.GetInstance().GetMaxScore(
                                            DataManipulators.MainData.GetInstance().GetMainPlayer(),
                                            localLP.LevelName, localLP.LevelHash).ToString();
            }

            Ball[] myBalls = GameObject.FindObjectsOfType<Ball>();
            for (int i = myBalls.Length - 1; i > -1; --i)
                myBalls[i].BallDestroy();

            HUD_Emergency[] myEms = GameObject.FindObjectsOfType<HUD_Emergency>();
            for (int i = myEms.Length - 1; i > -1; --i)
                myEms[i].gameObject.SetActive(false);

            EventTimer = AvarageEventTime + Random.Range(-DeltaEventTime, DeltaEventTime);
        }

        public void GameOver(Player inPlayer)
        {
            myPlayer = inPlayer;

            if (!TestMode)
            {
                DataManipulators.MainData.LevelProps localLP = DataManipulators.MainData.GetInstance().GetCurrentLevel();
                DataManipulators.MainData.GetInstance().SetMaxScore((int)Points, localLP.LevelName, localLP.LevelHash);
            }

            EndScreen.SetActive(true);
        }

        public void AddPoints(float inPoints)
        {
            Points += inPoints * ScoreMultiplyer;

            PointsText.text = Mathf.FloorToInt(Points).ToString();


            if (!TestMode)
            {
                DataManipulators.MainData.LevelProps localLP = DataManipulators.MainData.GetInstance().GetCurrentLevel();


                int MaxPoints = DataManipulators.MainData.GetInstance().GetMaxScore(
                                            DataManipulators.MainData.GetInstance().GetMainPlayer(),
                                            localLP.LevelName, localLP.LevelHash);

                if (Points > MaxPoints)
                {
                    DataManipulators.MainData.GetInstance().SetMaxScore((int)Points, localLP.LevelName, localLP.LevelHash);

                    BestScoreText.text = Mathf.FloorToInt(Points).ToString();
                }
            }
        }

        public int GetScore()
        {
            return Mathf.FloorToInt(Points);
        }

        public void UpdatePlayerStats(Player inPlayer)
        {
            if (inPlayer)
                HPText.text = inPlayer.GetHP().ToString();
        }

        private void OnDestroy()
        {
            if (StaticGM == this)
                StaticGM = null;
        }

        public virtual void CallGameEvent()
        {
            if (MyEvents.Length == 0)
                return;

            int NextEvent = lastEvent;

            if (MyEvents.Length > 1)
                while (NextEvent == lastEvent)
                {
                    NextEvent = Random.Range(0, MyEvents.Length);
                }

            lastEvent = NextEvent;

            MyEvents[NextEvent].CallEvent();

        }

        public void ReturnToMainManu()
        {
            SceneManager.LoadScene("Level_MainMenu");
        }


    }
}