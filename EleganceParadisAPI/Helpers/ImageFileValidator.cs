using Microsoft.IdentityModel.Tokens;

namespace EleganceParadisAPI.Helpers
{
    public class ImageFileValidator
    {
        public static bool IsValidateExtensions(string fileName)
        {
            var extensionList = new List<string>()
            {
                ".jpeg",".jpg", ".png", ".webp"            
            };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !(extensionList.Contains(extension)))
            {
                return false;
            }
            return true;
        }
    }
}
