using UnityEngine;

namespace Assets.Utils
{
    public static class GameObjectExtensions
    {
        public static void TeleportTo(this GameObject self, GameObject other)
        {
            self.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, self.transform.position.z);
        }
    }
}