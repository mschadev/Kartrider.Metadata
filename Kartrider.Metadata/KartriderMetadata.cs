using FluentScheduler;

using Kartrider.Metadata.Extensions;
using Kartrider.Metadata.Json.Converter;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
namespace Kartrider.Metadata
{
    public class KartriderMetadata : IKartriderMetadata
    {
        public const string METADATA_DOWNLOAD_URL = "https://static.api.nexon.co.kr/kart/latest/metadata.zip";
        private const string CHARACTER_JSON_FILENAME = "character.json";
        private const string FLYINGPET_JSON_FILENAME = "flyingPet.json";
        private const string GAMETYPE_JSON_FILENAME = "gameType.json";
        private const string KART_JSON_FILENAME = "kart.json";
        private const string PET_JSON_FILENAME = "pet.json";
        private const string TRACK_JSON_FILENAME = "track.json";
        private const string SCHEDULE_NAME = "MetadataUpdateJob";
        private readonly KartriderMetadataOptions _options;
        private static readonly HttpClient _httpClient = new HttpClient();
        // Key: 메타데이터 타입, Value: Pair(Metadata key, value)
        private readonly Dictionary<string, Dictionary<string, string>> _metadataMap = new Dictionary<string, Dictionary<string, string>>();

        public event KartriderMetadataEvent.UpdatedEventHandler OnUpdated;

        public string MetadataPath
        {
            get
            {
                return _options.Path;
            }
        }
        public bool Init
        {
            get
            {
                return 0 < _metadataMap.Count;
            }
        }
        public KartriderMetadataOptions Options
        {
            get
            {
                return _options;
            }
        }
        public bool AutoUpdate
        {
            get
            {
                return !JobManager.GetSchedule(SCHEDULE_NAME).Disabled;
            }
        }
        public string this[MetadataType type, string key, string defaultValue = ""]
        {
            get
            {
                return this[type.ToString(), key, defaultValue];
            }
        }
        public string this[string metadataName, string key, string defaultValue = ""]
        {
            get
            {
                if (!_metadataMap[metadataName].ContainsKey(key))
                {
                    return defaultValue;
                }
                return _metadataMap[metadataName][key];
            }
        }
        public KartriderMetadata(KartriderMetadataOptions options)
        {
            _options = options;
            if (string.IsNullOrEmpty(_options.Path))
            {
                string tmpFileName = Path.GetTempFileName();
                File.Delete(tmpFileName);
                _options.Path = Path.GetTempPath() + Path.GetFileNameWithoutExtension(tmpFileName);
            }
            JobManager.Initialize();
            JobManager.AddJob(Job, Schedule(options));


        }

        private async void Job()
        {
            await UpdateMetadataFiles().ConfigureAwait(false);
            MetadataUpdate(Path.Combine(_options.Path, "metadata", CHARACTER_JSON_FILENAME), MetadataType.Character);
            MetadataUpdate(Path.Combine(_options.Path, "metadata", FLYINGPET_JSON_FILENAME), MetadataType.FlyingPet);
            MetadataUpdate(Path.Combine(_options.Path, "metadata", GAMETYPE_JSON_FILENAME), MetadataType.GameType);
            MetadataUpdate(Path.Combine(_options.Path, "metadata", KART_JSON_FILENAME), MetadataType.Kart);
            MetadataUpdate(Path.Combine(_options.Path, "metadata", PET_JSON_FILENAME), MetadataType.Pet);
            MetadataUpdate(Path.Combine(_options.Path, "metadata", TRACK_JSON_FILENAME), MetadataType.Track);
            OnUpdated?.Invoke(this, JobManager.GetSchedule(SCHEDULE_NAME).NextRun);
        }
        private Action<Schedule> Schedule(KartriderMetadataOptions options)
        {
            return (schedule) =>
            {
                schedule.WithName(SCHEDULE_NAME);
                if (options.UpdateNow && options.UpdateInterval != -1)
                {
                    schedule.ToRunNow().AndEvery(_options.UpdateInterval).Seconds();
                }
                else if (options.UpdateInterval != -1)
                {
                    schedule.ToRunEvery(_options.UpdateInterval).Seconds();
                }
                else
                {
                    schedule.ToRunNow();
                }
            };
        }
        public string Get<T>(T type, string key, string defaultValue = "") where T : System.Enum
        {
            return this[type.ToString(), key, defaultValue];
        }
        public void AutoUpdateStop()
        {
            JobManager.GetSchedule(SCHEDULE_NAME).Disable();
        }
        public void AutoUpdateStart()
        {
            JobManager.GetSchedule(SCHEDULE_NAME).Enable();
        }
        private async Task UpdateMetadataFiles()
        {
            string zipPath = $"{_options.Path}.zip";
            await DownloadMeatadataAsync(zipPath);
            using (FileStream fileStream = new FileStream(zipPath, FileMode.Open))
            {
                ZipArchive zipArchive = new ZipArchive(fileStream);
                zipArchive.ExtractToDirectory(_options.Path, true);
            }
            File.Delete(zipPath);

        }
        public void MetadataUpdate<T>(string path, T type, bool overwrite = true) where T : System.Enum
        {
            MetadataUpdate(path, type.ToString(), overwrite);
        }
        public void MetadataUpdate(string path, MetadataType type, bool overwrite = true)
        {
            MetadataUpdate(path, type.ToString(), overwrite);
        }
        public void MetadataUpdate(string path, string metadataName, bool overwrite = true)
        {
            string content = File.ReadAllText(path);
            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(content, new JsonSerializerOptions()
            {
                Converters =
                {
                    new MetadataJsonConverter()
                }
            });
            if (!_metadataMap.ContainsKey(metadataName))
            {
                _metadataMap.Add(metadataName, new Dictionary<string, string>());
            }
            foreach (var pair in map)
            {
                if (_metadataMap[metadataName].ContainsKey(pair.Key) && overwrite)
                {
                    _metadataMap[metadataName][pair.Key] = pair.Value;
                }
                else if (!_metadataMap.ContainsKey(pair.Key))
                {
                    _metadataMap[metadataName].Add(pair.Key, pair.Value);
                }
            }
        }
        public static async Task DownloadMeatadataAsync(string path, CancellationToken cancellationToken = default)
        {
            var res = await _httpClient.GetAsync(METADATA_DOWNLOAD_URL, cancellationToken).ConfigureAwait(false);
            byte[] binaryContent = await res.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(path, binaryContent);
        }
    }
}
