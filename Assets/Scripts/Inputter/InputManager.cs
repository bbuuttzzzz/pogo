using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inputter
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager self;

        public InputBinder Binder;

        #region Messages
        public void Awake()
        {
            self = this;
            Binder = new InputBinder();
        }

        public void Update()
        {
            Binder.ResetInputs(); 
            Binder.Update();
        }
        #endregion

        public static bool CheckKey(KeyName key)
        {
            return self.Binder.Keys[key].held;
        }

        public static bool CheckKeyDown(KeyName key)
        {
            return self.Binder.Keys[key].Pressed;
        }

        public static bool CheckKeyUp(KeyName key)
        {
            return self.Binder.Keys[key].Released;
        }

        public static Vector3 CheckAxisSet(AxisSetName name)
        {
            return self.Binder.AxisSets[name].AxisValue;
        }
    }
}