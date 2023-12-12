using Cipher.Settings;

namespace client.Models;

/// <summary>
/// A model storing data about an active session with another client.
/// </summary>
public class Session
{
    /// <summary>
    /// Gets or sets the id of the session.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the type of cipher which is used in the session.
    /// </summary>
    public CipherType Cipher { get; set; }
    
    /// <summary>
    /// Gets or sets the key which is used in the session.
    /// </summary>
    public byte[] Key { get; set; }

    public Session(Guid id, CipherType cipher, byte[] key)
    {
        Id = id;
        
        Cipher = cipher;
        Key = key;
    }
    
    public Session(CipherType cipher, byte[] key)
    {
        Id = Guid.NewGuid();
        
        Cipher = cipher;
        Key = key;
    }
}