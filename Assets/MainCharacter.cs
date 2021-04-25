using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private readonly float speed = 15.0f;

    public CallbackThing<bool> SetDestination(GameObject targetPosition) => SetDestination(targetPosition.transform.position);

    public CallbackThing<bool> SetDestination(Vector2 targetPosition)
    {
        StopAllCoroutines();
        var callbackThing = new CallbackThing<bool>();
        this.BeginSerial()
            .MoveTo(gameObject, targetPosition, Vector2.Distance(transform.position, targetPosition) / speed)
            .Start(() =>
            {
                callbackThing.Callback?.Invoke(true);
            });
        return callbackThing;
    }
}
