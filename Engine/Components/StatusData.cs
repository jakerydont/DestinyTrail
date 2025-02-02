using System;

namespace DestinyTrail.Engine
{
    public sealed class StatusData : GameData<Status>, IStatusData
    {        
        private static readonly Lazy<StatusData> _instance = new(() => new StatusData());

        public static StatusData Instance => _instance.Value;

        private IUtility? _utility;
        public List<Status> Statuses { get => _items; set => _items = value; }

        private StatusData() { }

        public async Task InitializeAsync(IUtility utility)
        {
            if (_utility != null)
                throw new InvalidOperationException("StatusData is already initialized.");

            _utility = utility ?? throw new ArgumentNullException(nameof(utility));

            await GetStatusesAsync();
        }

        private async Task GetStatusesAsync()
        {
            string statusesFilePath = _utility!.GetAppSetting("StatusesFilePath");

            // Deserialize directly into the list of statuses
            var statusYaml = await _utility.LoadYamlAsync<List<Status>>(statusesFilePath);

            Statuses = statusYaml ?? new List<Status>(); // Avoid null assignment
        }
    }
}
