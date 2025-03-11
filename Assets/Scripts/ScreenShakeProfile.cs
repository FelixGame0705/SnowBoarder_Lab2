using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "ScreenShakeProfile", menuName = "ScreenShake/ScreenShakeProfile")]
    public class ScreenShakeProfile : ScriptableObject
    {
        [Header("Impulse Source Settings")]
        public float impactTime = 0.2f;
        public float impactForce = 1f;
        public Vector3 defaultVelocity = new Vector3(0f, -1f, 0f);
        public AnimationCurve impulseCurve;

        [Header("Impulse Source Settings")]
        public float listenerAmplitude = 1f;
        public float listenerFrequency = 1f;
        public float listenerDuration = 1f;
    }
}
