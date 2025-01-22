using System;

namespace DestinyTrail.Engine;

public class PersonName : GameComponent
{
    // Implicit conversion from string to PersonName
    public static implicit operator PersonName(string name) => new PersonName { Name = name };

    // Implicit conversion from PersonName to string
    public static implicit operator string(PersonName personName) => personName.Name;

}
