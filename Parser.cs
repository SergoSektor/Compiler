using System;
using System.Collections.Generic;
using System.Text;

namespace lab1_compiler.Bar
{
    public class ParsingError
    {
        public int NumberOfError { get; set; }
        public string Message { get; set; }
        public string ExpectedToken { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class RawTextParser
    {
        private List<ParsingError> Errors = new List<ParsingError>();
        private int _errorNumber = 1;
        private string _correctedText = string.Empty;

        /// <summary>
        /// Сбрасывает состояние парсера и подготавливает его для нового анализа
        /// </summary>
        public void AutoCorrectErrors()
        {
            Errors.Clear();
            _errorNumber = 1;
            _correctedText = string.Empty;
        }

        /// <summary>
        /// Возвращает текст после последней коррекции
        /// </summary>
        public string GetCorrectedText() => _correctedText;

        /// <summary>
        /// Анализирует текст построчно и исправляет ошибки:
        /// - Если строка начинается с "/" (но не с "//" или "/*"), то заменяет её на "//" или "/*" в зависимости от наличия признаков многострочного комментария.
        /// - Если строка начинается с "*" – добавляет отсутствующий "/" в начале и, если нужно, закрывающий "*/" в конце.
        /// - Если строка начинается с "/*", но не заканчивается на "*/", добавляет закрывающий токен.
        /// - Если строка начинается с "<?php", коррекция не производится (PHP-выражения).
        /// </summary>
        public List<ParsingError> ParseWithRecovery(string text)
        {
            AutoCorrectErrors();
            var lines = text.Split('\n');
            StringBuilder sb = new StringBuilder();
            int lineNumber = 0;

            foreach (var line in lines)
            {
                lineNumber++;
                string correctedLine = line;
                // Определяем позицию первого непробельного символа для отчёта об ошибке
                int firstNonWhitespace = line.Length - line.TrimStart().Length;
                int col = firstNonWhitespace + 1;
                string trimmed = line.TrimStart();

                // Если строка начинается с PHP-тега, пропускаем коррекцию
                if (trimmed.StartsWith("<?php"))
                {
                    sb.AppendLine(line);
                    continue;
                }

                // Если строка уже начинается с корректного однострочного комментария, оставляем её
                if (trimmed.StartsWith("//"))
                {
                    sb.AppendLine(line);
                    continue;
                }
                // Если строка начинается с корректного многострочного комментария
                else if (trimmed.StartsWith("/*"))
                {
                    if (!trimmed.EndsWith("*/"))
                    {
                        // Ошибка: незакрытый многострочный комментарий
                        AddError("Незакрытый многострочный комментарий", "*/", lineNumber, line.Length);
                        correctedLine = line + "*/";
                    }
                }
                // Если строка начинается с "/" но не с "//" или "/*"
                else if (trimmed.StartsWith("/"))
                {
                    // Если строка, вероятно, задумывалась как многострочный комментарий (например, заканчивается на "*" или содержит "*" ближе к концу)
                    if (trimmed.EndsWith("*") || trimmed.Contains(" *"))
                    {
                        AddError("Некорректный символ '/'", "// или /*", lineNumber, col);
                        // Заменяем первый символ на "/*"
                        correctedLine = line.Substring(0, firstNonWhitespace) + "/*" + line.Substring(firstNonWhitespace + 1);
                        // Если в строке отсутствует закрывающий токен, добавляем его
                        if (!correctedLine.TrimEnd().EndsWith("*/"))
                        {
                            correctedLine = TrimEndEndAsterisk(correctedLine) + "*/";
                        }
                    }
                    else
                    {
                        // Если строка задумывалась как однострочный комментарий
                        AddError("Некорректный символ '/'", "// или /*", lineNumber, col);
                        correctedLine = line.Substring(0, firstNonWhitespace) + "//" + line.Substring(firstNonWhitespace + 1);
                    }
                }
                // Если строка начинается с "*", но без открывающего комментария
                else if (trimmed.StartsWith("*"))
                {
                    AddError("Закрытие комментария без открытия", "/*", lineNumber, col);
                    correctedLine = line.Substring(0, firstNonWhitespace) + "/*" + line.Substring(firstNonWhitespace + 1);
                    if (!correctedLine.TrimEnd().EndsWith("*/"))
                    {
                        correctedLine = TrimEndEndAsterisk(correctedLine) + "*/";
                    }
                }
                // Иначе оставляем строку без изменений

                sb.AppendLine(correctedLine);
            }

            _correctedText = sb.ToString();
            return Errors;
        }

        private string TrimEndEndAsterisk(string line)
        {
            string trimmed = line.TrimEnd();
            if (trimmed.EndsWith("*") && !trimmed.EndsWith("*/"))
            {
                return trimmed.Substring(0, trimmed.Length - 1);
            }
            return trimmed;
        }


        /// <summary>
        /// Добавляет ошибку в коллекцию
        /// </summary>
        private void AddError(string message, string expected, int line, int col)
        {
            Errors.Add(new ParsingError
            {
                NumberOfError = _errorNumber++,
                Message = message,
                ExpectedToken = expected,
                Line = line,
                Column = col
            });
        }
    }
}
