using R3;
using System;
using PSkrzypa.MVVMUI.BaseMenuWindow;
using UnityEngine;

namespace PSkrzypa.MVVMUI.Samples
{
    public class MainMenuViewModel : BaseViewModel
    {
        public ReactiveProperty<bool> ProgressExists;
        public ReactiveCommand StartNewGame;
        public ReactiveCommand ContinueSavedGame;
        public ReactiveCommand OpenSettingsWindow;
        public ReactiveCommand QuitGameCommand;

        MainMenuModel model;
        IDisposable disposable;

        public MainMenuViewModel(MenuWindowConfig menuWindowConfig, MainMenuModel model) : base(menuWindowConfig)
        {
            this.menuWindowConfig = menuWindowConfig;
            this.model = model;
            var d = Disposable.CreateBuilder();
            ProgressExists = new ReactiveProperty<bool>(model.ProgressExists());
            ProgressExists.AddTo(ref d);
            StartNewGame = new ReactiveCommand();
            StartNewGame.Subscribe(_ => model.StartNewGame()).AddTo(ref d);
            ContinueSavedGame = new ReactiveCommand();
            ContinueSavedGame.Subscribe(_ => model.ContinueSavedGame()).AddTo(ref d);
            OpenSettingsWindow = new ReactiveCommand();
            OpenSettingsWindow.Subscribe(_ => model.OpenSettings()).AddTo(ref d);
            QuitGameCommand = new ReactiveCommand();
            QuitGameCommand.Subscribe(_ => QuitGame()).AddTo(ref d);
            disposable = d.Build();
        }
        void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        public override void Dispose()
        {
            base.Dispose();
            disposable?.Dispose();
        }
    }
}