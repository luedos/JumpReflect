using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LevelCreator
{
    public class ClickyRedirect : MonoBehaviour
    {

        public UnityEngine.Events.UnityEvent redirectEvent = new UnityEngine.Events.UnityEvent();

        private void OnMouseDown()
        {
            redirectEvent.Invoke();
        }
    }
}