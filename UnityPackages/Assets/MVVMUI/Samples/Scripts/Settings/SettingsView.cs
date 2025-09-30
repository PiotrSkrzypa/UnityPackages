using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.MVVMUI.Samples
{
    public class SettingsView : BaseWindowView<SettingsViewModel>
    {
        [SerializeField] Button returnButton;

        IDisposable disposable;

        protected override void OnViewModelBind()
        {
            base.OnViewModelBind();
            var d = Disposable.CreateBuilder();
            returnButton.OnClickAsObservable().Subscribe(_ => viewModel.CloseWindow()).AddTo(ref d);
            disposable = d.Build();
        }
        protected override void OnDispose()
        {
            disposable?.Dispose();
        }
    }
}