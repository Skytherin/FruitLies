using System.Collections;
using System.Collections.Generic;
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
                Conversation.StartConversation(c => { c.Add("Alice", "I can't leave without telling mom."); });
            }
        };
    }
}
