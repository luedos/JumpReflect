using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace LevelCreator
{
    public class EventOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float HoverTime = 0.7f;
        public UnityEngine.Events.UnityEvent myEvent;

        float Timer = 0f;

        private void Update()
        {
            if (Timer > 0f)
            {
                Timer -= Time.deltaTime;
                if (Timer < 0f)
                {
                    Timer = 0f;
                    myEvent.Invoke();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Timer = HoverTime;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Timer = 0f;
        }
    }
}