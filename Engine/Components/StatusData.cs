using System.Collections.Generic;

namespace DestinyTrail.Engine
{
    public class StatusData : GameData<Status>, IStatusData
    {
        private IUtility? _utility;
        public required List<Status> Statuses{ get => _items; set => _items = value; }

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
            Statuses = [.. statusYaml];
        }
    }

    public interface IStatusData
    {   
        public Task CreateAsync(IUtility _utility);
        public List<Status> Statuses{ get; set; }
    }
}
