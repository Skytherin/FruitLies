using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mom : MonoBehaviour
{
    void OnMouseDown()
    {
        if (!Conversation.Started)
        {
            var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
            var momMark = GameObject.Find("MomMark");
            mc.OverridePosition = momMark.transform.position;
            mc.OnArrival = () => GetComponent<MomConversation>().StartConversation();
        }
    }

    void StartCriticalMomConversation()
    {
        var c = GameObject.Find("ConversationController").GetComponent<Conversation>();
        c.ConversationFlow.Items.Add(new ConversationItem("Alice", "Hey mom, can I go out tonight?"));
        c.ConversationFlow.Items.Add(new ConversationItem("Mom", "Where are you going, my little snack carrot?"));

        var item = new ConversationItem("Alice", "");
        item.Answers.Add("Over Cass's to study.");
        item.Answers.Add("Over Cass's to listen the new Kale and Fresh Veggies album.");
        item.Answers.Add("I'm going to walk Barkies the dog.");

        c.ConversationFlow.Items.Add(item);

        c.ConversationFlow.Items.Add(new ConversationItem("Mom", "That's nice dear, have fun."));
    }
}
