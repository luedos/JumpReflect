using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenues
{
    public class Settings : MonoBehaviour
    {

        public Camera ControlledCamera;
        public Dropdown ResolutionDropdown;
        public Toggle FullscreenToggle;
        private Resolution[] myResolutions;



        public void Start()
        {
            myResolutions = Screen.resolutions;

            ResolutionDropdown.ClearOptions();

            List<string> localStrings = new List<string>();

            int currRes = 0;

            for (int i = 0; i < myResolutions.Length; ++i)
            {
                localStrings.Add(myResolutions[i].width.ToString() + "x" + myResolutions[i].height.ToString());

                if (myResolutions[i].width == Screen.currentResolution.width &&
                    myResolutions[i].height == Screen.currentResolution.height)
                    currRes = i;
            }

            ResolutionDropdown.AddOptions(localStrings);
            ResolutionDropdown.value = currRes;
            ResolutionDropdown.RefreshShownValue();

            FullscreenToggle.isOn = Screen.fullScreen;


            if (ControlledCamera != null)
            {
                float hightScale = 9.0f * Screen.width / (16.0f * Screen.height);
                Rect rect = new Rect();

                if (hightScale < 1f)
                {
                    rect.width = 1.0f;
                    rect.height = hightScale;
                    rect.x = 0;
                    rect.y = (1.0f - hightScale) / 2.0f;
                }
                else
                {
                    rect.width = 1f / hightScale;
                    rect.height = 1.0f;
                    rect.x = (1.0f - 1f / hightScale) / 2.0f;
                    rect.y = 0;
                }

                ControlledCamera.rect = rect;
            }


        }

        public void ChangeFullscreen(bool inFullscreen)
        {
            Screen.fullScreen = inFullscreen;
        }

        public void ChangeResolution(int inIndex)
        {
            Screen.SetResolution(myResolutions[inIndex].width, myResolutions[inIndex].height, Screen.fullScreen);

            if (ControlledCamera != null)
            {
                float hightScale = 9.0f * myResolutions[inIndex].width / (16.0f * myResolutions[inIndex].height);
                Rect rect = new Rect();

                if (hightScale < 1f)
                {
                    rect.width = 1.0f;
                    rect.height = hightScale;
                    rect.x = 0;
                    rect.y = (1.0f - hightScale) / 2.0f;
                }
                else
                {
                    rect.width = 1f / hightScale;
                    rect.height = 1.0f;
                    rect.x = (1.0f - 1f / hightScale) / 2.0f;
                    rect.y = 0;
                }

                ControlledCamera.rect = rect;
            }
        }
    }
}