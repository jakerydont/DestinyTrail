namespace DestinyTrail.Engine
{
    public interface IStatusData
    {   
        public Task CreateAsync(IUtility _utility);
        public List<Status> Statuses{ get; set; }
    }
}
