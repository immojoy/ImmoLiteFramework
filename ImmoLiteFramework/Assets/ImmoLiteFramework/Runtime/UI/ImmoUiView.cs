using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    public abstract class ImmoUiView : MonoBehaviour
    {
        public virtual void OnCreate() { }
        public virtual void OnShow(object args = null) => gameObject.SetActive(true);
        public virtual void OnHide() => gameObject.SetActive(false);
        public virtual void OnDestroy() => Destroy(gameObject);
    }
}