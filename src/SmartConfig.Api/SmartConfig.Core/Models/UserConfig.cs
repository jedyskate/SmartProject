using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartConfig.Core.Enums;

namespace SmartConfig.Core.Models;

[Table("UserConfig")]
public partial class UserConfig
{
    [Key]
    public int Id { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }
    public UserPreferences? UserPreferences { get; set; }
    public List<UserSetting>? UserSettings { get; set; }
    public UserConfigStatus Status { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset UpdatedUtc { get; set; }
}

public class UserPreferences
{
    public string Language { get; set; }
    public NotificationType? NotificationType { get; set; }
    public UserNotifications? UserNotifications { get; set; }
}

public class UserNotifications
{
    public bool NewsLetter { get; set; }
    public bool Billings { get; set; }
}

public class NotificationType
{
    public bool Email { get; set; }
    public bool Sms { get; set; }
}

public class UserSetting
{
    public string Key { get; set; }
    public string Value { get; set; }
}