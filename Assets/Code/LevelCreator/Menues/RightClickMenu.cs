using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace LevelCreator
{
    public class RightClickMenu : MonoBehaviour
    {

        public RightClickMenu[] myMenues;
        public UnityEvent OnCloseEvent;
        RightClickMenu current = null;

        int LastMenu = -1;

        public RectTransform OpenSubmenu(int inIndex)
        {
            if (inIndex < myMenues.Length && inIndex > -1)
            {
                if (current != null)
                    current.CloseMenu();

                current = myMenues[inIndex];
                current.gameObject.SetActive(true);

                LastMenu = inIndex;

                return current.GetComponent<RectTransform>();

            }

            return null;
        }

        public void OpenLast()
        {
            if (LastMenu > -1 && LastMenu < myMenues.Length)
            {
                myMenues[LastMenu].gameObject.SetActive(true);
                myMenues[LastMenu].OpenLast();
            }
        }


        protected void SetPos(Vector2 inScreenPos)
        {
            Rect rootRect = transform.root.GetComponent<RectTransform>().rect;

            float xTrans = rootRect.width / Screen.width;
            float yTrans = rootRect.height / Screen.height;

            RectTransform myTrans = GetComponent<RectTransform>();

            inScreenPos.x *= xTrans;
            inScreenPos.y *= yTrans;

            inScreenPos.y -= myTrans.rect.height;

            if (inScreenPos.y < 0)
            {
                inScreenPos.y += myTrans.rect.height;

                if (inScreenPos.y + myTrans.rect.height > rootRect.height)
                    inScreenPos.y = (rootRect.height - myTrans.rect.height) / 2;
            }

            if (inScreenPos.x + myTrans.rect.width > rootRect.width)
                inScreenPos.x -= myTrans.rect.width;

            inScreenPos.x -= myTrans.rect.x;
            inScreenPos.y -= myTrans.rect.y;

            inScreenPos.x /= xTrans;
            inScreenPos.y /= yTrans;

            myTrans.position = inScreenPos;
        }

        public void CloseMenu()
        {
            OnCloseEvent.Invoke();
            gameObject.SetActive(false);
        }

        public void CloseAllSubMenues()
        {
            current = null;
            for (int i = 0; i < myMenues.Length; ++i)
                if (myMenues[i].gameObject.activeSelf)
                    myMenues[i].CloseMenu();
        }

        protected void OnDisable()
        {
            CloseAllSubMenues();
        }
    }
}