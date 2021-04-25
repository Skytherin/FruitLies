using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class Policeman : MonoBehaviour
{
    public static Policeman Instance;

    public Policeman()
    {
        Instance = this;
    }

    public void PoliceRaid()
    {
        var mark = GameObject.Find("PolicemanMark");
        gameObject.TeleportTo(mark);

        Global.WhoHasMouseControl = Mouser.Cutscene;
        Conversation.Instance.StartConversation(c =>
        {
            c.Add("Policeman", "Everyone, this is an illegal fruit-related gathering! Stay where you are!");
            c.Add("Bartender", "The fuzz! Cheese it!");
        })
        .Then(() =>
        {
            Global.WhoHasMouseControl = Mouser.Cutscene;
            var bt = GameObject.Find("Bartender");
            var mark = GameObject.Find("OffscreenRightMark");
            this.BeginSerial()
                .MoveTo(bt, mark, 0.5f)
                .Start()
                .Then(() =>
                {
                    Conversation.Instance.StartConversation(c =>
                    {
                        c.Add("Policeman", "Redhaired girl, you look like an honest person. Let me ask you some questions.");
                    })
                    .Then(() =>
                    {
                        this.MoveToMark("MainCharacter", "PolicemanFront", 0.5f)
                            .Then(() => SceneTransition.Instance.FadeOutAndIn()
                                .Then(() => PoliceQuiz()));
                    });
                });
        });
    }

    private void PoliceQuiz()
    {
        var cb = Quiz("...So what did you say to your mom?", Mom.PossibleAnswers, Mom.AnswerToMom);

        cb.Then(() =>
        {
            cb = Quiz("How old did you tell the bartender you are?", Bouncer.AgeOptions, 0);

            cb.Then(() =>
            {
                cb = Quiz("What's your major?", Bartender.MajorOptions, Bartender.MajorLie);

                cb.Then(() =>
                {
                    Conversation.Instance.StartConversation(c =>
                    {
                        c.Add("Policeman",
                            "Well, you seem like an honest girl. Go on, you scamp. The rest of you, it's the hoosegow with you!");
                        c.Add("Cass", "Not the hoosegow! I'm too pretty.");
                    })
                    .Then(() => SceneTransition.Instance.TransitionTo("Credits"));
                });
            });
        });
    }

    private CallbackThing<bool> Quiz(string text, List<string> possibleAnswers, int actualAnswer)
    {
        var cb = new CallbackThing<bool>();
        Conversation.Instance.StartConversation(c =>
        {
            c.Add("Policeman", text);
            c.Add("Alice", "").Answers.AddRange(possibleAnswers);
        })
        .Then(c =>
        {
            if (c.Single() != actualAnswer)
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Policeman", "... That's not right. Off to the hoosegow with you!");
                })  
                .Then(() => DeathCanvas.Instance.Show());
            }
            else
            {
                cb.Callback(true);
            }
        });
        return cb;
    }
}
