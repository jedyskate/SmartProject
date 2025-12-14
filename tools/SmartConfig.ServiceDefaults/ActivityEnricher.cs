using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace SmartConfig.ServiceDefaults;

public class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentSpanId", activity.ParentSpanId.ToString()));
        }
        else
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", string.Empty));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", string.Empty));
        }
    }
}
