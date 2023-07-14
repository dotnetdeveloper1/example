using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Core.Telemetry
{
    public class EmptyTelemetryClient : ITelemetryClient
    {
        public IOperation StartOperation(string operationName) => new EmptyOperation();

        public void Track(ITelemetry telemetry)
        {
        }

        public void TrackAvailability(AvailabilityTelemetry telemetry)
        { 
        }

        public void TrackAvailability(string name, DateTimeOffset timeStamp, TimeSpan duration, string runLocation, bool success, string message = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
        }

        public void TrackDependency(DependencyTelemetry telemetry)
        {
        }

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
        }

        public void TrackDependency(string dependencyTypeName, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
        }

        public void TrackEvent(EventTelemetry telemetry)
        {
        }

        public void TrackException(Exception exception)
        {
        }

        public void TrackException(ExceptionTelemetry telemetry)
        {
        }

        public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
        }

        public void TrackMetric(MetricTelemetry telemetry)
        {
        }

        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
        }

        public void TrackRequest(RequestTelemetry request)
        {
        }

        public void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success)
        {
        }

        public void TrackTrace(TraceTelemetry telemetry)
        {
        }

        public void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties)
        {
        }

        public void TrackTrace(string message, IDictionary<string, string> properties)
        {
        }

        public void TrackTrace(string message, SeverityLevel severityLevel)
        {
        }

        public void TrackTrace(string message)
        {
        }
    }
}
