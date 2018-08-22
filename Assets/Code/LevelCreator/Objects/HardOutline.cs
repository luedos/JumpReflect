using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HardOutline : MonoBehaviour
    {

        public bool UpdateEachTick = false;
        public SpriteRenderer SpriteOutline;
        public float xDelta = 0.3f;
        public float yDelta = 0.3f;

        SpriteRenderer mySR = null;

        // Use this for initialization
        public void Start()
        {
            if (mySR == null)
                mySR = GetComponent<SpriteRenderer>();
            UpdateScale();
        }

        // Update is called once per frame
        void Update()
        {
            if (UpdateEachTick)
                UpdateScale();
        }

        public void UpdateScale()
        {
            if (SpriteOutline == null)
                return;

            if (SpriteOutline.sprite != mySR.sprite)
                mySR.sprite = SpriteOutline.sprite;

            Vector3 Scale = new Vector3();

            Scale.z = 1;
            Scale.x = (xDelta + SpriteOutline.sprite.bounds.size.x * SpriteOutline.transform.localScale.x / SpriteOutline.sprite.pixelsPerUnit) /
                            (mySR.sprite.bounds.size.x * SpriteOutline.transform.localScale.x / mySR.sprite.pixelsPerUnit);

            Scale.y = (yDelta + SpriteOutline.sprite.bounds.size.y * SpriteOutline.transform.localScale.y / SpriteOutline.sprite.pixelsPerUnit) /
                            (mySR.sprite.bounds.size.y * SpriteOutline.transform.localScale.y / mySR.sprite.pixelsPerUnit);


            transform.localScale = Scale;

        }
    }
}