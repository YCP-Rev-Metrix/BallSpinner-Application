using Client;

namespace RevMetrix.BallSpinner.BackEnd;

/// <summary>
/// The core ball spinner application
/// </summary>
public class Backend : IDisposable
{
    /// <summary>
    /// <inheritdoc cref="IFrontEnd"/>
    /// </summary>
    public IFrontEnd FrontEnd { get; private set; } = null!;

    /// <summary>
    /// Initializes the back end.
    /// Does not use constructor because the back end and front end both depend on each other to get an initial object reference.
    /// </summary>
    public void Init(IFrontEnd frontEnd)
    {
        FrontEnd = frontEnd;
    }

    /// <inheritdoc/>
    public void Dispose()
    {

    }
}