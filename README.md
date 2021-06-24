# Kartrider.Metadata
[![standard-readme compliant](https://img.shields.io/badge/standard--readme-OK-green.svg)](https://github.com/RichardLitt/standard-readme)  ![](https://img.shields.io/nuget/dt/Kartrider.Metadata)  

카트라이더 메타데이터 관리 라이브러리
### 특징
+ 자동 메타데이터 업데이트
+ 확장 가능한 메타데이터
+ ASP.NET Core 지원  

| Package                       | Description                                          | Link |
|-------------------------------|------------------------------------------------------|------|
| Kartrider.Metadata            | Nexon Kartrider metadata management for .NET         |   ![](https://img.shields.io/nuget/vpre/Kartrider.Metadata)   |
| Kartrider.Metadata.AspNetCore | Nexon Kartrider metadata management for ASP.NET Core |   ![](https://img.shields.io/nuget/vpre/Kartrider.Metadata.AspNetCore)   |
## Table of Contents

- [Install](#install)
- [Usage](#usage)
- [Maintainers](#maintainers)
- [Contributing](#contributing)
- [License](#license)

## Install

```sh
Install-Package Kartrider.Metadata.AspNetCore -Version 0.1.0
```

## Usage
```cs
using System;
using System.IO;

namespace ConsoleApp
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
            kartriderMetadata.OnUpdated += KartriderMetadata_OnUpdated;
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        private static void KartriderMetadata_OnUpdated(KartriderMetadata kartriderMetadata, DateTime nextRun)
        {
            Console.WriteLine("메타데이터 업데이트 완료");
            // 파라곤 X
            Console.WriteLine(kartriderMetadata[MetadataType.Kart, "d47aa62de79d88ecee263e07456555d99ff8957f1760d0f248667913acbc2b67"]);
            Directory.Delete("metadata", true);
        }
    }
}
```
더 많은 예제는 Examples에 있는 프로젝트를 참고하세요.
## Maintainers

[@zxc010613](https://github.com/zxc010613)

## Contributing

PRs accepted.

## License
[MIT](./LICENSE)
