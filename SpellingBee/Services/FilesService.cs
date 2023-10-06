using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SpellingBee.ViewModels;

namespace SpellingBee.Services
{
    public class FilesService : IFilesService
    {
        private readonly Window _target;

        public FilesService(Window target)
        {
            _target = target;
        }

        public async Task<IStorageFile?> OpenFileAsync()
        {
            var storageProvider = TopLevel.GetTopLevel(_target)!.StorageProvider;
            IStorageFolder? options = await storageProvider.TryGetFolderFromPathAsync("../../../saves/");
            var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open Save File",
                SuggestedStartLocation = options,
                AllowMultiple = false
            });

            return files.Count >= 1 ? files[0] : null;
        }
    }
}