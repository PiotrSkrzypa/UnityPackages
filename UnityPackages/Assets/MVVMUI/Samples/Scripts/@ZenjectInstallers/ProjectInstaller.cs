using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<MenuController>().AsSingle().NonLazy();
        }
    } 
}