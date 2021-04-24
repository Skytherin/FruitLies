using Assets.ProceduralAnimationLibrary.Tweeners;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private bool OverridePosition = false;

    private readonly float speed = 15.0f;

    public CallbackThing<bool> SetOverrideDestination(Vector2 targetPosition)
    {
        OverridePosition = true;
        StopAllCoroutines();
        var callbackThing = new CallbackThing<bool>();
        this.BeginSerial()
            .MoveTo(gameObject, targetPosition, Vector2.Distance(transform.position, targetPosition) / speed)
            .Start(() =>
            {
                OverridePosition = false;
                callbackThing.Callback?.Invoke(true);
            });
        return callbackThing;
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.WhoHasMouseControl != Mouser.General) return;

        if (Input.GetMouseButtonDown(0) && !OverridePosition)
        {
            StopAllCoroutines();
            var targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(targetPosition);
            this.BeginSerial()
                .MoveTo(gameObject, targetPosition, Vector2.Distance(transform.position, targetPosition) / speed)
                .Start(() =>
                {
                    Debug.Log("Done");
                });
        }
    }
}
