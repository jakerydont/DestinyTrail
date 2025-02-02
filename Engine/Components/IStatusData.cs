namespace DestinyTrail.Engine
{
    public interface IStatusData
    {   
        public Task InitializeAsync(IUtility _utility);
        public List<Status> Statuses{ get; set; }
    }
}
