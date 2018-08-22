using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMessage : MonoBehaviour {

    public float DefaultMessageTime = 5f;

    public Animation inAnimation;
    public string AnimationName;

    public Image PanelImage;

    public Color ErrorColor;
    public Color NormalColor;

    public Text inText;

    float Timer = 0f;
    float TurnOffTimer;

    private bool isError = false;
    private string LastMessage = "";

    static InfoMessage myInstance;

    
	// Use this for initialization
	void Start () {
        if(myInstance != null)
        {
            Destroy(this);
            return;
        }

       

        TurnOffTimer = 0.5f;
        myInstance = this;
        DontDestroyOnLoad(this);	
	}
	
    public static void ShowMessage(string inMessage, float inTime = 0f)
    {
        if (myInstance != null)
        {
            myInstance.isError = false;
            myInstance.gameObject.SetActive(true);
            myInstance.Message(inMessage, inTime);
        }
    }

    public static string GetLastMessage()
    {
        if (myInstance == null)
            return null;

        return myInstance.LastMessage;
    }

    public static void ReshowLastMessage()
    {
        if (myInstance != null)
        {
            myInstance.gameObject.SetActive(true);
            myInstance.Message(myInstance.LastMessage, myInstance.DefaultMessageTime);
        }
    }

    public static void ShowError(string inErrorMessage, float inTime = 0f)
    {
        if (myInstance != null)
        {
            myInstance.isError = true;
            myInstance.gameObject.SetActive(true);
            myInstance.Message(inErrorMessage, inTime);
        }
    }

    public void Message(string inMessage, float inTime)
    {
        if (inTime < inAnimation[AnimationName].length)
            Timer = DefaultMessageTime;
        else
            Timer = inTime;

        TurnOffTimer = 0f;

        inText.text = inMessage;

        if (isError)
            PanelImage.color = ErrorColor;
        else
            PanelImage.color = NormalColor;


        inAnimation[AnimationName].time = 0f;
        inAnimation[AnimationName].speed = 1f;        
        inAnimation.Play(AnimationName);

    }

	// Update is called once per frame
	void Update () {
		
        if(Timer > 0)
        {
            Timer -= Time.deltaTime;
            if(Timer < 0)
            {
                Timer = 0f;

                inAnimation[AnimationName].time = inAnimation[AnimationName].length;
                inAnimation[AnimationName].speed = -1f;
                inAnimation.Play(AnimationName);

                TurnOffTimer = inAnimation[AnimationName].length;
            }
        }

        if(TurnOffTimer > 0)
        {
            TurnOffTimer -= Time.deltaTime;
            if(TurnOffTimer < 0)
            {
                TurnOffTimer = 0f;
                gameObject.SetActive(false);
            }
        }

	}
}
