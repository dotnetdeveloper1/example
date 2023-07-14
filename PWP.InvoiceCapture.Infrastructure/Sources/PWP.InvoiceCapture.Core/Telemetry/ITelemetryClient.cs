using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Core.Telemetry
{
    public interface ITelemetryClient
    {
        void Track(ITelemetry telemetry);
        void TrackAvailability(AvailabilityTelemetry telemetry);
        void TrackAvailability(string name, DateTimeOffset timeStamp, TimeSpan duration, string runLocation, bool success, string message = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
        void TrackDependency(DependencyTelemetry telemetry);
        void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success);
        void TrackDependency(string dependencyTypeName, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success);
        void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
        void TrackEvent(EventTelemetry telemetry);
        void TrackException(ExceptionTelemetry telemetry);
        void TrackException(Exception exception);
        void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
        void TrackMetric(MetricTelemetry telemetry);
        void TrackMetric(string name, double value, IDictionary<string, string> properties = null);
        void TrackRequest(RequestTelemetry request);      
        void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success);
        void TrackTrace(TraceTelemetry telemetry);
        void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties);
        void TrackTrace(string message, IDictionary<string, string> properties);
        void TrackTrace(string message, SeverityLevel severityLevel);
        void TrackTrace(string message);
        IOperation StartOperation(string operationName);
    }
}
