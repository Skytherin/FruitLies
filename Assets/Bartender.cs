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
            c.Add("Bartender", "What will you have?", BoxType.Left);

            c.Add("Alice", "Give me...", BoxType.Left);
            c.Add("Alice", "", BoxType.QuestionLeft)
                .Answers.AddRange(Bouncer.JuiceDrinks);
        })
        .Then(c =>
        {
            if (c.Single() != Bouncer.JuiceDrinkOfChoice)
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Bouncer", "That's not what she told me!", BoxType.Right);
                    c.Add("Alice", "Hey, can't a girl try something new for a change?", BoxType.Left);
                    c.Add("Bartender", "In all my years of bartending, no one has ever changed their drink of choice.", BoxType.Left);
                })
                .Then(() => DeathCanvas.Instance.Show());
            }
            else
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Bartender", "Here you go.", BoxType.Left);
                    c.Add("Bartender", "Hey, you go to school around here, don't you? What's your major?", BoxType.Left);
                    c.Add("Alice", "My major is...", BoxType.Left);
                    c.Add("Alice", "", BoxType.QuestionLeft).Answers.AddRange(MajorOptions);
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
