using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class JwtService
{
    private readonly IConfiguration _config;
    
    public JwtService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public (string,int) GenerateToken(User user)
    {
     // Recupération de la configuration JWT
            //On lit la clé secrète depuis Env
            // Si elle n'existe pas, on envoie une exception avec un message clair
            var secretKey = _config["JWT_SECRET"]
                ?? throw new InvalidOperationException("JWT_SECRET manquant.");

            // duree de validité du token , par defaut un jours si non configuré
            var expirationMinutes = int.Parse(_config["JWT_EXPIRATION_MINUTES"] ?? "1");

            // Generation du token

            //L'outil qui sait créer et lire des jwt
            var tokenHandler = new JwtSecurityTokenHandler();

            // pour l'algo de signature
            // on a besoin que clé secrete soit converti en tableau de bytes
            var key = Encoding.ASCII.GetBytes(secretKey);

            //Date d'expiration du token
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            // Recette du token : ce qu'il contient et comment le signer
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //On va mettre les infos de la "carte magnétique"
                Subject = new ClaimsIdentity([
                    // Claim 1 : L'id de l'utilisateur 
                    // Utile pour savoir qui fait la requete coté server
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                    //Claim 2 : Le role de l'utilisateur
                    //Utilisé pour vérifier les droits [authorize(Roles="Admin")]
                    new Claim(ClaimTypes.Role, user.Role.ToString()),

                    //Claim 3 : L'email de l'utilisateur
                    //utile pour afficher coté client ou dans les logs
                    new Claim(ClaimTypes.Name, user.Username)
                ]),

                //Date d'expiration
                Expires = expiration,

                //Donner une signature numérique
                // Empeche la falsification du token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["JWT_ISSUER"],
                Audience = _config["JWT_AUDIENCE"]
            };

            // Fabrication du token à partir de la recette
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), expirationMinutes);
    }
}