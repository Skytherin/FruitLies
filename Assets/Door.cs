using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public static bool HasTalkedToMom = false;

    void OnMouseDown()
    {
        if (Conversation.Started) return;

        var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        var mark = GameObject.Find("DoorMark");
        mc.OverridePosition = mark.transform.position;
        mc.OnArrival = () =>
        {
            if (!HasTalkedToMom)
            {
                Conversation.StartConversation("", c => { c.Add("Alice", "I can't leave without telling mom."); });
            }
            else
            {
                StartCoroutine(DadComesMarchingIn());
            }
        };
    }

    private IEnumerator DadComesMarchingIn()
    {
        var mc = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        var dad = GameObject.Find("Dad");
        var dadSpawnPoint = GameObject.Find("DadSpawnPoint");
        var dadMark = GameObject.Find("DadMark");
        var doorMark = GameObject.Find("DoorMark");
        dad.transform.position = dadSpawnPoint.transform.position;

        for (var i = 0; i < 100; i++)
        {
            yield return new WaitForEndOfFrame();
            dad.transform.position = Vector2.Lerp(dad.transform.position, doorMark.transform.position, 0.01f);
            mc.transform.position = Vector2.Lerp(mc.transform.position, dadMark.transform.position, 0.01f);
        }

        var momsConversation = Conversation.RecordOfConversation["Mom"];
        var questionIndex = momsConversation.Answers.Keys.Single();
        var possibleAnswers = momsConversation.Items[questionIndex].Answers;
        var alicesAnswer = momsConversation.Answers[questionIndex];

        Conversation.StartConversation("Dad", c =>
        {
            c.Add("Dad", "Where do you think you are going, little lady?");

            var q = c.Add("Alice", "");
            q.Answers.AddRange(possibleAnswers);
        })
        .Then(record =>
        {
            if (record.Answers.Single().Value == alicesAnswer)
            {
                Conversation.StartConversation("", c =>
                {
                    c.Add("Dad", "You're not leaving this house.");
                    c.Add("Mom", "But hubby, we haven't had a night alone in weeks.");
                    c.Add("Dad", "...");
                    c.Add("Dad", "Have fun, Alice.");
                });
            }
            else
            {
                Conversation.StartConversation("", c =>
                {
                    c.Add("Mom", "That's not what you told me.");
                    c.Add("Dad", "Go to your room!!!");
                });
            }
        });
    }
}
