using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour {

    private void Start()
    {
        MainMenues.MainMenu.DoSomething();
    }

    public void ReturnMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level_MainMenu");
    }



    
}
