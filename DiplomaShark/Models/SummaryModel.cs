namespace DiplomaShark.Models
{
    internal class SummaryModel
    {
        public string? IP { get; set; }
        public int? SendedPackets { get; set; }
        public int? ReceivedPackets { get; set; }
        private string? _info;
        public string? Info
        {
            get
            {
                if (_info == null)
                {
                    return "Нет информации";
                }
                else
                {
                    return _info;
                }
            }
            set
            {
                _info = value;
            }
        }
    }
}
