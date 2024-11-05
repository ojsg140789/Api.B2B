using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.B2B.Services
{
    public class BlacklistService
    {
        private readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
        private readonly Dictionary<string, DateTime> _tokenExpiry = new Dictionary<string, DateTime>();

        // Añadir un token a la lista negra con la fecha de expiración
        public void BlacklistToken(string token, DateTime expiry)
        {
            _blacklistedTokens.Add(token);
            _tokenExpiry[token] = expiry;
        }

        // Verificar si un token está en la lista negra y si ha expirado
        public bool IsTokenBlacklisted(string token)
        {
            if (_blacklistedTokens.Contains(token))
            {
                // Si el token ha expirado, lo eliminamos de la lista negra
                if (_tokenExpiry.TryGetValue(token, out var expiry) && expiry < DateTime.UtcNow)
                {
                    _blacklistedTokens.Remove(token);
                    _tokenExpiry.Remove(token);
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
