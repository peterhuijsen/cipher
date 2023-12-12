namespace client.Models;

/// <summary>
/// A response for a request by which a message was sent to a target.
/// </summary>
public class SessionTransmittedResponse
{
    /// <summary>
    /// Gets or sets the session in which the message was sent.
    /// </summary>
    public Guid Session { get; set; }
    
    /// <summary>
    /// Gets or sets the message which was sent.
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Gets or sets the target to which the message was sent.
    /// </summary>
    public string Target { get; set; }

    public SessionTransmittedResponse(Guid session, string message, string target)
    {
        Session = session;
        Message = message;
        Target = target;
    }
}