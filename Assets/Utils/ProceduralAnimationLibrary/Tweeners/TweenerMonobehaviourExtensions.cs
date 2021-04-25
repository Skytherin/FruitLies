using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public static class TweenerMonobehaviourExtensions
    {
        public static StartableTweener BeginSerial(this MonoBehaviour gameObject)
        {
            return new StartableTweener(new SerialTweener(), gameObject);
        }

        public static StartableTweener BeginParallel(this MonoBehaviour gameObject)
        {
            return new StartableTweener(new ParallelTweener(), gameObject);
        }
    }
}