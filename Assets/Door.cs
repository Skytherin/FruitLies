using System;
using System.Linq;
using Assets;
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
        mc.SetDestination(mark.transform.position).Then(_ =>
        {
            switch (DoorState)
            {
                case DoorState.HasNotTalkedToMom:
                    Conversation.Instance.StartConversation(c => { c.Add(Constants.Names.MC, "I can't leave without telling mom.", BoxType.Left); });
                    break;
                case DoorState.HasNotTalkedToDad:
                {
                    var mc = GameObject.Find("MainCharacter");
                    var dad = GameObject.Find("Dad");
                    var dadSpawnPoint = GameObject.Find("DadSpawnPoint");
                    var dadMark = GameObject.Find("DadMark");
                    var doorMark = GameObject.Find("DoorMark");
                    dad.TeleportTo(dadSpawnPoint);
                    this.BeginSerial()
                        .MoveTo(dad, doorMark.transform.position, 1.0f)
                        .Start();

                    this.BeginSerial()
                        .MoveTo(mc, dadMark.transform.position, 1.0f)
                        .Start()
                        .Then(() => DadConversation());
                    break;
                }
                case DoorState.AllowedToLeave:
                {
                    var mc = GameObject.Find("MainCharacter");
                    var dadSpawnPoint = GameObject.Find("DadSpawnPoint");
                    
                    this.BeginSerial()
                        .MoveTo(mc, dadSpawnPoint.transform.position, 1.0f)
                        .Start()
                        .Then(() => SceneTransition.Instance.TransitionTo("OutsideTheClub"));
                    
                    break;
                }
            }
        });
    }

    private void DadConversation()
    {
        var possibleAnswers = Mom.PossibleAnswers;
        var alicesAnswer = Mom.AnswerToMom;

        Conversation.Instance.StartConversation(c =>
        {
            c.Add("Dad", "Where do you think you are going, little lady?", BoxType.Left);

            var q = c.Add(Constants.Names.MC, "", BoxType.QuestionLeft);
            q.Answers.AddRange(possibleAnswers);
        })
        .Then(record =>
        {
            if (record.Single() == alicesAnswer)
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Dad", "You're not leaving this house.", BoxType.Left);
                    c.Add("Mom", "But hubby, we haven't had a night alone in weeks.", BoxType.Left);
                    c.Add("Dad", "...", BoxType.Left);
                    c.Add("Dad", "Have fun, Alice.", BoxType.Left);
                })
                .Then(_ =>
                {
                    var dad = GameObject.Find("Dad");
                    var mom = GameObject.Find("Mom");
                    var mark = GameObject.Find("OffscreenLeftMark");
                    this.BeginSerial()
                        .MoveTo(dad, mark.transform.position, 1.0f)
                        .Start();
                    this.BeginSerial()
                        .MoveTo(mom, mark.transform.position, 1.0f)
                        .Start();
                    DoorState = DoorState.AllowedToLeave;
                });
            }
            else
            {
                Conversation.Instance.StartConversation(c =>
                {
                    c.Add("Mom", "That's not what you told me.", BoxType.Left);
                    c.Add("Dad", "Go to your room!!!", BoxType.Left);
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