using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    public class MapMarkerInfo
    {
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

        public Coordinate Location => this.location;
        public string Uploader => this.uploader;
        public DateTime TimeUploaded => this.timeUploaded;
        public Severity Severity => this.severity;

    }
}
