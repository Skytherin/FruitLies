#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Utils.ProceduralAnimationLibrary.Cutscenes;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Utils.ProceduralAnimationLibrary.Cutscenes
{
    public static class CutsceneParser
    {
        public static ITweener Parse(string text)
        {
            VerbLoader.Initialize();
            var result = new SerialTweener();

            var instant = false;

            foreach (var line in text.Split('\n')
                .Select(line => line.Trim())
                .Select(line =>
                {
                    var m = Regex.Match(line, @"^(?<line>[^#]*)");
                    return m.Groups["line"].Value;
                })
                .Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                if (Regex.IsMatch(line, @"^begin\s+instant$", RegexOptions.IgnoreCase))
                {
                    Assert.IsFalse(instant, "Nested Begin Instant illegal.");
                    instant = true;
                    continue;
                }

                if (Regex.IsMatch(line, @"^end\s+instant$", RegexOptions.IgnoreCase))
                {
                    Assert.IsTrue(instant, "End Instant without Begin Instant not legal.");
                    instant = false;
                    continue;
                }


                var subjectMatch = Regex.Match(line, @"^\s*(?<Subject>\w+)");
                Assert.IsTrue(subjectMatch.Success);
                var remainder = line.Substring(subjectMatch.Length).Trim();
                var subjectString = subjectMatch.Groups["Subject"].Value;

                GameObject? subject = null;
                VerbDetails? verbDetails = null;
                if (VerbLoader.Verbs.TryGetValue(subjectString, out var subjectlessVerb)
                    && subjectlessVerb.NoSubject)
                {
                    verbDetails = subjectlessVerb;
                }
                else
                {
                    var verbMatch = Regex.Match(remainder, @"(?<Verb>\w+)");
                    Assert.IsTrue(verbMatch.Success);
                    subject = GameObject.Find(subjectString);
                    Assert.IsNotNull(subject);
                    var verbString = verbMatch.Groups["Verb"].Value;
                    var verbFound = VerbLoader.Verbs.TryGetValue(verbString, out verbDetails);
                    Assert.IsTrue(verbFound);
                    remainder = remainder.Substring(verbMatch.Length).Trim();
                }
                
                if (verbDetails.DirectObjectIsString)
                {
                    var temp = verbDetails.Action.Invoke(subject!, remainder,
                        new Dictionary<string, string>());
                    if (instant)
                    {
                        temp = temp.ToInstant();
                    }
                    result.Append(temp);
                    continue;
                }

                var remainderMatch = Regex.Match(remainder, @"(?<DirectObject>\w+)\s*(\swith\s+(?<Parameters>.*))?$");
                Assert.IsTrue(remainderMatch.Success);

                var t2 = remainderMatch.Groups["DirectObject"].Value;
                var t3 = remainderMatch.Groups["Parameters"].Value ?? "";
                var directObject = GameObject.Find(t2);
                var parameters =
                    !string.IsNullOrWhiteSpace(t3)
                        ? t3.Split(',')
                            .Select(it => Regex.Split(it, @"\s+"))
                            .ToDictionary(it => it[0], it => it[1])
                        : new Dictionary<string, string>();

                var temp2 = verbDetails.Action.Invoke(subject!, directObject, parameters);
                if (instant)
                {
                    temp2 = temp2.ToInstant();
                }
                result.Append(temp2);
            }

            return result;
        }
    }
}