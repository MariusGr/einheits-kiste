using EinheitsKiste.KeyedMonoBehaviourMode;

namespace EinheitsKiste
{
    public abstract class StringKeyedMonoBehaviour<T> : StringKeyedMonoBehaviour<T, IInstancesInSceneOnlyMode>
        where T : StringKeyedMonoBehaviour<T, IInstancesInSceneOnlyMode>
    { }

    public abstract class StringKeyedMonoBehaviour<T, TMode> : KeyedMonoBehaviour<T, string, TMode>
        where T : KeyedMonoBehaviour<T, string, TMode>
        where TMode : IMode
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
