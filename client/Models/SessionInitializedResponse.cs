namespace client.Models;

/// <summary>
/// A response for a request by which a session was initialized.
/// </summary>
public class SessionInitializedResponse
{
    /// <summary>
    /// Gets or sets the id of the session which was initialized.
    /// </summary>
    public Guid Session { get; set; }

    public SessionInitializedResponse(Guid session)
        => Session = session;
}