#nullable enable
using System;
using System.Collections;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public class StartableTweener: ITweener
    {
        private readonly ITweener Implementation;
        private readonly MonoBehaviour GameObject;
        private Coroutine? Coroutine;

        public StartableTweener(ITweener implementation, MonoBehaviour gameObject)
        {
            Implementation = implementation;
            GameObject = gameObject;
        }

        public CallbackThing<bool> Start()
        {
            var cb = new CallbackThing<bool>();
            Assert.IsNull(Coroutine, "Cannot Start tweener while it is active");
            Coroutine = GameObject.StartCoroutine(Begin(() => cb.Callback(true)));
            return cb;
        }

        public void AppendInternal(ITween tween)
        {
            Implementation.AppendInternal(tween);
        }

        public IEnumerator Begin(Action? onCompleteCallback)
        {
            return Implementation.Begin(() =>
            {
                Coroutine = null;
                onCompleteCallback?.Invoke();
            });
        }

        public bool InProgress => Coroutine != null;

        public void Stop()
        {
            if (Coroutine != null)
            {
                GameObject.StopCoroutine(Coroutine);
                Coroutine = null;
            }
        }
    }
}