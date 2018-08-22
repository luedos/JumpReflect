using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LevelCreator
{
    public class BlanksMenuButton : MonoBehaviour, IPointerClickHandler
    {

        public InputField myInput;
        public Blank myBlank;

        public Image myImage;

        public Color ActivatedColor;
        public Color DeactivatedColor;



        public BlanksMenu myMenu;

        public void OnPointerClick(PointerEventData eventData)
        {

            LevelCreatorManager.Instance.CloseMenues(E_MenuType.All & ~E_MenuType.BlanksMenu);

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (myInput.isFocused)
                    myInput.DeactivateInputField();

                if (!myBlank.Active)
                {
                    LevelCreatorManager.Instance.SetAsActiveBlank(myBlank);
                }
                else
                {
                    myBlank.OnRightMouseButtonDown();
                }
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                Activate(!myBlank.Active, true);
            }
            else
            {
                LevelCreatorManager.Instance.SetAsActiveBlank(myBlank);

            }

        }

        private void OnEnable()
        {
            UpdateShot();

        }

        public void Activate(bool inState, bool WithBlank = false)
        {
            if (inState)
            {
                myImage.color = ActivatedColor;
                myInput.interactable = true;
                if (WithBlank)
                    LevelCreatorManager.Instance.AddActiveBlank(myBlank);

            }
            else
            {
                myInput.interactable = false;
                myImage.color = DeactivatedColor;
                if (WithBlank)
                    LevelCreatorManager.Instance.RemoveActiveBlank(myBlank);
            }
        }

        public void UpdateShot()
        {
            if (myBlank == null)
                return;

            Activate(myBlank.Active);
            myInput.text = myBlank.Name;
        }

        public void RenameBlank(string inString)
        {
            myBlank.Name = myInput.text;
        }


    }
}