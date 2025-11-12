using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            // Проверяем длину
            if (cleanPhone.Length != 12)
                return (false, "Телефон должен содержать 12 символов (включая +7)");

            if (cleanPhone[0] != ('+'))
                return (false, "Вы забыли +");
            if (cleanPhone[1] != ('7'))
                return (false, "Телефон должен начинаться с 7");

            if (cleanPhone[2] != '9')
                return (false, "Третья цифра телефона должна быть 9");

            // Проверяем что остальные символы - цифры
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

            // Ищем символ @ вручную
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

            // Проверяем что есть ровно один @
            if (atCount != 1)
                return (false, "Email должен содержать один символ @");

            // Проверяем что @ не в начале и не в конце
            if (atPosition <= 0 || atPosition >= email.Length - 1)
                return (false, "Символ @ не может быть в начале или конце email");

            // Проверяем что есть хотя бы одна точка после @
            bool hasDotAfterAt = false;
            for (int i = atPosition + 1; i < email.Length; i++)
            {
                if (email[i] == '.')
                {
                    hasDotAfterAt = true;
                    // Проверяем что после точки есть символы
                    if (i >= email.Length - 1)
                        return (false, "После точки должен быть домен");
                    break;
                }
            }

            if (!hasDotAfterAt)
                return (false, "Email должен содержать точку после @");

            // Упрощенная проверка символов (разрешаем точки в домене)
            foreach (char c in email)
            {
                if (!IsValidEmailCharSimple(c))
                    return (false, "Email содержит недопустимые символы");
            }

            return (true, "");
        }

        private static bool IsValidEmailCharSimple(char c)
        {
            // Разрешаем: буквы, цифры, @, точка, дефис, подчеркивание
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

        // Проверка существующего пользователя
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

        // Сохранение пользователя в файл
        public static void SaveUser(string fullName, string age, string phone, string email, string filePath = "users.txt")
        {
            string userData = $"{fullName}|{phone}|{email}|{age}";
            File.AppendAllLines(filePath, new[] { userData });
        }

        // Получение всех пользователей
        public static List<string[]> GetUsers(string filePath = "users.txt")
        {
            var users = new List<string[]>();

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        users.Add(parts);
                    }
                }
            }

            return users;
        }

        // Вспомогательные методы
        private static bool IsValidNameChar(char c)
        {
            // Русские буквы
            if ((c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я') || c == 'ё' || c == 'Ё')
                return true;

            // Английские буквы
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                return true;

            // Разрешенные специальные символы
            if (c == ' ' || c == '-')
                return true;

            return false;
        }
    }
}