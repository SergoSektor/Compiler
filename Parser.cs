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

        /// <summary>
        /// Анализирует текст с комментариями в стиле C/C++.
        /// Однострочные комментарии начинаются с "//" – они просто пропускаются.
        /// Многострочные комментарии начинаются с "/*" и заканчиваются на "*/".
        /// Регистрируются ошибки:
        ///  - Если однострочный комментарий начинается с одиночного '/'.
        ///  - Если встречается закрытие многострочного комментария "*/" вне комментария,
        ///    регистрируется ошибка с сообщением, что ожидалось начало комментария "/*"
        ///    и позиция определяется как позиция предполагаемого открывающего токена (первый непробельный символ строки).
        ///  - Если многострочный комментарий не закрыт (ожидалось "*/" в конце текста).
        /// </summary>
        public List<ParsingError> Parse(string text)
        {
            Errors.Clear();
            _errorNumber = 1;

            int i = 0;
            int line = 1;
            int col = 1;
            int length = text.Length;
            bool insideMultiLine = false;

            while (i < length)
            {
                char current = text[i];

                // Обработка перевода строки
                if (current == '\n')
                {
                    line++;
                    col = 1;
                    i++;
                    continue;
                }

                if (!insideMultiLine)
                {
                    if (current == '/')
                    {
                        if (i + 1 < length)
                        {
                            char next = text[i + 1];
                            if (next == '/')
                            {
                                // Корректный однострочный комментарий "//"
                                i += 2;
                                col += 2;
                                while (i < length && text[i] != '\n')
                                {
                                    i++;
                                    col++;
                                }
                                continue;
                            }
                            else if (next == '*')
                            {
                                // Открытие многострочного комментария "/*"
                                insideMultiLine = true;
                                i += 2;
                                col += 2;
                                continue;
                            }
                            else
                            {
                                // Ошибка: одиночный слеш, ожидается "//"
                                AddError("Ожидалось начало однострочного комментария \"//\"", "/", line, col);
                                i++;
                                col++;
                                continue;
                            }
                        }
                        else
                        {
                            AddError("Одиночный '/' не является началом комментария", "/", line, col);
                            i++;
                            col++;
                            continue;
                        }
                    }

                    // Если вне многострочного комментария встречается "*/"
                    if (current == '*' && i + 1 < length && text[i + 1] == '/')
                    {
                        // Определяем позицию, где ожидалось открытие комментария "/*".
                        // Для этого ищем первый непробельный символ текущей строки.
                        int expectedCol = GetFirstNonWhitespaceColumn(text, i, col);
                        AddError("Закрытие многострочного комментария без соответствующего открытия", "/*", line, expectedCol);
                        i += 2;
                        col += 2;
                        continue;
                    }
                }
                else
                {
                    // Находимся внутри многострочного комментария.
                    if (current == '*' && i + 1 < length && text[i + 1] == '/')
                    {
                        insideMultiLine = false;
                        i += 2;
                        col += 2;
                        continue;
                    }
                }

                i++;
                col++;
            }

            // Если текст закончился, а многострочный комментарий не закрыт
            if (insideMultiLine)
            {
                // Ошибка в точке конца текста – здесь ожидался закрывающий токен "*/"
                AddError("Незакрытый многострочный комментарий", "*/", line, col);
            }

            return Errors;
        }

        /// <summary>
        /// Ищет в текущей строке первый непробельный символ.
        /// Если не найден, возвращает 1.
        /// </summary>
        private int GetFirstNonWhitespaceColumn(string text, int currentIndex, int currentCol)
        {
            // Ищем начало строки
            int index = currentIndex;
            while (index > 0 && text[index - 1] != '\n')
            {
                index--;
            }
            // Теперь index – начало текущей строки; вычисляем колонку первого непробельного символа
            int col = 1;
            while (index < text.Length && text[index] != '\n')
            {
                if (!Char.IsWhiteSpace(text[index]))
                {
                    return col;
                }
                index++;
                col++;
            }
            return 1;
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
