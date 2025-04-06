using System;
using System.Collections.Generic;

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

        public List<ParsingError> Parse(string text)
        {
            Errors.Clear();

            int i = 0;
            int line = 1;
            int col = 1;
            int length = text.Length;

            // Стек для хранения позиций открывающих токенов "/*"
            Stack<(int line, int col)> multiLineCommentStack = new();

            while (i < length)
            {
                char current = text[i];
                char next = (i + 1 < length) ? text[i + 1] : '\0';

                if (current == '\n')
                {
                    line++;
                    col = 1;
                    i++;
                    continue;
                }

                // Обработка однострочного комментария: //
                if (current == '/' && next == '/')
                {
                    i += 2;
                    col += 2;
                    while (i < length && text[i] != '\n')
                    {
                        i++;
                        col++;
                    }
                    continue;
                }

                // Начало многострочного комментария: /*
                if (current == '/' && next == '*')
                {
                    // Сохраняем позицию начала "/*"
                    multiLineCommentStack.Push((line, col));
                    i += 2;
                    col += 2;
                    continue;
                }

                // Конец многострочного комментария: */
                if (current == '*' && next == '/')
                {
                    if (multiLineCommentStack.Count == 0)
                    {
                        // Ошибка: закрывающий токен без соответствующего открытия
                        AddError("Незакрытый закрывающий токен многострочного комментария", "/*", line, col);
                    }
                    else
                    {
                        multiLineCommentStack.Pop();
                    }
                    i += 2;
                    col += 2;
                    continue;
                }

                // Обнаружен одиночный '/'
                if (current == '/')
                {
                    AddError("Ожидалось //", "//", line, col);
                    i++;
                    col++;
                    continue;
                }

                i++;
                col++;
            }

            // Для каждого оставшегося в стеке открывающего токена выводим ошибку
            foreach (var (startLine, startCol) in multiLineCommentStack)
            {
                AddError("Незакрытый открывающий токен многострочного комментария", "*/", line, col);
            }

            return Errors;
        }

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
