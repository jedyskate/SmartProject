using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SmartConfig.Core.Models;

[ModelMetadataType(typeof(IUserConfigMetadata))]
public partial class UserConfig : IUserConfigMetadata
{
}

public interface IUserConfigMetadata
{
    [JsonIgnore]
    public int Id { get; set; }
}