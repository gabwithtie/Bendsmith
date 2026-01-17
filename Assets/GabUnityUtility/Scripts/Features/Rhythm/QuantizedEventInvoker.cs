using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class QuantizedEventInvoker : MonoSingleton<QuantizedEventInvoker>
    {
        [SerializeField] private float interval = 1;
        [SerializeField] private float lookahead = 0.07f;
        [SerializeField] private float minimum_time = 0.15f;
        public static float BarLength => (60.0f / RhythmManager.Bpm) * Instance.interval;
        public static float RemainingTillNextBar => BarLength - (RhythmManager.Time % BarLength);

        private static List<System.Action> invokelist = new List<System.Action>();
        private static List<System.Action> invokelist_next = new List<System.Action>();

        private bool invoked_this_bar = false;

        public static float GetNextInvokationFromNow()
        {
            if (RemainingTillNextBar < Instance.minimum_time)
            {
                return RemainingTillNextBar + BarLength;
            }
            else
            {
                return RemainingTillNextBar;
            }
        }

        public static float InvokeOnNext(System.Action action)
        {
            if (RemainingTillNextBar < Instance.minimum_time)
            {
                invokelist_next.Add(action);
                return RemainingTillNextBar + BarLength;
            }
            else
            {
                invokelist.Add(action);
                return RemainingTillNextBar;
            }
        }

        private void Update()
        {
            float cur_progress_in_bar = RhythmManager.Time % BarLength;

            if(cur_progress_in_bar - lookahead < 0)
            {
                if (invoked_this_bar)
                    return;

                foreach (System.Action action in invokelist)
                {
                    action.Invoke();
                }

                invoked_this_bar = true;

                invokelist.Clear();
                invokelist.AddRange(invokelist_next);
                invokelist_next.Clear();
            }
            else
            {
                invoked_this_bar = false;
            }
        }
    }
}
