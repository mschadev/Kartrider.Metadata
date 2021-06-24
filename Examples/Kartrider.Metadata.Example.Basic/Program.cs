using System;
using System.IO;
using System.Threading;

namespace Kartrider.Metadata.Example.Basic
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
                UpdateNow = true // 초기화하자마자 업데이트 할 것인지
            });
            Console.WriteLine("Hello World!");
            while (!kartriderMetadata.Init)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("메타데이터 업데이트 완료");
            Console.WriteLine(kartriderMetadata[MetadataType.Kart, "d47aa62de79d88ecee263e07456555d99ff8957f1760d0f248667913acbc2b67"]);
            Console.WriteLine(kartriderMetadata[MetadataType.Kart, "1", "알 수 없음"]);
            Directory.Delete("metadata", true);
        }
    }
}
