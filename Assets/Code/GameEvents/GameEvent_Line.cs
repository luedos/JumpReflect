using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProccess
{
    public class GameEvent_Line : GameEvent
    {

        public float aLength = 15;
        public float bLength = 7;
        public int NumOfBalls = 8;

        [Header("HUD Elements")]
        public HUD_Emergency HUD_TopRight;
        public HUD_Emergency HUD_TopLeft;
        public HUD_Emergency HUD_BottomRight;
        public HUD_Emergency HUD_BottomLeft;


        public override void CallEvent()
        {
            if (DataManipulators.DataPool.Instance == null)
                return;

            Vector3 pos;
            pos.z = -1f;

            int hs = 1;
            int ws = 1;

            float angle = 119.36f;

            int dir = Random.Range(0, 4);

            if (dir > 1)
            {
                angle += 180f;
                hs = -1;
            }
            if (dir == 1 || dir == 2)
                ws = -1;

            if (hs != ws)
                angle += 121.28f;

            for (int i = 0; i < NumOfBalls; ++i)
            {
                pos.x = ws * (0.871576f * aLength - 0.49026f * bLength * (NumOfBalls - 1 - 2 * i) / (2 * NumOfBalls));
                pos.y = hs * (0.49026f * aLength + 0.871576f * bLength * (NumOfBalls - 1 - 2 * i) / (2 * NumOfBalls));


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
                    HUD_TopRight.gameObject.SetActive(true);
                    break;
                case 1:
                    HUD_TopLeft.gameObject.SetActive(true);
                    break;
                case 2:
                    HUD_BottomLeft.gameObject.SetActive(true);
                    break;
                case 3:
                    HUD_BottomRight.gameObject.SetActive(true);
                    break;
            }
        }
    }
}