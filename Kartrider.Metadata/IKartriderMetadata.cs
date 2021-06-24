using System;

namespace Kartrider.Metadata
{
    public interface IKartriderMetadata
    {
        event KartriderMetadataEvent.UpdatedEventHandler OnUpdated;
        string MetadataPath { get; }
        bool Init { get; }
        KartriderMetadataOptions Options { get; }
        bool AutoUpdate { get; }
        string this[MetadataType type, string key, string defaultValue = ""] { get; }
        string this[string metadataName, string key, string defaultValue = ""] { get; }
        string Get<T>(T type, string key, string defaultValue = "") where T : Enum;
        void AutoUpdateStop();
        void AutoUpdateStart();
        void MetadataUpdate<T>(string path, T type, bool overwrite = true) where T : System.Enum;
        void MetadataUpdate(string path, MetadataType type, bool overwrite = true);
        void MetadataUpdate(string path, string metadataName, bool overwrite = true);
    }
}
