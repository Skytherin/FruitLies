using System;
using System.Linq;
using Assets.ProceduralAnimationLibrary.Tweeners;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class Door : MonoBehaviour
{
    public static DoorState DoorState;

    void Start()
    {
        DoorState = DoorState.HasNotTalkedToMom;
    }

    void OnMouseDown()
    {
        if (Global.WhoHasMouseControl != Mouser.General) return;

        var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        var mark = GameObject.Find("DoorMark");
        mc.SetOverrideDestination(mark.transform.position).Then(_ =>
        {
            switch (DoorState)
            {
                case DoorState.HasNotTalkedToMom:
                    Conversation.Instance.StartConversation("", c => { c.Add("Alice", "I can't leave without telling mom."); });
                    break;
                case DoorState.HasNotTalkedToDad:
                {
                    var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
                    var dad = GameObject.Find("Dad");
                    var dadSpawnPoint = GameObject.Find("DadSpawnPoint");
                    var dadMark = GameObject.Find("DadMark");
                    var doorMark = GameObject.Find("DoorMark");
                    dad.transform.position = dadSpawnPoint.transform.position;
                    this.BeginSerial()
                        .MoveTo(dad, doorMark.transform.position, 1.0f)
                        .Start();

                    this.BeginSerial()
                        .MoveTo(mc.gameObject, dadMark.transform.position, 1.0f)
                        .Start(() => DadConversation());
                    break;
                }
                case DoorState.AllowedToLeave:
                    throw new NotImplementedException();
            }
        });
    }

    private void DadConversation()
    {
        var momsConversation = Conversation.Instance.RecordOfConversation["Mom"];
        var questionIndex = momsConversation.Answers.Keys.Single();
        var possibleAnswers = momsConversation.Items[questionIndex].Answers;
        var alicesAnswer = momsConversation.Answers[questionIndex];

        Conversation.Instance.StartConversation("Dad", c =>
        {
            c.Add("Dad", "Where do you think you are going, little lady?");

            var q = c.Add("Alice", "");
            q.Answers.AddRange(possibleAnswers);
        })
        .Then(record =>
        {
            if (record.Answers.Single().Value == alicesAnswer)
            {
                Conversation.Instance.StartConversation("", c =>
                {
                    c.Add("Dad", "You're not leaving this house.");
                    c.Add("Mom", "But hubby, we haven't had a night alone in weeks.");
                    c.Add("Dad", "...");
                    c.Add("Dad", "Have fun, Alice.");
                })
                .Then(_ =>
                {
                    var dad = GameObject.Find("Dad");
                    var mom = GameObject.Find("Mom");
                    var mark = GameObject.Find("OffscreenLeftMark");
                    this.BeginSerial()
                        .MoveTo(dad, mark.transform.position, 2.0f)
                        .Start();
                    this.BeginSerial()
                        .MoveTo(mom, mark.transform.position, 2.0f)
                        .Start();
                    DoorState = DoorState.AllowedToLeave;
                });
            }
            else
            {
                Conversation.Instance.StartConversation("", c =>
                {
                    c.Add("Mom", "That's not what you told me.");
                    c.Add("Dad", "Go to your room!!!");
                })
                .Then(() =>
                {
                    DeathCanvas.Instance.Show();
                });
            }
        });
    }
}

public enum DoorState
{
    HasNotTalkedToMom,
    HasNotTalkedToDad,
    AllowedToLeave
}