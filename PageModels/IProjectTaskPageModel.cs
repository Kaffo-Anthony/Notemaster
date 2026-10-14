using CommunityToolkit.Mvvm.Input;
using Notemaster.Models;

namespace Notemaster.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}