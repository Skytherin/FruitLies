using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

public class Bartender : MonoBehaviour
{
    void OnMouseDown()
    {
        if (Global.WhoHasMouseControl != Mouser.General) return;

        var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        var mark = GameObject.Find("BartenderMark");

        mc.SetDestination(mark)
            .Then(() => BartenderConversation());
    }

    public static List<string> MajorOptions = new List<string>
    {
        "Rocking out",
        "Game design",
        "Napping"
    };

    public static int MajorLie;

    private void BartenderConversation()
    {
        Conversation.Instance.StartConversation(c =>
        {
            c.Add("Bartender", "What will you have?");

            c.Add("Alice", "Give me...")
                .Answers.AddRange(Bouncer.JuiceDrinks);
        })
        .Then(c =>
        {
            if (c.Single() != Bouncer.JuiceDrinkOfChoice)
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Bouncer", "That's not what she told me!");
                    c.Add("Alice", "Hey, can't a girl try something new for a change?");
                    c.Add("Bartender", "In all my years of bartending, no one has ever changed their drink of choice.");
                })
                .Then(() => DeathCanvas.Instance.Show());
            }
            else
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Bartender", "Here you go.");
                    c.Add("Bartender", "Hey, you go to school around here, don't you? What's your major?");
                    c.Add("Alice", "My major is...").Answers.AddRange(MajorOptions);
                })
                .Then(c =>
                {
                    MajorLie = c.Single();
                    Policeman.Instance.PoliceRaid();
                });
            }
        });
    }
}
