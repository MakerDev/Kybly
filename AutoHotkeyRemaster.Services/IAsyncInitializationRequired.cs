using System.Threading.Tasks;

namespace AutoHotkeyRemaster.Services
{
    //TODO : init 안된상태로 쓰려하면 exception 나게 고칠 수 있나?
    public interface IAsyncInitializationRequired
    {
        Task InitializeAsync();
    }
}
