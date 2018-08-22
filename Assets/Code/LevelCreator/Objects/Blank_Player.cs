using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    public class Blank_Player : Blank
    {

        public int PlayerHP = 3;
        public float InvulDamageTime = 1.5f;
        public float InvulHitTime = 0.7f;
        public float HitReloadTime = 0.8f;
        public float HitRadius = 0.5f;

        public GameObject myOutline;

        public override void HideBlank()
        {
            base.HideBlank();
            myOutline.SetActive(false);
            active = false;
        }

        public override void ActivateBlank()
        {
            base.ActivateBlank();
            myOutline.SetActive(true);
            active = true;
        }

        public override void OnRightMouseButtonDown()
        {
            LevelCreatorManager.Instance.OpenMainPlayerMenu(this);
        }


        public override string ToString()
        {
            /*
                MainPlayer: 
                (SpawnPoint = -7.23|1.46, HP = 5, InvulDamageTime = 1.5, InvulHitTime = 0.7, HitReloadTime = 3, HitRadius = 2)
            */

            return  "(SpawnPoint = " + transform.position.x.ToString() + "|" + transform.position.y.ToString() +
                    ", HP = " + PlayerHP.ToString() + ", InvulDamageTime = " + InvulDamageTime.ToString() +
                    ", InvulHitTime = " + InvulHitTime.ToString() + ", HitReloadTime = " + HitReloadTime.ToString() +
                    ", HitRadius = " + HitRadius.ToString() + ")";

        }
    }

    
}