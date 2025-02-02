using System;

namespace DestinyTrail.Engine
{
    public sealed class StatusData : GameData<Status>, IStatusData
    {
        private static readonly Lazy<StatusData> _instance = new(() => new StatusData());

        private IUtility? _utility;

        public List<Status> Statuses { get => _items; set => _items = value; }

        public static StatusData Instance => _instance.Value;

        private StatusData() { }

        public async Task CreateAsync(IUtility utility)
        {
            if (utility == null) throw new NullReferenceException("utility param null");
            _utility = utility;
            await GetStatusesAsync();
        }

        private async Task GetStatusesAsync()
        {
            string statusesFilePath = _utility!.GetAppSetting("StatusesFilePath");
            var statusYaml = await _utility.LoadYamlAsync<StatusData>(statusesFilePath);
            _items = [.. statusYaml];
        }
    }
}
