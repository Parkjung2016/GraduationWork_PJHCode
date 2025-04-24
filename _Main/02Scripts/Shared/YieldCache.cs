using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Shared
{
    static class YieldCache
    {
        class FloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y)
            {
                return Mathf.Approximately(x, y);
            }

            int IEqualityComparer<float>.GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }

        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

        private static readonly Dictionary<float, WaitForSeconds> _timeInterval =
            new(new FloatComparer());

        private static readonly Dictionary<float, WaitForSecondsRealtime> _timeIntervalReal =
            new(new FloatComparer());

        private static readonly Dictionary<Func<bool>, WaitUntil> _waitUntils =
            new Dictionary<Func<bool>, WaitUntil>();

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (!_timeInterval.TryGetValue(seconds, out var wfs))
            {
                wfs = WaitForSeconds(seconds);
                _timeInterval.Add(seconds, wfs);
            }

            return wfs;
        }

        public static WaitForSecondsRealtime WaitForSecondsRealTime(float seconds)
        {
            if (!_timeIntervalReal.TryGetValue(seconds, out var wfsReal))
            {
                wfsReal = new WaitForSecondsRealtime(seconds);
                _timeIntervalReal.Add(seconds, wfsReal);
            }

            return wfsReal;
        }

        public static WaitUntil WaitUntil(Func<bool> predicate)
        {
            if (!_waitUntils.TryGetValue(predicate, out var wn))
            {
                wn = new WaitUntil(predicate);
                _waitUntils.Add(predicate, wn);
            }

            return wn;
        }
    }
}