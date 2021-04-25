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
    public Action<T> Callback = _ => { };

    public void Then(Action<T> callback)
    {
        Callback = callback;
    }

    public void Then(Action callback)
    {
        Callback = _ => callback();
    }
}

[Serializable]
public struct People
{
    public string Name;
    public GameObject Left;
    public GameObject Right;
    public GameObject LeftQuestion;
    public GameObject RightQuestion;
}

public class Conversation : MonoBehaviour
{
    public static Conversation Instance;

    public List<People> People;
    private GameObject _currentBox;

    private ConversationFlow ConversationFlow = new ConversationFlow();
    private int ConversationIndex = -1;
    private Button Q1;
    private Button Q2;
    private Button Q3;
    private CallbackThing<List<int>> Callback;

    public CallbackThing<List<int>> StartConversation(Action<ConversationFlow> script)
    {
        ConversationFlow = new ConversationFlow();
        script(ConversationFlow);

        return StartConversationInternal();
    }

    void Start()
    {
        Instance = this;
    }

    private CallbackThing<List<int>> StartConversationInternal()
    {
        Global.WhoHasMouseControl = Mouser.Cutscene;
        ConversationIndex = 0;
        ShowCurrentIndex();
        Callback = new CallbackThing<List<int>>();
        return Callback;
    }

    private void ShowCurrentIndex()
    {
        var item = ConversationFlow.Items[ConversationIndex];
        var person = People.First(x => x.Name == item.Who);
        _currentBox = item.BoxType switch
        {
            BoxType.Left => person.Left,
            BoxType.Right => person.Right,
            BoxType.QuestionLeft => person.LeftQuestion,
            BoxType.QuestionRight => person.RightQuestion
        };
        _currentBox.SetActive(true);

        if (item.BoxType == BoxType.Left || item.BoxType == BoxType.Right)
        {
            _currentBox.transform.Find("text").Find("Who").GetComponent<Text>().text = item.Who;
            _currentBox.transform.Find("text").Find("What").GetComponent<Text>().text = item.Text;
        }
        else
        {
            Q1 = _currentBox.transform.Find("answers").Find("Q1").GetComponent<Button>();
            Q2 = _currentBox.transform.Find("answers").Find("Q2").GetComponent<Button>();
            Q3 = _currentBox.transform.Find("answers").Find("Q3").GetComponent<Button>();

            foreach (var a in item.Answers
                .Select((answer, index) => new {answer, index})
                .Shuffle()
                .Zip(new[]{ Q1, Q2, Q3 }, (it, button) => new {it.answer,button, it.index}))
            {
                a.button.GetComponentInChildren<Text>().text = a.answer;
                a.button.onClick = new Button.ButtonClickedEvent();
                a.button.onClick.AddListener(() => Answer(a.index));
            }

            _hasAnswered = false;
        }

    }

    private bool _hasAnswered = true;
    private void Answer(int which)
    {
        _hasAnswered = true;
        ConversationFlow.Answers.Add(which);
        foreach (var button in new[] {Q1, Q2, Q3})
        {
            button.onClick = new Button.ButtonClickedEvent();
        }
        AdvanceConversation();
    }

    public void Update()
    {
        if (ConversationIndex >= 0 && Input.GetMouseButtonDown(0))
        {
            if (!_hasAnswered)
            {
                return;
            }

            AdvanceConversation();
        }
    }

    private void AdvanceConversation()
    {
        _currentBox.SetActive(false);

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
        ConversationIndex = -1;
        Global.WhoHasMouseControl = Mouser.General;
        Callback?.Callback?.Invoke(ConversationFlow.Answers);
    }
}

public class ConversationFlow
{
    public readonly List<ConversationItem> Items = new List<ConversationItem>();
    public readonly List<int> Answers = new List<int>();

    public ConversationItem Add(string who, string what, BoxType boxType = BoxType.Left)
    {
        var result = new ConversationItem(who, what, boxType);
        Items.Add(result);
        return result;
    }
}

public enum BoxType
{
    Left,
    Right,
    QuestionLeft,
    QuestionRight,
}

public class ConversationItem
{
    public string Who;
    public string Text;
    public BoxType BoxType;
    public List<string> Answers = new List<string>();

    public ConversationItem AddAnswer(string answer)
    {
        Answers.Add(answer);
        return this;
    }

    public ConversationItem(string who, string text, BoxType boxType)
    {
        Who = who;
        Text = text;
        BoxType = boxType;
    }
}