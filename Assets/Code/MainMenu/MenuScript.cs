using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MainMenues
{
    public class MenuScript : MonoBehaviour
    {

        public Animation OpenCloseAnim;
        public string AnimName;

        protected float TimerToInactive;

        private void Update()
        {
            if (TimerToInactive > 0)
            {
                TimerToInactive -= Time.deltaTime;
                if (TimerToInactive < 0)
                {
                    TimerToInactive = 0f;
                    gameObject.SetActive(false);
                }
            }
        }

        public virtual void OpenMenu(bool HardMode = false)
        {
            gameObject.SetActive(true);

            TimerToInactive = 0f;

            if (!OpenCloseAnim)
                return;
            if (HardMode)
                OpenCloseAnim[AnimName].time = OpenCloseAnim[AnimName].length;
            else
                OpenCloseAnim[AnimName].time = 0f;


            OpenCloseAnim[AnimName].speed = 1f;
            OpenCloseAnim.Play();
        }

        public virtual void CloseMenu(bool HardMode = false)
        {
            if (HardMode || !OpenCloseAnim)
            {
                gameObject.SetActive(false);
                TimerToInactive = 0f;
                return;
            }

            TimerToInactive = OpenCloseAnim[AnimName].time = OpenCloseAnim[AnimName].length;
            OpenCloseAnim[AnimName].speed = -1.5f;
            OpenCloseAnim.Play();
        }

        public virtual void UpdateHUD() { }


    }
}