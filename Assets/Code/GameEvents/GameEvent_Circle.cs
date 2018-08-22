using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProccess
{
    public class GameEvent_Circle : GameEvent
    {



        public float aLength = 15;
        public int NumOfBalls = 8;

        [Header("HUD Elements")]
        public HUD_Emergency HUD_Element;




        public override void CallEvent()
        {

            if (DataManipulators. DataPool.Instance == null)
                return;

            Vector3 pos;
            pos.z = -1f;

            float angle;

            for (int i = 0; i < NumOfBalls; ++i)
            {
                angle = 360f * i / (NumOfBalls);

                pos.x = -aLength * Mathf.Sin(Mathf.Deg2Rad * angle);
                pos.y = aLength
                    * Mathf.Cos(Mathf.Deg2Rad * angle);


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

                    localComp.BallSpawn(pos, Quaternion.Euler(0, 0, angle + 180f));


                }

            }

            HUD_Element.gameObject.SetActive(true);
        }

    }
}