using SaburaIIS.POCO;
using System;
using System.Collections.Generic;

namespace SaburaIIS.Models
{
    public class Snapshot
    {
        public string id { get; set; }

        private string _scaleSetName;
        public string ScaleSetName
        {
            get => _scaleSetName;
            set
            {
                _scaleSetName = value;
                id = $"{_scaleSetName}|{_name}|{_timestamp.Ticks}";
            }
        }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                id = $"{_scaleSetName}|{_name}|{_timestamp.Ticks}";
            }
        }

        private DateTimeOffset _timestamp;
        public DateTimeOffset Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                id = $"{_scaleSetName}|{_name}|{_timestamp.Ticks}";
            }
        }
        public ICollection<ApplicationPool> ApplicationPools { get; set; }
        public ICollection<Site> Sites { get; set; }
    }
}
