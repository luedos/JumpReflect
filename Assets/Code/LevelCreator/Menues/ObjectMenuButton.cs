using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace LevelCreator
{
    public class ObjectMenuButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        //public Sprite ImageHolder;
        //public GameObject myObject;

        public E_LevelObjType myObjType;

        //public Image PlaceHolder;
        Blank localObject;

        Camera myCamera;

        // Use this for initialization
        void Start()
        {
            //PlaceHolder = new GameObject();
            //SpriteRenderer localSR = PlaceHolder.AddComponent<SpriteRenderer>();
            //
            //localSR.sprite = ImageHolder;
            //PlaceHolder.gameObject.SetActive(false);



            localObject = LevelCreatorManager.Instance.CreateBlank(myObjType, true);

            myCamera = Camera.main;
        }


        public void OnDrag(PointerEventData eventData)
        {


            //if (!PlaceHolder.gameObject.activeSelf)
            //{
            //    PlaceHolder.gameObject.SetActive(true);
            //    localObject.SetActive(false);
            //}

            //Vector3 localPos = myCamera.ScreenToWorldPoint(Input.mousePosition);
            //localPos.z = -2;

            //PlaceHolder.gameObject.transform.position = Input.mousePosition;
            //}
            //else
            //{
            //    if (!localObject.activeSelf)
            //    {
            //        PlaceHolder.gameObject.SetActive(false);
            //        localObject.SetActive(true);
            //    }
            //

            Vector3 localPos = Input.mousePosition;
            if (localPos.x < 0)
                localPos.x = 0;
            if (localPos.y < 0)
                localPos.y = 0;
            if (localPos.x > myCamera.pixelWidth)
                localPos.x = myCamera.pixelWidth;
            if (localPos.y > myCamera.pixelHeight)
                localPos.y = myCamera.pixelHeight;

            localPos = myCamera.ScreenToWorldPoint(localPos);
            localPos.z = -1;
            //
            localObject.transform.position = localPos;
            //}
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector3 localPos = myCamera.ScreenToWorldPoint(Input.mousePosition);
            localPos.z = -1;

            localObject.transform.position = localPos;


            //PlaceHolder.gameObject.SetActive(false);

            localObject = LevelCreatorManager.Instance.CreateBlank(myObjType, true);
            localObject.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //PlaceHolder.sprite = ImageHolder;
            localObject.gameObject.SetActive(true);
            LevelCreatorManager.Instance.SetAsActiveBlank(localObject, true);
        }
    }
}