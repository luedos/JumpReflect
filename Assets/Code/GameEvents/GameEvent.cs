using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameProccess
{
    public abstract class GameEvent : MonoBehaviour
    {

        public UnityEvent EventOnCall;

        public abstract void CallEvent();
    }
}