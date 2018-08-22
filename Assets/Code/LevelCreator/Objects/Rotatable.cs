using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelCreator
{
    public class Rotatable : MonoBehaviour, IDragHandler
    {

        public SpriteRenderer SpriteObject;
        public Transform TransformToRot;

        Camera myCamera;

        bool isDrag = false;

        private void Start()
        {
            myCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 newVec = new Vector3();

            newVec.z = -1f;

            newVec.y = SpriteObject.sprite.bounds.extents.y * SpriteObject.transform.localScale.y + 0.3f;


            //transform.position = newVec;
            transform.localPosition = newVec;

            if (isDrag)
            {
                Vector3 dir = myCamera.ScreenToWorldPoint(Input.mousePosition) - TransformToRot.position;
                dir.z = 0;
                TransformToRot.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            }
        }

        private void OnMouseDown()
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                isDrag = true;
            }
        }


        private void OnMouseUp()
        {
            isDrag = false;
        }
        //private void OnMouseDrag()
        //{
        //
        //}

        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}