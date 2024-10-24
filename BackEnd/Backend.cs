using Client;

namespace RevMetrix.BallSpinner.BackEnd;

/// <summary>
/// The core ball spinner application
/// </summary>
public class Backend : IDisposable
{
    /// <inheritdoc cref="IFrontEnd"/>
    public IFrontEnd FrontEnd { get; private set; } = null!;

    /// <summary>
    /// Initializes the back end. The FrontEnd is not fully initialized, but is still accessible.
    /// Does not use constructor because the back end and front end both depend on each other to get an initial object reference.
    /// </summary>
    public void Init(IFrontEnd frontEnd)
    {
        Console.WriteLine("BackEnd Init");

        FrontEnd = frontEnd;
    }

    /// <inheritdoc/>
    public void Dispose()
    {

    }
}