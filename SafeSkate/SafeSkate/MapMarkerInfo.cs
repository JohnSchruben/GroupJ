using System.Reflection;

namespace SafeSkate
{
    /// <summary>
    ///   Ticket #5 The Service Types
    /// </summary>
    [Serializable]
    public class MapMarkerInfo : IEquatable<MapMarkerInfo>
    {
        private Coordinate location;
        private string description = "Hazard";
        private DateTime timeUploaded;
        private Severity severity;
        private Guid id;

        public MapMarkerInfo()
        { 
            this.Id = Guid.NewGuid();
        }

        public MapMarkerInfo(Coordinate location, string description, DateTime timeUploaded, Severity severity)
        {
            this.location = location;
            this.description = description;
            this.timeUploaded = timeUploaded;
            this.severity = severity;
            this.Id = Guid.NewGuid();
        }

        public Coordinate Location
        {
            get => this.location;
            set => this.location = value;
        }

        public string Description
        {
            get => this.description;
            set => this.description = value;
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
        public Guid Id
        {
            get => this.id;
            set => this.id = value;
        }

        public bool Equals(MapMarkerInfo? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.Location.Equals(other.Location) &&
                   this.Id.Equals(other.Id) &&
                   string.Equals(this.Description, Description) &&
                   this.TimeUploaded == other.TimeUploaded &&
                   this.Severity == other.Severity;
        }

        public string SeverityDescription()
        {
            return this.severity == Severity.FlintstonesVitamin ? "Low" :
                this.severity == Severity.BabyAspirin ? "Medium" :
                this.severity == Severity.Morphine ? "High" : "Ultra";
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine($"Location: {this.Location}");
            builder.AppendLine($"   Description: {this.Description}");
            builder.AppendLine($"   Time Uploaded: {this.TimeUploaded}");
            builder.AppendLine($"   Severity: {this.Severity}");
            return builder.ToString();
        }
    }
}