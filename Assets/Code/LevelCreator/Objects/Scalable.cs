using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelCreator
{
    public class Scalable : MonoBehaviour, IDragHandler
    {

        public SpriteRenderer ObjectToScale;
        public Transform parentTansform;

        public HardOutline myOutline;

        public bool ScaleEqualy = false;

        Camera myCamera;

        bool isDrag = false;

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void Start()
        {
            myCamera = Camera.main;

            

            Vector3 newVec = new Vector3();
            newVec.z = -1;
            newVec.x = ObjectToScale.sprite.bounds.extents.x * ObjectToScale.transform.localScale.x;
            newVec.y = ObjectToScale.sprite.bounds.extents.y * ObjectToScale.transform.localScale.y;

            Transform localParent = ObjectToScale.transform.parent;

            if (localParent != null)
            {
                newVec.x *= localParent.localScale.x;
                newVec.y *= localParent.localScale.y;
            }
            if (ScaleEqualy)
            {
                Vector3 ScaleVector = new Vector3(ObjectToScale.transform.localScale.x, ObjectToScale.transform.localScale.y, 1);



                if (newVec.x > newVec.y)
                {
                    newVec.y = newVec.x;
                    ScaleVector.y = ObjectToScale.transform.localScale.x * localParent.transform.localScale.x / localParent.transform.localScale.y;
                }
                else
                {
                    newVec.x = newVec.y;
                    ScaleVector.x = ObjectToScale.transform.localScale.y * localParent.transform.localScale.y / localParent.transform.localScale.x;
                }

                //if (localParent != null)
                //{
                //    ScaleVector.x *= localParent.transform.localScale.x;
                //    ScaleVector.y *= localParent.transform.localScale.y;
                //}

                ObjectToScale.transform.localScale = ScaleVector;
            }

            if (myOutline != null)
                myOutline.Start();

            transform.localPosition = newVec;
        }

        private void OnMouseDown()
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                isDrag = true;
            }
        }

        private void Update()
        {
            if (!isDrag)
                return;

            Vector3 newVec = transform.InverseTransformDirection(myCamera.ScreenToWorldPoint(Input.mousePosition) - parentTansform.position);
            newVec.z = -1;

            if (newVec.x < 0.1f)
                newVec.x = 0.1f;
            if (newVec.y < 0.1f)
                newVec.y = 0.1f;

            if (ScaleEqualy)
            {
                if (newVec.x > newVec.y)
                    newVec.y = newVec.x;
                else
                    newVec.x = newVec.y;
            }

            transform.localPosition = newVec;

            Vector3 ScaleVec = new Vector3();

            ScaleVec.x = newVec.x / ObjectToScale.sprite.bounds.extents.x;
            ScaleVec.y = newVec.y / ObjectToScale.sprite.bounds.extents.y;
            ScaleVec.z = 1;

            Transform localParent = ObjectToScale.transform.parent;

            if (localParent != null)
            {
                ScaleVec.x /= localParent.transform.localScale.x;
                ScaleVec.y /= localParent.transform.localScale.y;
            }
            ObjectToScale.transform.localScale = ScaleVec;

            if (myOutline != null)
                myOutline.UpdateScale();
        }

        public Vector2 GetScale()
        {
            return ObjectToScale.transform.localScale;

        }

        private void OnDisable()
        {
            isDrag = false;
        }

        private void OnMouseUp()
        {
            isDrag = false;

        }

        //private void OnMouseDrag()
        //{
        //    
        //}
    }
}