using R3;
using TMPro;

namespace PSkrzypa.MVVMUI
{
    public static class UIR3Extensions
    {
        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static Observable<int> OnValueChangedAsObservable(this TMP_Dropdown dropdown)
        {
            return Observable.Create<int, TMP_Dropdown>(dropdown, static (observer, d) =>
            {
                observer.OnNext(d.value);
                return d.onValueChanged.AsObservable(d.destroyCancellationToken).Subscribe(observer);
            });
        }
    }
}