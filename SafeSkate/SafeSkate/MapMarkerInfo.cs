using System;
using System.Xml.Serialization; // Make sure this is included for XML serialization attributes

namespace SafeSkate
{
    [Serializable]
    public class MapMarkerInfo
    {
        // Fields are now private and not directly serialized.
        // Serialization will use public properties instead.
        private Coordinate location;
        private string uploader;
        private DateTime timeUploaded;
        private Severity severity;

        public MapMarkerInfo() { }

        public MapMarkerInfo(Coordinate location, string uploader, DateTime timeUploaded, Severity severity)
        {
            this.location = location;
            this.uploader = uploader;
            this.timeUploaded = timeUploaded;
            this.severity = severity;
        }

        public Coordinate Location
        {
            get => this.location;
            set => this.location = value; 
        }

        public string Uploader
        {
            get => this.uploader;
            set => this.uploader = value; 
        }

        public DateTime TimeUploaded
        {
            get => this.timeUploaded;
            set => this.timeUploaded = value; 
        }

        public Severity Severity
        {
            get => this.severity;
            set => this.severity = value;
        }
        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine($"Location: {this.Location}");
            builder.AppendLine($"   Uploader: {this.Uploader}");
            builder.AppendLine($"   Time Uploaded: {this.TimeUploaded}");
            builder.AppendLine($"   Severity: {this.Severity}");
            return builder.ToString();
        }

    }
}
