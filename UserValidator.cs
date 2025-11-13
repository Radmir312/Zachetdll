using System;
using System.IO;

namespace Zachetdll
{
    public static class UserValidator
    {
        public static (bool isValid, string error) ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (false, "ФИО не может быть пустым");

            foreach (char c in fullName)
            {
                if (!IsValidNameChar(c))
                    return (false, "ФИО может содержать только буквы, пробелы и дефисы");
            }

            return (true, "");
        }

        public static (bool isValid, string error) ValidateAge(string age)
        {
            if (string.IsNullOrWhiteSpace(age))
                return (false, "Возраст не может быть пустым");

            foreach (char c in age)
            {
                if (c < '0' || c > '9')
                    return (false, "Возраст должен содержать только цифры");
            }

            if (int.TryParse(age, out int ageNumber))
            {
                if (ageNumber < 1 || ageNumber > 150)
                    return (false, "Возраст должен быть от 1 до 150 лет");
            }
            else
            {
                return (false, "Некорректный формат возраста");
            }

            return (true, "");
        }

        public static (bool isValid, string error) ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return (false, "Телефон не может быть пустым");

            string cleanPhone = phone.Replace(" ", "");

            if (cleanPhone.Length != 12)
                return (false, "Телефон должен содержать 12 символов (включая +7)");

            if (cleanPhone[0] != '+')
                return (false, "Вы забыли +");
            if (cleanPhone[1] != '7')
                return (false, "Телефон должен начинаться с 7");

            if (cleanPhone[2] != '9')
                return (false, "Третья цифра телефона должна быть 9");

            for (int i = 3; i < cleanPhone.Length; i++)
            {
                if (cleanPhone[i] < '0' || cleanPhone[i] > '9')
                    return (false, "Телефон должен содержать только цифры после +7");
            }

            return (true, "");
        }

        public static (bool isValid, string error) ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email не может быть пустым");

            int atPosition = -1;
            int atCount = 0;

            for (int i = 0; i < email.Length; i++)
            {
                if (email[i] == '@')
                {
                    atPosition = i;
                    atCount++;
                }
            }

            if (atCount != 1)
                return (false, "Email должен содержать один символ @");

            if (atPosition <= 0 || atPosition >= email.Length - 1)
                return (false, "Символ @ не может быть в начале или конце email");

            bool hasDotAfterAt = false;
            for (int i = atPosition + 1; i < email.Length; i++)
            {
                if (email[i] == '.')
                {
                    hasDotAfterAt = true;
                    if (i >= email.Length - 1)
                        return (false, "После точки должен быть домен");
                    break;
                }
            }

            if (!hasDotAfterAt)
                return (false, "Email должен содержать точку после @");

            foreach (char c in email)
            {
                if (!IsValidEmailChar(c))
                    return (false, "Email содержит недопустимые символы");
            }

            return (true, "");
        }

        public static bool UserExists(string fullName, string phone, string email, string filePath = "users.txt")
        {
            if (!File.Exists(filePath))
                return false;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 3)
                {
                    if (parts[0].Equals(fullName, StringComparison.OrdinalIgnoreCase) ||
                        parts[1].Equals(phone, StringComparison.OrdinalIgnoreCase) ||
                        parts[2].Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void SaveUser(string fullName, string age, string phone, string email, string filePath = "users.txt")
        {
            string userData = $"{fullName}|{phone}|{email}|{age}";
            File.AppendAllLines(filePath, new[] { userData });
        }

        private static bool IsValidEmailChar(char c)
        {
            if ((c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                (c >= '0' && c <= '9') ||
                c == '@' ||
                c == '.' ||
                c == '-' ||
                c == '_')
                return true;

            return false;
        }

        private static bool IsValidNameChar(char c)
        {
            if ((c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я') || c == 'ё' || c == 'Ё')
                return true;

            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                return true;

            if (c == ' ' || c == '-')
                return true;

            return false;
        }
    }
}