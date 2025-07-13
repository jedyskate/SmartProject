using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace SmartConfig.Data.Extensions;

public static class ValueConversionExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class, new()
    {
        ValueConverter<T, string> converter = new ValueConverter<T, string>
        (
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<T>(v) ?? new T()
        );

        ValueComparer<T> comparer = new ValueComparer<T>
        (
            (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
            v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
            v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v)) ?? new T()
        );

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        propertyBuilder.HasColumnType("nvarchar(max)");

        return propertyBuilder;
    }

    public static PropertyBuilder<T> HasEnumCollectionConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        ValueConverter<ICollection<T>, string> converter = new ValueConverter<ICollection<T>, string>
        (
            v => JsonConvert.SerializeObject(v.Select(e => e.ToString()).ToList()),
            v => (JsonConvert.DeserializeObject<ICollection<string>>(v) ?? new List<string>())
                .Select(e => (T)Enum.Parse(typeof(T), e)).ToList()
        );

        ValueComparer<ICollection<T>> comparer = new ValueComparer<ICollection<T>>
        (
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => (ICollection<T>)c.ToHashSet()
        );

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        propertyBuilder.HasColumnType("nvarchar(max)");

        return propertyBuilder;
    }
}