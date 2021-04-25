using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

namespace Assets.Utils
{
    public static class GameObjectExtensions
    {
        public static void TeleportTo(this GameObject self, GameObject other)
        {
            self.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, self.transform.position.z);
        }

        public static CallbackThing<bool> MoveToMark(this MonoBehaviour self, string subject, string mark, float duration)
        {
            var go = GameObject.Find(subject);
            var m = GameObject.Find(mark);
            return self.BeginSerial()
                .MoveTo(go, m, duration)
                .Start();
        }

        public static void LookLeft(this GameObject self)
        {
            var transformRotation = self.transform.rotation;
            var canvas = self.transform.Find("Canvas");
            if (transformRotation.eulerAngles.x >= 0)
            {
                self.transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
                canvas.transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
            }
        }

        public static void LookRight(this GameObject self)
        {
            var transformRotation = self.transform.rotation;
            var canvas = self.transform.Find("Canvas");
            if (transformRotation.eulerAngles.x < 0)
            {
                self.transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
                canvas.transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
            }
        }
    }
}