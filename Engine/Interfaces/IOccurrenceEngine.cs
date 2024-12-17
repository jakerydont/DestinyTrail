using System;

namespace DestinyTrail.Engine
{
    public interface IOccurrenceEngine
    {

        string[] Statuses { get; }

        /// <summary>
        /// Picks a random occurrence based on the probabilities of each occurrence.
        /// </summary>
        /// <returns>A randomly selected <see cref="Occurrence"/>.</returns>
        Occurrence PickRandomOccurrence();

        /// <summary>
        /// Processes an occurrence by applying its effects, including updating a party member's status.
        /// </summary>
        /// <param name="occurrence">The occurrence to process.</param>
        /// <returns>The processed <see cref="Occurrence"/> with updated display text.</returns>
        Occurrence ProcessOccurrence(Occurrence occurrence);

        void SetStatus(string status);

        void ClearStatus();

        void TrySetFlag(IOccurrence occurrence);
        void TrySetQuantityInventoryItem(IOccurrence occurrence);

        void TryIncreaseInventoryItem(IOccurrence occurrence);

        void TryDecreaseInventoryItem(IOccurrence occurrence);

    }
}
