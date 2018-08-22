using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProccess
{
    public class GameEvent_Slide : GameEvent
    {

        public float aLength = 15;
        public float bLength = 7;
        public float cLength = 4;
        public int NumOfBalls = 8;

        [Header("HUD Elements")]
        public HUD_Emergency HUD_Left;
        public HUD_Emergency HUD_Right;
        public HUD_Emergency HUD_Top;
        public HUD_Emergency HUD_Bottom;


        public override void CallEvent()
        {
            if (DataManipulators.DataPool.Instance == null)
                return;

            Vector3 pos;
            pos.z = -1f;

            int s = 1, sa = 1;
            int dir = Random.Range(0, 4);


            if (dir > 1)
                sa = -1;
            if (dir == 1 || dir == 2)
                s = -1;

            float angle = 180 + dir * 90f;

            for (int i = 0; i < NumOfBalls; ++i)
            {

                if (dir == 0 || dir == 2)
                {
                    pos.x = s * (NumOfBalls - 1 - 2 * i) * bLength / (2 * NumOfBalls);
                    pos.y = s * aLength + sa * (NumOfBalls - 1 - 2 * i) * cLength / (2 * NumOfBalls);
                }
                else
                {
                    pos.x = s * aLength + sa * (NumOfBalls - 1 - 2 * i) * cLength / (2 * NumOfBalls);
                    pos.y = s * (NumOfBalls - 1 - 2 * i) * bLength / (2 * NumOfBalls);
                }


                GameObject localObj = DataManipulators.DataPool.Instance.GetInactiveObject(DataManipulators.DataPool.E_ObjectType.SideBall);

                if (localObj == null)
                    return;

                Ball localComp = localObj.GetComponent<Ball>();

                if (localComp)
                {
                    SideBall testComp = localComp as SideBall;

                    if (testComp != null)
                    {
                        testComp.LiveTime = 2 * aLength / testComp.Speed;
                    }

                    localComp.BallSpawn(pos, Quaternion.Euler(0, 0, angle));
                }

            }

            switch (dir)
            {
                case 0:
                    HUD_Top.gameObject.SetActive(true);
                    break;
                case 1:
                    HUD_Left.gameObject.SetActive(true);
                    break;
                case 2:
                    HUD_Bottom.gameObject.SetActive(true);
                    break;
                case 3:
                    HUD_Right.gameObject.SetActive(true);
                    break;
            }
        }
    }
}