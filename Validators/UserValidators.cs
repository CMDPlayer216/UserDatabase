namespace userdb.Validators;

public static class UserValidators
{
    private static bool StringValidator(string text, string allowedChars)
    {
        bool isValidType = !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        bool containsValidChars = true;
        
        foreach (char current in text)
        {
            if (!allowedChars.Contains(current))
            {
                containsValidChars = false;
                break;
            }
        }

        return isValidType && containsValidChars;
    }
    private static bool StringValidator(string text, string[] allowedStrings)
    {
        bool isValidType = !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        bool isAllowed = allowedStrings.Contains(text);

        return isValidType && isAllowed;
    }

    private static bool? IntValidator(int i, int maxValue, int minValue, int neutralValue)
    {
        if (i == neutralValue) return null;
        return (maxValue >= i && i >= minValue);
    }
    public static bool ValidateName(string name)
    {
        string validChars = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZáéíóúÁÉÍÓÚüÜçÇ1234567890'-_ ";
        return StringValidator(name, validChars);
    }

    public static bool ValidateId(string ID)
    {
        string validChars = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ1234567890-_";
        return StringValidator(ID, validChars);
    }

    public static bool ValidateFandom(string fandom)
    {
        string validChars = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZáéíóúÁÉÍÓÚüÜçÇ1234567890'-_ ";
        return StringValidator(fandom, validChars);
    }

    public static bool? ValidateAge(int age)
    {
        int minValue = 0;
        int maxValue = Int32.MaxValue;
        int neutralValue = -1;

        return IntValidator(age, maxValue, minValue, neutralValue);
    }

    public static bool ValidateRole(string role)
    {
        string validChars = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZáéíóúÁÉÍÓÚüÜçÇ1234567890'-_ ";
        return StringValidator(role, validChars);
    }
    public static bool ValidatePronoun(string pronoun)
    {
        string validChars = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZáéíóúÁÉÍÓÚüÜçÇ1234567890'-_";
        return StringValidator(pronoun, validChars);
    }
    public static bool? ValidateStreak(int streak)
    {
        int minValue = 0;
        int maxValue = Int32.MaxValue;
        int neutralValue = -1;

        return IntValidator(streak, maxValue, minValue, neutralValue);
    }
    public static bool ValidateStatus(string status)
    {
        string[] validStatus = {"Active"   , "Activo"  , "Inactive",
                                "Inactivo" , "Unknow"  , "Desconocido",
                                "Banned"   , "Baneado" , "Kicked",
                                "Expulsado", "Silenced", "Silenciado"};
        return StringValidator(status, validStatus);
    }
}