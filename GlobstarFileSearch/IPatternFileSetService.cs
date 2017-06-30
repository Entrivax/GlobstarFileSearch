namespace GlobstarFileSearch
{
    public interface IPatternFileSetService
    {
        /// <summary>
        /// Détermine si l'expression fournie semble être un modèle pouvant cibler plusieurs fichiers
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        bool IsPattern(string expr);

        /// <summary>
        /// Recherche des fichiers selon un modèle
        /// </summary>
        /// <param name="sourceDir">Chemin absolu du répertoire à partir duquel rechercher les fichiers</param>
        /// <param name="sourcePattern">Modèle de filtrage des fichiers relatif à <paramref name="sourceDir"/></param>
        /// <param name="relSrcDir">Répertoire relatif parent du premier élément possédant une étoile dans <paramref name="sourcePattern"/></param>
        /// <param name="relFilePaths">Liste des fichier relatifs à <paramref name="relSrcDir"/> ayant matchés avec le <paramref name="sourcePattern"/></param>
        void Execute(string sourceDir, string sourcePattern, out string relSrcDir, out string[] relFilePaths);
    }
}
