using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    private GameObject JuicyLucyEntrance;

    public static readonly List<string> JuiceDrinks = new List<string>
    {
        "Citrus Squeeze",
        "Melon Mountain",
        "Brash Banana"
    };

    private static readonly List<string> AllAgeOptions = new List<string>
    {
        "We're both 18.",
        "We're both 19.",
        "We're both 20.",
        "We're both 21.",
        "I'm 18 and she's 19.",
        "I'm 18 and she's 20.",
        "I'm 18 and she's 21.",
        "I'm 19 and she's 18.",
        "I'm 19 and she's 20.",
        "I'm 19 and she's 21.",
        "I'm 20 and she's 19.",
        "I'm 20 and she's 18.",
        "I'm 20 and she's 21.",
        "I'm 21 and she's 19.",
        "I'm 21 and she's 18.",
        "I'm 21 and she's 20.",
    };

    public static readonly List<string> AgeOptions = new List<string>
    {
        AllAgeOptions[0],
        AllAgeOptions[1],
        AllAgeOptions[2],
    };

    public static readonly List<string> HouseBands = new List<string>
    {
        "Kate and the Fresh Veggies",
        "The Apple Sauce Jam Band",
        "The Orange Peels"
    };

    public static int JuiceDrinkOfChoice;

    void Start()
    {
        JuicyLucyEntrance = GameObject.Find("JuicyLucyEntrance");
        JuicyLucyEntrance.SetActive(false);
    }

    void OnMouseDown()
    {
        if (Global.WhoHasMouseControl != Mouser.General) return;

        var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        var mark = GameObject.Find("BouncerMark");
        mc.SetDestination(mark)
            .Then(() => BouncerConversation());
    }

    private void BouncerConversation()
    {
        Conversation.Instance.StartConversation(c =>
        {
            c.Add("Bouncer", "Hey, you're too young to drink juice.");
            var options = c.Add(Constants.Names.MC, "I come here all the time, my friend and me are...");

            options.Answers.AddRange(AllAgeOptions);

            c.Add("Bouncer", "Guess I'm getting old; you kids all looks so young these days.");
            c.Add("Bouncer", "If you come here all the time, who's the house band?");
            c.Add("Alice", "").Answers.AddRange(HouseBands);
        })
        .Then(c =>
        {
            var ageLie = c.First();
            AgeOptions.Clear();
            var l = Enumerable.Range(0, AllAgeOptions.Count).ToList();
            l.Remove(ageLie);
            AgeOptions.Add(AllAgeOptions[ageLie]);
            AgeOptions.AddRange(l.Shuffle().Take(2).Select(it => AllAgeOptions[it]));

            if (c.Last() != 2)
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Bouncer", "Bzzzt, wrong. Get outta here, kid.");
                })
                .Then(() =>
                {
                    DeathCanvas.Instance.Show();
                });
            }
            else
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Bouncer", "Wow, you really are a regular. (Wonder why I've never seen you?)");
                    c.Add("Bouncer", "One more question: what's your favorite juice?");
                    c.Add("Alice", "").Answers.AddRange(JuiceDrinks);
                    c.Add("Bouncer", "You're legit, kid. Go on in.");
                }).Then(c =>
                {
                    JuiceDrinkOfChoice = c.Last();
                    var mark = GameObject.Find("BouncerMark2");
                    this.BeginSerial()
                        .MoveTo(gameObject, mark, 0.5f)
                        .Start();
                    JuicyLucyEntrance.SetActive(true);
                });
            }
        });
    }
}
