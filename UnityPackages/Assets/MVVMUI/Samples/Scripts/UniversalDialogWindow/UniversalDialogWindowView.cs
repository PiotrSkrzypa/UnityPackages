using System;
using PSkrzypa.MVVMUI.BaseMenuWindow;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.MVVMUI.Samples
{
    public class UniversalDialogWindowView : BaseWindowView<UniversalDialogWindowViewModel>
    {
        [SerializeField] TextMeshProUGUI messageText, configmButtonText, cancelButtonText;
        [SerializeField] Button confirmButton, cancelButton;

        IDisposable disposable;

        protected override void Awake()
        {
            base.Awake();
            DisposableBuilder d = Disposable.CreateBuilder();
            confirmButton.OnClickAsObservable().Subscribe(_ => viewModel.OnConfirm()).AddTo(ref d);
            cancelButton.OnClickAsObservable().Subscribe(_ => viewModel.OnCancel()).AddTo(ref d);
            viewModel.message.Subscribe(x => messageText.text = x).AddTo(ref d);
            viewModel.confirmText.Subscribe(x => { configmButtonText.text = x; confirmButton.gameObject.SetActive(!string.IsNullOrEmpty(x)); }).AddTo(ref d);
            viewModel.denyText.Subscribe(x => { cancelButtonText.text = x; cancelButton.gameObject.SetActive(!string.IsNullOrEmpty(x)); }).AddTo(ref d);
            disposable = d.Build();
        }
        protected override void OnDestroy()
        {
            disposable?.Dispose();
            base.OnDestroy();
        }
    }
}