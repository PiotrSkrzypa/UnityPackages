using UnityEngine;
using UnityEngine.UI;
using R3;
using System;
using PSkrzypa.MVVMUI.BaseMenuWindow;
using PSkrzypa.UnityFX;

namespace PSkrzypa.MVVMUI.Samples
{
    public class MainMenuView : BaseWindowView<MainMenuViewModel>
    {
        [SerializeField] Button newGameButton;
        [SerializeField] Button continueButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button quitGameButton;

        IDisposable disposable;

        protected override void Awake()
        {
            base.Awake();
            var d = Disposable.CreateBuilder();
            viewModel.ProgressExists.Subscribe(x => continueButton.interactable = x).AddTo(ref d);
            newGameButton.OnClickAsObservable().Subscribe(_ => viewModel.StartNewGame.Execute(Unit.Default)).AddTo(ref d);
            continueButton.OnClickAsObservable().Subscribe(_ => viewModel.ContinueSavedGame.Execute(Unit.Default)).AddTo(ref d);
            settingsButton.OnClickAsObservable().Subscribe(_ => viewModel.OpenSettingsWindow.Execute(Unit.Default)).AddTo(ref d);
            quitGameButton.OnClickAsObservable().Subscribe(_ => viewModel.QuitGameCommand.Execute(Unit.Default)).AddTo(ref d);

            disposable = d.Build();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            disposable?.Dispose();
        }
    }
}