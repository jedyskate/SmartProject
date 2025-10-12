#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartConfig.App.Data;
using SmartConfig.App.Models;
using SmartConfig.App.Services;

namespace SmartConfig.App.PageModels;

public partial class ProjectListPageModel : ObservableObject
{
    private readonly ProjectRepository _projectRepository;

    [ObservableProperty] private List<Project> _projects = [];

    public ProjectListPageModel(ProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [RelayCommand]
    private async Task Appearing()
    {
        Projects = await _projectRepository.ListAsync();
    }

    [RelayCommand]
    Task NavigateToProject(Project project)
        => Shell.Current.GoToAsync($"project?id={project.ID}");

    [RelayCommand]
    async Task AddProject()
    {
        await Shell.Current.GoToAsync($"project");
    }
}