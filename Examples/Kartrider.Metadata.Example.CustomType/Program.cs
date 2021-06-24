using System;
using System.IO;

namespace Kartrider.Metadata.Example.CustomType
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists("metadata"))
            {
                Directory.CreateDirectory("metadata");
            }
            IKartriderMetadata kartriderMetadata = new KartriderMetadata(new KartriderMetadataOptions()
            {
                Path = "metadata", // 메타데이터 폴더 경로
                UpdateInterval = 30, // 초 단위
                UpdateNow = false // 초기화하자마자 업데이트 할 것인지
            });
            kartriderMetadata.MetadataUpdate("channel.json", MetadataCustomType.Channel);
            Console.WriteLine(kartriderMetadata.Get(MetadataCustomType.Channel, "speedIndiFastest"));
            Console.WriteLine(kartriderMetadata.Get(MetadataCustomType.Channel, "1", "없는 채널"));
            Console.ReadKey();
        }
    }
}
