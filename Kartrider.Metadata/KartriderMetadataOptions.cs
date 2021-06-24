namespace Kartrider.Metadata
{
    public class KartriderMetadataOptions
    {
        public string Path { get; set; }
        public int UpdateInterval { get; set; } = 1;
        public bool UpdateNow { get; set; } = false;
    }
}
