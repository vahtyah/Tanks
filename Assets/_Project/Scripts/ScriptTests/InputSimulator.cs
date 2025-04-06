using UnityEngine;
using System.Collections.Generic;

namespace Testing
{
    public static class InputSimulator
    {
        private static Dictionary<string, float> axisValues = new Dictionary<string, float>();

        public static void SetAxisValue(string axisName, float value)
        {
            axisValues[axisName] = value;
        }

        public static float GetAxisValue(string axisName)
        {
            if (axisValues.ContainsKey(axisName))
            {
                return axisValues[axisName];
            }
            return 0f;
        }

        public static void Reset()
        {
            axisValues.Clear();
        }

        public static void SetButtonDown(string fire1)
        {
            axisValues[fire1] = 1f;
        }

        public static void SetButtonUp(string fire1)
        {
                axisValues[fire1] = 0f;
        }
    }
}