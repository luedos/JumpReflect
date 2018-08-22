using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelCreator
{
    public class GameEventButton : SubmenuButton
    {

        public ObjectsMenu myObjMenu;
        public EventsData myEventData;

        public Color OpenedColor;
        public Color ClosedColor;
        [Header("!!!DIY!!!")]
        public RectTransform myRect;
        public UnityEngine.UI.Text myText;
        public UnityEngine.UI.Image myImage;
        public void ChangeEventData(EventsData inData)
        {
            myEventData = inData;
            myImage.color = ClosedColor;
            UpdateName();
        }

        public void UpdateName()
        {
            if (myMenu.myMenues[0].gameObject.activeSelf && myObjMenu.CurrentEventData == myEventData)
                myImage.color = OpenedColor;
            else
                myImage.color = ClosedColor;

            myText.text = myEventData.myType.ToString();
        }
        private void OnEnable()
        {
            UpdateName();
        }

        public override void OpenMenu()
        {
            myObjMenu.ChangeEventData(myEventData);

            myImage.color = OpenedColor;

            base.OpenMenu();
        }

    }
}