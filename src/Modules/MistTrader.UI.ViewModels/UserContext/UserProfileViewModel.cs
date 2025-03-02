using DataParsers.Models;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.UserContext;

public class UserProfileViewModel : ViewModel
{
    private readonly Profile _profile;
    [Reactive] public string Name { get; set; }
    [Reactive] public int Level { get; set; }
    [Reactive] public int Silver { get; set; }
    [Reactive] public int Rubies { get; set; }
    
    public delegate UserProfileViewModel Factory(Profile profile);
    public UserProfileViewModel(Profile profile)
    {
        _profile = profile;
        Name = profile.Name;
        Level = profile.Level ?? 0;
        Silver = profile.Silver ?? 0;
        Rubies = profile.Rubies ?? 0;
    }
}