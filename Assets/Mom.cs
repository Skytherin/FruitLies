using UnityEngine;

public class Mom : MonoBehaviour
{
    void OnMouseDown()
    {
        if (!Conversation.Started)
        {
            var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
            var momMark = GameObject.Find("MomMark");
            mc.SetOverrideDestination(momMark.transform.position)
                .Then(_ => StartCriticalMomConversation());
        }
    }

    void StartCriticalMomConversation()
    {
        if (!Door.HasTalkedToMom)
        {
            Door.HasTalkedToMom = true;

            Conversation.StartConversation("Mom", c =>
            {
                c.Add("Alice", "Hey mom, can I go out tonight?");
                c.Add("Mom", "Where are you going, my little snack carrot?");

                var item = c.Add("Alice", "");
                item.Answers.Add("Over Cass's to study.");
                item.Answers.Add("Over Cass's to listen the new Kale and Fresh Veggies album.");
                item.Answers.Add("I'm going to walk Barkies the dog.");

                c.Add("Mom", "That's nice dear, have fun.");
            });
        }
    }
}
