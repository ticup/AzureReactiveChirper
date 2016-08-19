using System.Threading.Tasks;

namespace GrainInterfaces
{
    /// <summary>
    /// Orleans grain communication interface IHello
    /// </summary>
    public interface IHelloEnvironment : Orleans.IGrainWithIntegerKey
    {
        Task<string> RequestDetails();
    }
}
