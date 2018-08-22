using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DataManipulators
{
    public class DataPool : MonoBehaviour
    {

        public enum E_ObjectType
        {
            SmallBall,
            SideBall
        }

        [System.Serializable]
        public struct SpawnStruct
        {
            public GameObject SpawnObj;
            public E_ObjectType ObjType;
            public int SpawnOnStart;
        }

        public SpawnStruct[] ObjectSamples;

        public static DataPool Instance { get { return instance; } }

        static DataPool instance;

        Dictionary<E_ObjectType, List<GameObject>> myLists = new Dictionary<E_ObjectType, List<GameObject>>();

        private void Start()
        {
            if (instance == null || instance == this)
                instance = this;
            else
            {
                Destroy(this);
                return;
            }

            for (int i = 0; i < ObjectSamples.Length; ++i)
            {
                myLists.Add(ObjectSamples[i].ObjType, new List<GameObject>());

                for (int ii = 0; ii < ObjectSamples[i].SpawnOnStart; ++ii)
                {
                    GameObject localObj = Instantiate(ObjectSamples[i].SpawnObj);
                    localObj.SetActive(false);

                    myLists[ObjectSamples[i].ObjType].Add(localObj);
                }
            }

        }

        public GameObject GetInactiveObject(E_ObjectType inType)
        {

            List<GameObject> localList = myLists[inType];

            if (localList != null)
            {
                for (int i = 0; i < localList.Count; ++i)
                    if (!localList[i].activeSelf)
                        return localList[i];

                GameObject localObj = null;

                if (localList.Count > 0)
                {
                    localObj = Instantiate(localList[0]);
                    localObj.SetActive(false);
                    localList.Add(localObj);
                    return localObj;
                }
                else
                {
                    for (int i = 0; i < ObjectSamples.Length; ++i)
                        if (ObjectSamples[i].ObjType == inType)
                        {
                            localObj = Instantiate(localList[0]);
                            localObj.SetActive(false);
                            localList.Add(localObj);
                            return localObj;
                        }


                }


            }

            return null;
        }
    }
}
