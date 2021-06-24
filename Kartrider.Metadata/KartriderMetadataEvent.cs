using System;

namespace Kartrider.Metadata
{
    public static class KartriderMetadataEvent
    {
        public delegate void UpdatedEventHandler(KartriderMetadata kartriderMetadata, DateTime nextRun);
    }
}
