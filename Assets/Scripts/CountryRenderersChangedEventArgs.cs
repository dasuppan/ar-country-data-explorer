using System.Collections.Generic;

public struct CountryRenderersChangedEventArgs
{
    /// <summary>
    /// The list of <see cref="CountryRenderer"/>s added since the last event.
    /// </summary>
    public List<CountryRenderer> added { get; private set; }

    /// <summary>
    /// The list of <see cref="CountryRenderer"/>s udpated since the last event.
    /// </summary>
    /*public List<CountryRenderer> updated { get; private set; }*/

    /// <summary>
    /// The list of <see cref="CountryRenderer"/>s removed since the last event.
    /// </summary>
    public List<CountryRenderer> removed { get; private set; }
}