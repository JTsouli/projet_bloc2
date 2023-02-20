using System;
using System.Diagnostics;

class logMetier
{
    static void Main(string[] args)
    {
        // Vérifier si Word est en cours d'exécution
        if (Process.GetProcessesByName("winword").Length > 0)
        {
            // Afficher un message d'erreur et arrêter l'exécution du programme
            Console.WriteLine("Un logiciel métier est en cours d'exécution. Fermez-le et réessayez.");
            return; //Si un ou plusieurs processus sont trouvés, le code affiche un message d’erreur à l’utilisateur et arrête l’exécution du programme en utilisant l’instruction « return ».
        }
        // Instructions de votre programme
    }
}