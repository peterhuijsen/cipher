using Cipher.Settings;
using client.Models;

namespace client.Services;

/// <summary>
/// An interface for a store of cipher sessions.
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// Register a new session by the type of cipher used and the data of the symmetric key used.
    /// </summary>
    /// <param name="type">The <see cref="CipherType"/> of the new session.</param>
    /// <param name="key">The decoded data of the symmetric key of the new session.</param>
    /// <returns>The id of the new session.</returns>
    Guid Register(CipherType type, byte[] key);
    
    /// <summary>
    /// Register a new session by the type of cipher used and the data of the symmetric key used.
    /// </summary>
    /// <param name="id">The id of the new session.</param>
    /// <param name="type">The <see cref="CipherType"/> of the new session.</param>
    /// <param name="key">The decoded data of the symmetric key of the new session.</param>
    /// <returns>The id of the new session.</returns>
    Guid Register(Guid id, CipherType type, byte[] key);

    /// <summary>
    /// Retrieve a earlier registered session from the store by the given id.
    /// </summary>
    /// <param name="id">The id by which the session was registered.</param>
    /// <returns>The found session corresponding to the given id, or null if no session could be found.</returns>
    Session? Retrieve(Guid id);
    
    /// <summary>
    /// Retrieve a earlier registered session from the store by the given id.
    /// </summary>
    /// <param name="key">The key by which the session was registered.</param>
    /// <returns>The found session corresponding to the given key, or null if no session could be found.</returns>
    Session? Retrieve(byte[] key);
}

/// <summary>
/// An simple local cache implementation of the <see cref="ISessionStore"/>.
/// </summary>
public class SessionStore : ISessionStore
{
    /// <summary>
    /// A local cache of the registered sessions to the current instance of the webapplication.
    /// </summary>
    private readonly Dictionary<Guid, Session> _sessions = new();
    
    /// <inheritdoc cref="ISessionStore.Register(Cipher.Settings.CipherType,byte[])"/>
    public Guid Register(CipherType type, byte[] key)
    {
        if (_sessions.Any(s => s.Value.Key.SequenceEqual(key)))
            return Retrieve(key)!.Id;

        var session = new Session(type, key);
        _sessions.Add(session.Id, session);
        
        return session.Id;
    }
    
    /// <inheritdoc cref="ISessionStore.Register(Guid,Cipher.Settings.CipherType,byte[])"/>
    public Guid Register(Guid id, CipherType type, byte[] key)
    {
        if (_sessions.Any(s => s.Value.Key.SequenceEqual(key)))
            return Retrieve(key)!.Id;

        var session = new Session(id, type, key);
        _sessions.Add(id, session);
        
        return id;
    }

    /// <inheritdoc cref="ISessionStore.Retrieve(System.Guid)"/>
    public Session? Retrieve(Guid id)
    {
        if (!_sessions.TryGetValue(id, out var key))
            return null;

        return key;
    }

    /// <inheritdoc cref="ISessionStore.Retrieve(byte[])"/>
    public Session? Retrieve(byte[] key)
        => _sessions.FirstOrDefault(s => s.Value.Key.SequenceEqual(key)).Value;
}