using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace LevelCreator
{
    [RequireComponent(typeof(RectTransform))]
    public class SubmenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RightClickMenu myMenu;
        public int SubMenuIndex;

        public float HoverTime = 0.7f;

        float Timer = 0f;
        RectTransform myTransform;

        private void Start()
        {
            myTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (Timer > 0f)
            {
                Timer -= Time.deltaTime;
                if (Timer < 0f)
                {
                    Timer = 0f;

                    OpenMenu();
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

        public virtual void OpenMenu()
        {
            RectTransform localTransform = myMenu.OpenSubmenu(SubMenuIndex);

            if (localTransform == null)
                return;

            Rect parentRect = myTransform.root.GetComponent<RectTransform>().rect;

            float xTrans = parentRect.width / Screen.width;
            float yTrans = parentRect.height / Screen.height;

            Vector2 pos = new Vector2(myTransform.position.x * xTrans + myTransform.rect.x + myTransform.rect.width,
                myTransform.position.y * yTrans + myTransform.rect.y + myTransform.rect.height);

            if (pos.x + localTransform.rect.width > parentRect.width)
                pos.x -= myTransform.rect.width + localTransform.rect.width;

            if (pos.y < localTransform.rect.height)
                pos.y = localTransform.rect.height;

            pos.x -= localTransform.rect.x;
            pos.y -= localTransform.rect.y + localTransform.rect.height;

            pos.x /= xTrans;
            pos.y /= yTrans;

            localTransform.position = pos;

        }
    }
}