using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

public class Mom : MonoBehaviour
{
    public static int AnswerToMom;

    public static List<string> PossibleAnswers = new List<string>
    {
        "Over Cass's to study.",
        "Over Cass's to listen the new Kale and Fresh Veggies album.",
        "I'm going to walk Barkies the dog."
    };

    void OnMouseDown()
    {
        if (Global.WhoHasMouseControl != Mouser.General) return;

        var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        var momMark = GameObject.Find("MomMark");
        mc.SetDestination(momMark.transform.position)
            .Then(_ => StartCriticalMomConversation());
    }

    void StartCriticalMomConversation()
    {
        if (Door.DoorState == DoorState.HasNotTalkedToMom)
        {
            Door.DoorState = DoorState.HasNotTalkedToDad;

            Conversation.Instance.StartConversation(c =>
            {
                c.Add(Constants.Names.MC, "Hey mom, can I go out tonight?");
                c.Add("Mom", "Where are you going, my little snack carrot?");

                c.Add(Constants.Names.MC, "").Answers.AddRange(PossibleAnswers);

                c.Add("Mom", "That's nice dear, have fun.");
            }).Then(c => AnswerToMom = c.Single());
        }
        else
        {
            Conversation.Instance.StartConversation(c =>
            {
                c.Add("Mom", "Have a nice time, dear.");
            });
        }
    }
}
