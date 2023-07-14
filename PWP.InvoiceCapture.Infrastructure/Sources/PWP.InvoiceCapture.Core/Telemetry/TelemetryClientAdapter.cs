using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Core.Telemetry
{
    public class TelemetryClientAdapter : ITelemetryClient
    {
        public TelemetryClientAdapter(TelemetryClient telemetryClient) 
        {
            Guard.IsNotNull(telemetryClient, nameof(telemetryClient));

            this.telemetryClient = telemetryClient;
        }

        public void Track(ITelemetry telemetry) => 
            telemetryClient.Track(telemetry);

        public void TrackAvailability(AvailabilityTelemetry telemetry) => 
            telemetryClient.TrackAvailability(telemetry);

        public void TrackAvailability(string name, DateTimeOffset timeStamp, TimeSpan duration, string runLocation, bool success, string message = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null) =>
            telemetryClient.TrackAvailability(name, timeStamp, duration, runLocation, success, message, properties, metrics);

        public void TrackDependency(DependencyTelemetry telemetry) => 
            telemetryClient.TrackDependency(telemetry);

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success) =>
            telemetryClient.TrackDependency(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, success);

        public void TrackDependency(string dependencyTypeName, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success) =>
            telemetryClient.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success);

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null) =>
            telemetryClient.TrackEvent(eventName, properties, metrics);

        public void TrackEvent(EventTelemetry telemetry) => 
            telemetryClient.TrackEvent(telemetry);

        public void TrackException(Exception exception) => 
            telemetryClient.TrackException(exception);

        public void TrackException(ExceptionTelemetry telemetry) => 
            telemetryClient.TrackException(telemetry);

        public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null) =>
            telemetryClient.TrackException(exception, properties, metrics);

        public void TrackMetric(MetricTelemetry telemetry) => 
            telemetryClient.TrackMetric(telemetry);

        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null) =>
            telemetryClient.TrackMetric(name, value, properties);

        public void TrackRequest(RequestTelemetry request) =>
            telemetryClient.TrackRequest(request);

        public void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success) =>
            telemetryClient.TrackRequest(name, startTime, duration, responseCode, success);

        public void TrackTrace(TraceTelemetry telemetry) =>
            telemetryClient.TrackTrace(telemetry);

        public void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties) =>
            telemetryClient.TrackTrace(message, severityLevel, properties);

        public void TrackTrace(string message, IDictionary<string, string> properties) =>
            telemetryClient.TrackTrace(message, properties);

        public void TrackTrace(string message, SeverityLevel severityLevel) =>
            telemetryClient.TrackTrace(message, severityLevel);

        public void TrackTrace(string message) =>
            telemetryClient.TrackTrace(message);

        public IOperation StartOperation(string operationName)
        {
            return new OperationHolderAdapter(
                telemetryClient.StartOperation<RequestTelemetry>(operationName));
        }

        private readonly TelemetryClient telemetryClient;
    }
}
