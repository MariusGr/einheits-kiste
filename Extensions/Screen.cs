
using UnityEngine;

namespace EinheitsKiste
{
    public static class ScreenExtensions
    {
        public static Vector2 NormalizePosition(Vector2 screenPosition)
        {
            Vector2 screenSize = new(Screen.width, Screen.height);
            return new Vector2(screenPosition.x / screenSize.x, screenPosition.y / screenSize.y);
        }
    }
}
