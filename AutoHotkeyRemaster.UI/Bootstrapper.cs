using AutoHotkeyRemaster.Services;
using AutoHotkeyRemaster.UI.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AutoHotkeyRemaster.UI
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

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
                .Singleton<WindowsHookManager>()
                .Singleton<ProfileManager>();
                

            //RegisterPerRequest는 요청이 A라는 타입의 클래스가 요청되면 정말 그 클래스 객체를 생성하도록 등록한다.
            //PerRequest는 어떤 인터페이스나 부모클래스가 요청받았을 때, 그를 구현하거나 상속받은 클래스가 생성되도록 등록하는 차이가 있다.
            //컨테이너에 뷰모델도 등록하는 듯.
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                   viewModelType, viewModelType.ToString(), viewModelType
                   ));
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
