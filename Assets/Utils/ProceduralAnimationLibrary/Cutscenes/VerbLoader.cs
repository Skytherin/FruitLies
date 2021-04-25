#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Utils.ProceduralAnimationLibrary.Cutscenes
{
    public class EZComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> Comparer;
        private readonly Func<T, int> Hash;

        public EZComparer(Func<T, T, bool> comparer, Func<T, int> hash)
        {
            Comparer = comparer;
            Hash = hash;
        }

        public bool Equals(T x, T y)
        {
            return Comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return Hash(obj);
        }
    }

    public static class VerbLoader
    {
        public static Dictionary<string, VerbDetails> Verbs =
            new Dictionary<string, VerbDetails>(
                new EZComparer<string>((s1, s2) => string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) == 0,
                    s => s.ToUpper().GetHashCode())
            );

        public static bool Initialized;

        public static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;
                AddVerbsFromType(typeof(BuiltInVerbs));
            }
        }

        public static void AddVerbsFromType(Type type)
        {
            var verbs = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Select(m => new
                {
                    Method = m,
                    Action = m.GetCustomAttribute<CutsceneVerbAttribute>()
                })
                .Where(it => it.Action != null);

            foreach (var verb in verbs)
            {
                var expectedParameters = new Queue<ParameterInfo>(verb.Method.GetParameters());

                var noSubject = expectedParameters.First().Name == "directObject";

                if (!noSubject)
                {
                    var subjectParam = expectedParameters.Dequeue();
                    Assert.AreEqual(subjectParam.Name, "subject");
                    Assert.IsTrue(subjectParam.ParameterType == typeof(GameObject));
                }

                var doParam = expectedParameters.Dequeue();
                
                Assert.AreEqual(doParam.Name, "directObject");
                var doIsString = false;
                if (doParam.ParameterType == typeof(GameObject))
                {
                    // pass
                }
                else if (doParam.ParameterType == typeof(string))
                {
                    doIsString = true;
                }
                else
                {
                    throw new NotImplementedException("DirectObject should be GameObject or String");
                }

                ITween Action(GameObject subject, object directObject, Dictionary<string, string> parameters)
                {
                    var actuals = new List<object?>();

                    if (!noSubject)
                    {
                        actuals.Add(subject);
                    }

                    if (doIsString)
                    {
                        Assert.IsTrue(directObject is string);
                        actuals.Add(directObject);
                    }
                    else
                    {
                        Assert.IsTrue(directObject is GameObject);
                        actuals.Add(directObject);
                    }

                    foreach (var expected in expectedParameters)
                    {
                        if (parameters.TryGetValue(expected.Name, out var actualString))
                        {
                            if (expected.ParameterType == typeof(float?))
                            {
                                if (float.TryParse(actualString, out var actualFloat))
                                {
                                    actuals.Add(actualFloat);
                                    continue;
                                }
                            }

                            throw new NotImplementedException();
                        }

                        actuals.Add(null);
                    }

                    return (ITween) verb.Method.Invoke(null, actuals.ToArray());
                }

                var name = verb.Action.Name ?? verb.Method.Name;
                Verbs[name] = new VerbDetails(Action, doIsString, noSubject);
            }
        }
    }
}