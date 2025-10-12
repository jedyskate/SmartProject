using SmartConfig.App.Models;
using SmartConfig.App.PageModels;

namespace SmartConfig.App.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}