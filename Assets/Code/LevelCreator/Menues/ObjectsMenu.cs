using Assets.SimpleColorPicker.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace LevelCreator
{
    public class ObjectsMenu : MonoBehaviour
    {

        public Animation OpenAnimation;
        public string AnimName;

        public RightClickMenu mySettingsMenu;

        public bool StartClosed = true;

        public Image FlipFlomImg;



        [Header("Level settings objects")]
        public InputField InLevelName;

        public Slider SliderPointsMultiplyer;
        public Slider SliderTimeSpeed;
        public Slider SliderSideBallStartSpawn;
        public Slider SliderSmallBallStartSpawn;
        public Slider SliderCameraSize;
        public Toggle ToggleSnapToPlayer;
        public Slider SliderAverageTime;
        public Slider SliderDeltaTime;
        public Slider SliderSideBallStartSpeed;
        public Slider SliderSmallBallStartSpeed;
        public Slider SliderSmallBallSpeedHitChange;
        public Slider SliderSmallBallReflects;


        public Text TextPointsMultiplyer;
        public Text TextTimeSpeed;
        public Text TextSideBallStartSpawn;
        public Text TextSmallBallStartSpawn;
        public Text TextCameraSize;
        public Text TextAverageTime;
        public Text TextDeltaTime;
        public Text TextSideBallStartSpeed;
        public Text TextSmallBallStartSpeed;
        public Text TextSmallBallSpeedHitChange;
        public Text TextSmallBallReflects;

        [Header("Menues")]
        public Blank_Image SideBallBlank;
        public Blank_Image SmallBallBlank;
        public Blank_Image LevelImage;        
        public ColorPicker myColorPicker;
        public TextureChooser myTC;

        [Header("Events menu")]
        public Slider Slider_EventsA;
        public Slider Slider_EventsB;
        public Slider Slider_EventsC;
        public Slider Slider_EventsNum;

        public Text Text_EventsA;
        public Text Text_EventsB;
        public Text Text_EventsC;
        public Text Text_EventsNum;

        public Dropdown DD_EventType;

        public RectTransform EventButtonsMenu;
        public GameEventButton EventButtonSample;
        List<GameEventButton> EventButtonsList = new List<GameEventButton>();

        EventsData currentEventData;
        public EventsData CurrentEventData { get { return currentEventData; } }


        float ActiveTimer = 0f;

        public void OpenTextureChooserOnSideBall()
        {
            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.ObjectsMenu);

            myColorPicker.gameObject.SetActive(false);
            myTC.OpenMenu(SideBallBlank);
        }

        public void OpenTextureChooserOnSmallBall()
        {
            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.ObjectsMenu);

            myColorPicker.gameObject.SetActive(false);
            myTC.OpenMenu(SmallBallBlank);
        }

        public void OpenTextureChooserOnLevelImage()
        {
            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.ObjectsMenu);

            myColorPicker.gameObject.SetActive(false);
            myTC.OpenMenu(LevelImage);
        }

        public void OpenColorPickerOnSideBall()
        {
            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.ObjectsMenu);

            myTC.gameObject.SetActive(false);
            myColorPicker.OpenOnTextureHolder(SideBallBlank);
            myColorPicker.gameObject.SetActive(true);
        }

        public void OpenColorPickerOnSmallBall()
        {
            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.ObjectsMenu);

            myTC.gameObject.SetActive(false);
            myColorPicker.OpenOnTextureHolder(SmallBallBlank);
            myColorPicker.gameObject.SetActive(true);
        }

        public void OpenColorPickerOnBackground()
        {
            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.ObjectsMenu);

            myTC.gameObject.SetActive(false);
            myColorPicker.OpenOnTextureHolder(LevelCreatorManager.Instance);
            myColorPicker.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (ActiveTimer > 0)
            {
                ActiveTimer -= Time.deltaTime;
                if (ActiveTimer < 0)
                {
                    ActiveTimer = 0f;
                    gameObject.SetActive(false);
                }
            }
        }

        private void Start()
        {
            DD_EventType.ClearOptions();

            DD_EventType.options = new List<Dropdown.OptionData>() { new Dropdown.OptionData("Line"), new Dropdown.OptionData("Slide"), new Dropdown.OptionData("Circle") };

        }

        public void OpenCloseMenu()
        {
            if (StartClosed)
                OpenMenu();
            else
                CloseMenu();
        }

        public void CloseMenu()
        {
            if (StartClosed)
                return;
            mySettingsMenu.CloseAllSubMenues();
            ActiveTimer = OpenAnimation[AnimName].length;
            OpenAnimation[AnimName].time = OpenAnimation[AnimName].length;
            OpenAnimation[AnimName].speed = -1f;
            OpenAnimation.Play();
            StartClosed = true;
            if (FlipFlomImg != null)
                FlipFlomImg.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void OpenMenu()
        {
            if (!StartClosed)
                return;
            gameObject.SetActive(true);
            ActiveTimer = 0f;
            OpenAnimation[AnimName].time = 0;
            OpenAnimation[AnimName].speed = 1f;
            OpenAnimation.Play();
            StartClosed = false;

            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.ObjectsMenu & ~E_MenuType.BlanksMenu);

            if (FlipFlomImg != null)
                FlipFlomImg.transform.rotation = Quaternion.Euler(0, 0, 180f);
        }

        private void OnEnable()
        {
            UpdateData();
        }

        //-------------------------// For Level Settings //-------------------------------

        public void UpdateData()
        {
            SliderPointsMultiplyer.value = LevelCreatorManager.Instance.PointsMultiplyer;
            SliderTimeSpeed.value = LevelCreatorManager.Instance.TimeSpeed;
            SliderSideBallStartSpawn.value = LevelCreatorManager.Instance.SideBallStartSpawn;
            SliderSmallBallStartSpawn.value = LevelCreatorManager.Instance.SmallBallStartSpawn;
            SliderCameraSize.value = LevelCreatorManager.Instance.CameraBorder.transform.localScale.x;
            ToggleSnapToPlayer.isOn = LevelCreatorManager.Instance.SnapCameraToPlayer;
            SliderAverageTime.value = LevelCreatorManager.Instance.AverageTime;
            SliderSideBallStartSpeed.value = LevelCreatorManager.Instance.SideBallStartSpeed;
            SliderSmallBallStartSpeed.value = LevelCreatorManager.Instance.SmallBallStartSpeed;
            SliderSmallBallSpeedHitChange.value = LevelCreatorManager.Instance.SmallBallSpeedHitChange;
            SliderSmallBallReflects.value = LevelCreatorManager.Instance.SmallBallReflects;


            InLevelName.text = LevelCreatorManager.Instance.LevelName;
            TextPointsMultiplyer.text = LevelCreatorManager.Instance.PointsMultiplyer.ToString();
            TextTimeSpeed.text = LevelCreatorManager.Instance.TimeSpeed.ToString();
            TextSideBallStartSpawn.text = LevelCreatorManager.Instance.SideBallStartSpawn.ToString();
            TextSmallBallStartSpawn.text = LevelCreatorManager.Instance.SmallBallStartSpawn.ToString();
            TextCameraSize.text = SliderCameraSize.value.ToString();
            TextAverageTime.text = LevelCreatorManager.Instance.AverageTime.ToString();
            TextSideBallStartSpeed.text = LevelCreatorManager.Instance.SideBallStartSpeed.ToString();
            TextSmallBallStartSpeed.text = LevelCreatorManager.Instance.SmallBallStartSpeed.ToString();
            TextSmallBallSpeedHitChange.text = LevelCreatorManager.Instance.SmallBallSpeedHitChange.ToString();
            TextSmallBallReflects.text = LevelCreatorManager.Instance.SmallBallReflects.ToString();

            if (LevelCreatorManager.Instance.DeltaTime > LevelCreatorManager.Instance.AverageTime)
            {
                TextDeltaTime.text = LevelCreatorManager.Instance.AverageTime.ToString();
                SliderDeltaTime.value = LevelCreatorManager.Instance.AverageTime;
                LevelCreatorManager.Instance.DeltaTime = LevelCreatorManager.Instance.AverageTime;
            }
            else
            {
                TextDeltaTime.text = LevelCreatorManager.Instance.DeltaTime.ToString();
                SliderDeltaTime.value = LevelCreatorManager.Instance.DeltaTime;
            }
            SliderDeltaTime.maxValue = LevelCreatorManager.Instance.AverageTime;

            UpdateEventButtons();
        }

        public void ChangeLevelName(string inName)
        {
            LevelCreatorManager.Instance.LevelName = inName;
        }

        // Change level Image

        public void ChangePointsMultiplyer(float inNum)
        {
            LevelCreatorManager.Instance.PointsMultiplyer = inNum;
            TextPointsMultiplyer.text = inNum.ToString();
        }

        public void ChangeTimeSpeed(float inNum)
        {
            LevelCreatorManager.Instance.TimeSpeed = inNum;
            TextTimeSpeed.text = inNum.ToString();
        }

        public void ChangeSideBallStartSpawn(float inNum)
        {
            LevelCreatorManager.Instance.SideBallStartSpawn = Mathf.FloorToInt(inNum);
            TextSideBallStartSpawn.text = Mathf.FloorToInt(inNum).ToString();
        }

        public void ChangeSmallBallStartSpawn(float inNum)
        {
            LevelCreatorManager.Instance.SmallBallStartSpawn = Mathf.FloorToInt(inNum);
            TextSmallBallStartSpawn.text = Mathf.FloorToInt(inNum).ToString();
        }

        public void ChangeCameraSize(float inNum)
        {
            LevelCreatorManager.Instance.ChangeCameraSize(inNum);
            TextCameraSize.text = inNum.ToString();
        }

        public void ChageSnapToPlayer(bool inNum)
        {
            LevelCreatorManager.Instance.SnapCameraToPlayer = inNum;
        }

        public void ChangeAvaregeTime(float inNum)
        {
            LevelCreatorManager.Instance.AverageTime = inNum;
            TextAverageTime.text = inNum.ToString();
            if (SliderDeltaTime.value > inNum)
            {
                TextDeltaTime.text = inNum.ToString();
                SliderDeltaTime.value = inNum;
            }
            SliderDeltaTime.maxValue = inNum;
        }

        public void ChangeDeltaTime(float inNum)
        {
            LevelCreatorManager.Instance.DeltaTime = inNum;
            TextDeltaTime.text = inNum.ToString();
        }

        public void ChangeSideBallStartSpeed(float inNum)
        {
            LevelCreatorManager.Instance.SideBallStartSpeed = inNum;
            TextSideBallStartSpeed.text = inNum.ToString();
        }
        public void ChangeSmallBallStartSpeed(float inNum)
        {
            LevelCreatorManager.Instance.SmallBallStartSpeed = inNum;
            TextSmallBallStartSpeed.text = inNum.ToString();
        }
        public void ChangeSmallBallSpeedHitChange(float inNum)
        {
            LevelCreatorManager.Instance.SmallBallSpeedHitChange = inNum;
            TextSmallBallSpeedHitChange.text = inNum.ToString();
        }
        public void ChangeSmallBallReflects(float inNum)
        {
            LevelCreatorManager.Instance.SmallBallReflects = Mathf.FloorToInt(inNum);
            TextSmallBallReflects.text = Mathf.FloorToInt(inNum).ToString();
        }

        public void AddEventData()
        {
            LevelCreatorManager.Instance.myEvents.Add(new EventsData());
            UpdateEventButtons();
        }

        public void DeleteCurrentEventData()
        {
            LevelCreatorManager.Instance.myEvents.Remove(currentEventData);
            UpdateEventButtons();
        }

        public void UpdateEventButtons()
        {
            List<EventsData> localData = LevelCreatorManager.Instance.myEvents;
            int i = 0;

            for (; i < localData.Count && i < EventButtonsList.Count; ++i)
            {
                EventButtonsList[i].ChangeEventData(localData[i]);
                EventButtonsList[i].myRect.anchoredPosition = new Vector2(0f, -(i * EventButtonsList[i].myRect.rect.height));
                EventButtonsList[i].gameObject.SetActive(true);
            }


            for (; i < localData.Count; ++i)
            {
                EventButtonsList.Add(Instantiate<GameEventButton>(EventButtonSample, EventButtonsMenu));
                EventButtonsList[i].ChangeEventData(localData[i]);
                EventButtonsList[i].myRect.anchoredPosition = new Vector2(0f, -(i * EventButtonsList[i].myRect.rect.height));
                EventButtonsList[i].gameObject.SetActive(true);
            }

            for (; i < EventButtonsList.Count; ++i)
                EventButtonsList[i].gameObject.SetActive(false);

            EventButtonsMenu.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (localData.Count * EventButtonSample.myRect.rect.height) + 30f);

        }

        public void SoftUpdateEventButtons()
        {
            for (int i = 0; i < EventButtonsList.Count; ++i)
            {
                if (!EventButtonsList[i].gameObject.activeSelf)
                    break;

                EventButtonsList[i].UpdateName();
            }
        }


        public void ChangeEventData(EventsData inData)
        {
            currentEventData = inData;

            UpdateEventData();
        }

        public void UpdateEventData()
        {
            Slider_EventsA.value = currentEventData.a;
            Slider_EventsB.value = currentEventData.b;
            Slider_EventsC.value = currentEventData.c;
            Slider_EventsNum.value = currentEventData.Num;

            Text_EventsA.text = currentEventData.a.ToString();
            Text_EventsB.text = currentEventData.b.ToString();
            Text_EventsC.text = currentEventData.c.ToString();
            Text_EventsNum.text = currentEventData.Num.ToString();

            DD_EventType.value = (int)currentEventData.myType;


            switch (currentEventData.myType)
            {
                case EventsData.E_EventType.Line:
                    Slider_EventsA.interactable = true;
                    Slider_EventsB.interactable = true;
                    Slider_EventsC.interactable = false;
                    Slider_EventsNum.interactable = true;
                    break;
                case EventsData.E_EventType.Slide:
                    Slider_EventsA.interactable = true;
                    Slider_EventsB.interactable = true;
                    Slider_EventsC.interactable = true;
                    Slider_EventsNum.interactable = true;

                    break;
                case EventsData.E_EventType.Circle:
                    Slider_EventsA.interactable = true;
                    Slider_EventsB.interactable = false;
                    Slider_EventsC.interactable = false;
                    Slider_EventsNum.interactable = true;
                    break;
                default:
                    break;
            }

            SoftUpdateEventButtons();
        }

        public void ChageEventDataType(int inEventType)
        {
            currentEventData.myType = (EventsData.E_EventType)inEventType;

            SoftUpdateEventButtons();

            UpdateEventData();
        }

        public void ChangeEvents_A(float inNum)
        {
            currentEventData.a = inNum;
            Text_EventsA.text = inNum.ToString();
        }
        public void ChangeEvents_B(float inNum)
        {
            currentEventData.b = inNum;
            Text_EventsB.text = inNum.ToString();
        }
        public void ChangeEvents_C(float inNum)
        {
            currentEventData.c = inNum;
            Text_EventsC.text = inNum.ToString();
        }
        public void ChangeEvents_Num(float inNum)
        {
            currentEventData.Num = Mathf.FloorToInt(inNum);
            Text_EventsNum.text = Mathf.FloorToInt(inNum).ToString();
        }

        public void SimplyWriteSaves()
        {
            LevelCreatorManager.Instance.WriteSave(null);
        }

        public void SaveForMainMenu()
        {
            LevelCreatorManager.Instance.WriteSave(new UnityEngine.Events.UnityAction(LoadMainMenu));
        }

        public void LoadMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level_MainMenu");
        }

        

    }
}