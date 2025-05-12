using MyBox;

namespace EinheitsKiste
{
    public abstract class StringKeyedMonoBehaviour<T> : KeyedMonoBehaviour<T, string> where T : KeyedMonoBehaviour<T, string>
    {
        override protected void OnValidate()
        {
            if (string.IsNullOrEmpty(Key)) return;
            base.OnValidate();
        }

        override protected void OnDestroy()
        {
            if (string.IsNullOrEmpty(Key)) return;
            base.OnDestroy();
        }
    }
}
