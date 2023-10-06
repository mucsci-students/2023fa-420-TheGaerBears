using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace SpellingBee.Services
{
    public interface IFilesService
    {
        public Task<IStorageFile?> OpenFileAsync();
    }
}