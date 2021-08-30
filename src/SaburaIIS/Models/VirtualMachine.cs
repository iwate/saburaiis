namespace SaburaIIS.Models
{
    public class VirtualMachine
    {
        public string id { get; set; }

        private string _scaleSetName;
        public string ScaleSetName
        {
            get => _scaleSetName;
            set
            {
                _scaleSetName = value;
                id = $"{_scaleSetName}|{_name}";
            }
        }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                id = $"{_scaleSetName}|{_name}";
            }
        }
        public Snapshot Current { get; set; }
    }
}
