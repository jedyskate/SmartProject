using CommunityToolkit.Mvvm.Input;
using SmartConfig.App.Models;

namespace SmartConfig.App.PageModels;

public interface IProjectTaskPageModel
{
    IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
    bool IsBusy { get; }
}