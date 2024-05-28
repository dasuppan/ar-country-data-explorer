using System.Collections.Generic;

public struct ActiveInfoCategoriesChangedEventArgs
{
    /// <summary>
    /// The list of <see cref="InfoCategory"/>s added since the last event.
    /// </summary>
    public List<InfoCategory> added { get; private set; }

    /// <summary>
    /// The list of <see cref="InfoCategory"/>s udpated since the last event.
    /// </summary>
    /*public List<InfoCategory> updated { get; private set; }*/

    /// <summary>
    /// The list of <see cref="InfoCategory"/>s removed since the last event.
    /// </summary>
    public List<InfoCategory> removed { get; private set; }
        
}