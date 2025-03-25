using DestinyTrail.Engine.Interfaces;
using System.Collections.Generic;

namespace DestinyTrail.Engine.Extensions
{
    public static class WagonPartyExtensions
    {
        public static void SetPlayerNames(this IWagonParty wagonParty, List<string> names)
        {
            if (names == null || names.Count == 0)
            {
                throw new ArgumentException("At least one player name must be provided.");
            }

            // If names exceed current party size, add new members
            while (wagonParty.Members.Count < names.Count)
            {
                var newPerson = CreatePerson("Unnamed");
                wagonParty.Members.Add(newPerson);
            }

            // Set names for existing members
            for (int i = 0; i < names.Count; i++)
            {
                wagonParty.Members[i].Name = names[i];
            }

            // Update the leader if it's not set or no longer exists
            if (wagonParty is WagonParty party)
            {
                if (party.Leader == null || !party.Members.Contains(party.Leader))
                {
                    party.Leader = party.Members.FirstOrDefault();
                }
            }
        }

        private static IPerson CreatePerson(string name)
        {
            return new Person
            {
                ID = 0, // You might want to implement a better ID generation strategy
                Name = name,
                Status = new Status { Name = "Good" }
            };
        }
    }
}