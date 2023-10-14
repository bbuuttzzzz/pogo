using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.Levels
{
    public class LevelStateBoolean : LevelStateSubListener
    {
        public LevelState[] PositiveStates;

        public UnityEvent OnEnterPositiveStateInstantly;
        public UnityEvent OnEnterNegativeStateInstantly;

        public UnityEvent OnEnterPositiveState;
        public UnityEvent OnEnterNegativeState;

        protected override void Parent_OnStateChanged(LevelStateChangedArgs e)
        {
            bool newStateIsPositive = StateIsPositive(e.NewState);

            // do nothing if there's an old state and the boolean is unchanged
            if (e.OldState != null && newStateIsPositive == StateIsPositive(e.OldState.Value))
            {
                return;
            }

            SetBoolean(newStateIsPositive, e.Instant);
        }

        private void SetBoolean(bool value, bool instant)
        {
            if (value)
            {
                if (instant)
                {
                    OnEnterPositiveStateInstantly?.Invoke();
                }
                else
                {
                    OnEnterPositiveState?.Invoke();
                }
            }
            else
            {

                if (instant)
                {
                    OnEnterNegativeStateInstantly?.Invoke();
                }
                else
                {
                    OnEnterNegativeState?.Invoke();
                }
            }
        }

        private bool StateIsPositive(LevelState levelState)
        {
            foreach (var positiveState in PositiveStates)
            {
                if (positiveState.Level != levelState.Level) break;

                if (positiveState.AnyState
                    || positiveState.StateId == levelState.StateId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
