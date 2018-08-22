using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonScript : MonoBehaviour {

    public Image ImageForFun;
    public Text TextForFun;
    public float colorSpeed;
    private Color imgColor;
	// Use this for initialization
	void Start () {
        imgColor.g = 1;
        imgColor.r = 0.5f;
        imgColor.a = 1f;
	}
	
	// Update is called once per frame
	void Update () {
        
        if(imgColor.r > 0 && imgColor.r < 1f)
            if(imgColor.b == 0)
            {
                imgColor.r += Time.deltaTime * colorSpeed;
                if (imgColor.r > 1)
                {
                    imgColor.r = 1;
                    imgColor.g = 0.9999f;
                }
            }
            else
            {
                imgColor.r -= Time.deltaTime * colorSpeed;
                if (imgColor.r < 0)
                {
                    imgColor.r = 0;
                    imgColor.g = 0.0001f;
                }
            }

        if (imgColor.b > 0 && imgColor.b < 1f)
            if (imgColor.g == 0)
            {
                imgColor.b += Time.deltaTime * colorSpeed;
                if (imgColor.b > 1)
                {
                    imgColor.b = 1;
                    imgColor.r = 0.9999f;
                }
            }
            else
            {
                imgColor.b -= Time.deltaTime * colorSpeed;
                if (imgColor.b < 0)
                {
                    imgColor.b = 0;
                    imgColor.r = 0.0001f;
                }
            }

        if (imgColor.g > 0 && imgColor.g < 1f)
            if (imgColor.r == 0)
            {
                imgColor.g += Time.deltaTime * colorSpeed;
                if (imgColor.g > 1)
                {
                    imgColor.g = 1;
                    imgColor.b = 0.9999f;
                }
            }
            else
            {
                imgColor.g -= Time.deltaTime * colorSpeed;
                if (imgColor.g < 0)
                {
                    imgColor.g = 0;
                    imgColor.b = 0.0001f;
                }
            }

        ImageForFun.color = imgColor;
        
        Color test = Color.white - imgColor;
        test.a = 1f;
        TextForFun.color = test;
    }

    public void onButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level_TestLevel");
    }
}
