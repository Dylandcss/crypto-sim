namespace AuthService.Services;

    //Service pour le hashage et la verification des mots de passe
    // Utilise Bcrypt
    public static class PasswordService
    {
        // Nombre de tous de clé du coffre fort
        // Plus c'est haut, plus c'est difficile et long à verifier(pour nous et pour les pirates)
        private const int WorkFactor = 11;

        //Hash un mot de passe en clair
        // A utiliser lors de l'inscriptions ou du changement de mot de passe
        public static string HashPassword(string motDePasse)
        {
            //genere automatiquement un salt aléatoire
            return BCrypt.Net.BCrypt.HashPassword(motDePasse, WorkFactor);
        }

        //Verifie si un mot de passe correspond à un hash
        public static bool VerifyPassword(string motDePasse, string motDePasseHashe)
        {
            // retourne true si les hash correspondent
            return BCrypt.Net.BCrypt.Verify(motDePasse, motDePasseHashe);
        }
    }
