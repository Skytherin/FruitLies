using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CallbackThing<T>
{
    public Action<T> Callback;

    public void Then(Action<T> callback)
    {
        Callback = callback;
    }

    public void Then(Action callback)
    {
        Callback = _ => callback();
    }
}

public class Conversation : MonoBehaviour
{
    public static Conversation Instance;

    private bool Started = false;
    private ConversationFlow ConversationFlow = new ConversationFlow();
    private int ConversationIndex = -1;
    private GameObject SpeechBubbleAnchor;
    private CanvasGroup CanvasGroup;
    private Text Who;
    private Text What;
    private Button Q1;
    private Button Q2;
    private Button Q3;
    private CallbackThing<List<int>> Callback;

    public CallbackThing<List<int>> StartConversation(Action<ConversationFlow> script)
    {
        Assert.IsFalse(Started, "Attempted to start two conversations in a row!");
        ConversationFlow = new ConversationFlow();
        script(ConversationFlow);

        return StartConversationInternal();
    }

    void Start()
    {
        Instance = this;
        var canvas = GameObject.Find("ConversationCanvas");
        SpeechBubbleAnchor = canvas.transform.Find("Anchor").gameObject;
        CanvasGroup = canvas.GetComponent<CanvasGroup>();
        Who = SpeechBubbleAnchor.transform.Find("Who").GetComponent<Text>();
        What = SpeechBubbleAnchor.transform.Find("What").GetComponent<Text>();
        Q1 = SpeechBubbleAnchor.transform.Find("Q1").GetComponent<Button>();
        Q2 = SpeechBubbleAnchor.transform.Find("Q2").GetComponent<Button>();
        Q3 = SpeechBubbleAnchor.transform.Find("Q3").GetComponent<Button>();
        CanvasGroup.alpha = 0.0f;
    }

    private CallbackThing<List<int>> StartConversationInternal()
    {
        Global.WhoHasMouseControl = Mouser.Cutscene;
        Started = true;
        Q1.transform.gameObject.SetActive(false);
        Q2.transform.gameObject.SetActive(false);
        Q3.transform.gameObject.SetActive(false);
        CanvasGroup.alpha = 1.0f;
        ConversationIndex = 0;
        ShowCurrentIndex();
        Callback = new CallbackThing<List<int>>();
        return Callback;
    }

    private void ShowCurrentIndex()
    {
        var item = ConversationFlow.Items[ConversationIndex];
        var go = GameObject.Find(item.Who == Constants.Names.MC ? "MainCharacter" :  item.Who);
        var screenSpace = Camera.main.WorldToScreenPoint(go.transform.position);
        //SpeechBubbleAnchor.transform.position = new Vector3(screenSpace.x + 230, screenSpace.y + 170, 0);
        Who.text = item.Who;
        What.text = item.Text;

        foreach (var a in item.Answers
            .Select((answer, index) => new {answer, index})
            .Shuffle()
            .Zip(new[]{ Q1, Q2, Q3 }, (it, button) => new {it.answer,button, it.index}))
        {
            a.button.transform.gameObject.SetActive(true);
            a.button.GetComponentInChildren<Text>().text = a.answer;
            a.button.onClick = new Button.ButtonClickedEvent();
            a.button.onClick.AddListener(() => Answer(a.index));
        }
    }

    private void Answer(int which)
    {
        ConversationFlow.Answers.Add(which);
        foreach (var button in new[] {Q1, Q2, Q3})
        {
            button.transform.gameObject.SetActive(false);
            button.onClick = new Button.ButtonClickedEvent();
        }
        AdvanceConversation();
    }

    public void Update()
    {
        if (ConversationIndex >= 0 && Input.GetMouseButtonDown(0))
        {
            if (Q1.transform.gameObject.activeSelf ||
                Q1.transform.gameObject.activeSelf ||
                Q1.transform.gameObject.activeSelf)
            {
                return;
            }

            AdvanceConversation();
        }
    }

    private void AdvanceConversation()
    {
        ConversationIndex += 1;
        if (ConversationIndex >= ConversationFlow.Items.Count)
        {
            LeaveConversation();
            return;
        }

        ShowCurrentIndex();
    }

    private void LeaveConversation()
    {
        StartCoroutine(WaitThenExit());
    }

    private IEnumerator WaitThenExit()
    {
        yield return new WaitForFixedUpdate();
        Q1.transform.gameObject.SetActive(true);
        Q2.transform.gameObject.SetActive(true);
        Q3.transform.gameObject.SetActive(true);
        CanvasGroup.alpha = 0.0f;
        ConversationIndex = -1;
        Started = false;
        Global.WhoHasMouseControl = Mouser.General;
        Callback?.Callback?.Invoke(ConversationFlow.Answers);
    }
}

public class ConversationFlow
{
    public readonly List<ConversationItem> Items = new List<ConversationItem>();
    public readonly List<int> Answers = new List<int>();

    public ConversationItem Add(string who, string what)
    {
        var result = new ConversationItem(who, what);
        Items.Add(result);
        return result;
    }
}

public class ConversationItem
{
    public string Who;
    public string Text;
    public List<string> Answers = new List<string>();

    public ConversationItem AddAnswer(string answer)
    {
        Answers.Add(answer);
        return this;
    }

    public ConversationItem(string who, string text)
    {
        Who = who;
        Text = text;
    }
}