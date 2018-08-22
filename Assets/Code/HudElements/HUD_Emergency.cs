using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Emergency : MonoBehaviour {

    public float EmergencyTime = 1f;


    private float Timer;
    private Image[] MyHudImages;

	// Use this for initialization
	void Start () {
        MyHudImages = gameObject.GetComponentsInChildren<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Timer > 0f)
        {
            Timer -= Time.deltaTime;

            Color localColor;

            for (int i = MyHudImages.Length - 1; i > -1; --i)
            {
                localColor = MyHudImages[i].color;
                localColor.a = Mathf.Sin(Time.time * 5) * 0.3f + 0.7f;
                MyHudImages[i].color = localColor;
            }

            if (Timer < 0f)
            {
                Timer = 0f;                

                gameObject.SetActive(false);
            }
        }

	}

    private void OnEnable()
    {
        Timer = EmergencyTime;
    }
}
