using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.WPF.ViewModels;
using AutoHotkeyRemaster.WPF.Helpers;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

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
            //Instance는 우리가 미리 생성한 instance를 등록하는 것-싱글턴처럼 동작
            _container.Instance(_container);

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ProfileSwitchKeyTable>()
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

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
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
