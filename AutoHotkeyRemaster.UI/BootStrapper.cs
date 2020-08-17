using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.Services.Helpers;
using AutoHotkeyRemaster.Services.Interfaces;
using AutoHotkeyRemaster.WPF.Helpers;
using AutoHotkeyRemaster.WPF.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace AutoHotkeyRemaster.WPF
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container
            = new SimpleContainer();
        private readonly VirtualKeycodeToWpfKeyConverter _keyConverter
            = new VirtualKeycodeToWpfKeyConverter();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            //HACK : AutoHotkeyRemaster.Package에서 WINDOWS_UWP 상수를 정의할 수 없어서 이렇게 처리
#if WINDOWS_WP
            _container.Singleton<IAsyncJsonFileManager, AsyncJsonSavefileManager>();
#else
            _container.Singleton<IAsyncJsonFileManager, UWPAsyncFileManager>();
#endif
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ProfileSwitchKeyTableManager>()
                .Singleton<ApplicationModel>()
                .Singleton<WindowsHookManager>()
                .Singleton<ProfileManager>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                   viewModelType, viewModelType.ToString(), viewModelType
                   ));


            KeyInfo.VirtualKeycodeToStringConverter = _keyConverter;
        }

        //HACK : All async initialization goes here. 
        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            var asyncInitRequiredClasses = Assembly
                .GetAssembly(typeof(IAsyncInitializationRequired))
                .GetTypes()
                .Where(type => type.IsClass)
                .Where(type => typeof(IAsyncInitializationRequired).IsAssignableFrom(type))
                .ToList();

            List<Task> initTasks = new List<Task>();

            foreach (var item in asyncInitRequiredClasses)
            {
                var instance = _container.GetInstance(item, null) as IAsyncInitializationRequired;
                initTasks.Add(instance.InitializeAsync());
            }

            await Task.WhenAll(initTasks);

            await DisplayRootViewFor<ShellViewModel>();
        }

        //아래들은 프레임워크에 컨테이너를 ServiceLocator로 등록하는 과정이라고 보면 될듯.
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
