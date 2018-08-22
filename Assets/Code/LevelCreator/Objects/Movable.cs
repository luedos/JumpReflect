using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace LevelCreator
{
    public class Movable : MonoBehaviour, IDragHandler, IPointerDownHandler
    {


        public Transform TransformToMove;
        public Rect WorldRect;


        Camera myCamera;
        Vector3 offset;

        bool isDrag = false;

        private void Start()
        {
            myCamera = Camera.main;
        }

        private void OnMouseDown()
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                isDrag = true;
                offset = TransformToMove.position - myCamera.ScreenToWorldPoint(Input.mousePosition);
                offset.z = 0f;
            }
        }

        private void Update()
        {
            if (!isDrag)
                return;

            Vector3 newVec = Input.mousePosition;
            if (newVec.x < 0)
                newVec.x = 0;
            if (newVec.y < 0)
                newVec.y = 0;
            if (newVec.x > myCamera.pixelWidth)
                newVec.x = myCamera.pixelWidth;
            if (newVec.y > myCamera.pixelHeight)
                newVec.y = myCamera.pixelHeight;

            newVec = myCamera.ScreenToWorldPoint(newVec);

            if (WorldRect.width != 0 || WorldRect.y != 0)
            {
                if (newVec.x < WorldRect.x)
                    newVec.x = WorldRect.x;
                if (newVec.y < WorldRect.y)
                    newVec.y = WorldRect.y;
                if (newVec.x > WorldRect.x + WorldRect.width)
                    newVec.x = WorldRect.x + WorldRect.width;
                if (newVec.y > WorldRect.y + WorldRect.height)
                    newVec.y = WorldRect.y + WorldRect.height;
            }

            newVec.z = TransformToMove.position.z;
            TransformToMove.position = newVec + offset;
        }


        private void OnMouseUp()
        {
            isDrag = false;
        }
        //
        //private void OnMouseDrag()
        //{
        //
        //}

        public void OnDrag(PointerEventData eventData)
        {
            print("Drag");


        }

        public void OnPointerDown(PointerEventData eventData)
        {
            print("Down");

        }
    }
}