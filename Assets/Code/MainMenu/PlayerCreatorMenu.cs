using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenues
{
    public class PlayerCreatorMenu : MenuScript
    {

        public InputField myInputField;
        public MainMenu myMainMenu;

        public void OnButtonClicked()
        {
            if (myMainMenu.AddPlayer(myInputField.text))
                CloseMenu();
        }

        private void OnEnable()
        {
            myInputField.text = string.Empty;
        }
    }
}