using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace lab1_compiler.Bar
{
    public class LexicalToken
    {
        public int Code { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Position { get; set; }
    }

    internal class LexicalAnalyzer
    {
        // Код токенов:
        // 1 - Начало однострочного комментария (// или #)
        // 2 - Начало многострочного комментария /*
        // 3 - Конец многострочного комментария */
        // 4 - Текст комментария
        private readonly Dictionary<string, int> _tokenTypes = new Dictionary<string, int>
        {
            { "SingleLineCommentStart", 1 },
            { "MultiLineCommentStart", 2 },
            { "MultiLineCommentEnd", 3 },
            { "CommentText", 4 }
        };

        public List<LexicalToken> Tokens { get; } = new List<LexicalToken>();
        public List<string> Errors { get; } = new List<string>();
        private bool errorReported = false;

        // Функция нейтрализации ошибок
        public void NeutralizeErrors() => Errors.Clear();

        // Вспомогательный метод для регистрации первой ошибки
        private void ReportError(string message)
        {
            if (!errorReported)
            {
                Errors.Add(message);
                errorReported = true;
            }
        }

        public void Analyze(string text)
        {
            // Удаляем незначащие пробелы и табы перед началом комментариев на каждой строке
            text = Regex.Replace(text, @"^[ \t]+(?=(//|#|/\*))", "", RegexOptions.Multiline);

            Tokens.Clear();
            Errors.Clear();
            errorReported = false;

            int i = 0;
            int line = 1;
            int col = 1;
            int length = text.Length;
            bool inString = false;
            char stringDelimiter = '\0';

            while (i < length)
            {
                char current = text[i];

                if (current == '\n')
                {
                    line++;
                    col = 1;
                    i++;
                    inString = false;
                    continue;
                }

                // Строковые литералы
                if (!inString && (current == '"' || current == '\''))
                {
                    inString = true;
                    stringDelimiter = current;
                    i++;
                    col++;
                    continue;
                }
                if (inString)
                {
                    if (current == stringDelimiter)
                        inString = false;
                    else if (current == '\\' && i + 1 < length)
                    {
                        i += 2;
                        col += 2;
                        continue;
                    }
                    i++;
                    col++;
                    continue;
                }

                // Пропускаем остальные пробелы и табы
                if (char.IsWhiteSpace(current) && current != '\n')
                {
                    i++;
                    col++;
                    continue;
                }

                int startLine = line, startCol = col;

                // Однострочный комментарий // или #
                if (current == '/' && i + 1 < length && text[i + 1] == '/')
                {
                    EmitSingleLine(text, ref i, ref col, startLine, startCol, "//");
                    continue;
                }
                if (current == '#')
                {
                    EmitSingleLine(text, ref i, ref col, startLine, startCol, "#");
                    continue;
                }

                // Многострочный комментарий /* ... */
                if (current == '/' && i + 1 < length && text[i + 1] == '*')
                {
                    EmitMultiLine(text, ref i, ref col, ref line, startLine, startCol);
                    continue;
                }

                // внутри метода Analyze, замените блок «Некорректное начало комментария» на:
                if (current == '/' && i + 1 < length && text[i + 1] != '/' && text[i + 1] != '*')
                {
                    // 1) Зарегистрировать первую ошибку
                    ReportError($"Неправильное начало комментария '/{text[i + 1]}' at line {line}, col {col}");

                    // 2) Вставить второй слэш сразу после текущего:
                    var sb = new StringBuilder(text);
                    sb.Insert(i + 1, "/");
                    text = sb.ToString();
                    length = text.Length;

                    // 3) Скорректировать счётчик столбцов — мы «проехали» ещё один символ
                    col++;

                    // 4) Теперь у нас точно "//", эмитим однострочный комментарий
                    EmitSingleLine(text, ref i, ref col, line, startCol, "//");
                    continue;
                }

                // Некорректное закрытие */ вне контекста
                if (current == '*' && i + 1 < length && text[i + 1] == '/')
                {
                    Tokens.Add(new LexicalToken
                    {
                        Code = _tokenTypes["MultiLineCommentEnd"],
                        Type = "Конец многострочного комментария",
                        Value = "*/",
                        Position = $"Строка {startLine}, Позиция {startCol}"
                    });
                    i += 2;
                    col += 2;
                    continue;
                }

                // Остальное пропускаем
                i++;
                col++;
            }
        }

        private void EmitSingleLine(string text, ref int i, ref int col, int line, int colPos, string marker)
        {
            Tokens.Add(new LexicalToken
            {
                Code = _tokenTypes["SingleLineCommentStart"],
                Type = "Начало однострочного комментария",
                Value = marker,
                Position = $"Строка {line}, Позиция {colPos}"
            });
            i += marker.Length;
            col += marker.Length;

            int contentStart = i;
            while (i < text.Length && text[i] != '\n')
            {
                i++;
                col++;
            }
            string content = text.Substring(contentStart, i - contentStart).Trim();
            Tokens.Add(new LexicalToken
            {
                Code = _tokenTypes["CommentText"],
                Type = "Текст комментария",
                Value = content,
                Position = $"Строка {line}, Позиция {colPos + marker.Length}"
            });
        }

        private void EmitMultiLine(string text, ref int i, ref int col, ref int line, int startLine, int startCol)
        {
            Tokens.Add(new LexicalToken
            {
                Code = _tokenTypes["MultiLineCommentStart"],
                Type = "Начало многострочного комментария",
                Value = "/*",
                Position = $"Строка {startLine}, Позиция {startCol}"
            });
            i += 2;
            col += 2;
            int contentStart = i;
            bool closed = false;

            while (i < text.Length)
            {
                if (text[i] == '\n')
                {
                    line++;
                    col = 1;
                    i++;
                    continue;
                }
                if (text[i] == '*' && i + 1 < text.Length && text[i + 1] == '/')
                {
                    closed = true;
                    break;
                }
                i++;
                col++;
            }

            string content = text.Substring(contentStart, i - contentStart).Trim();
            Tokens.Add(new LexicalToken
            {
                Code = _tokenTypes["CommentText"],
                Type = "Текст комментария",
                Value = content,
                Position = $"Строка {startLine}, Позиция {startCol + 2}"
            });

            if (closed)
            {
                Tokens.Add(new LexicalToken
                {
                    Code = _tokenTypes["MultiLineCommentEnd"],
                    Type = "Конец многострочного комментария",
                    Value = "*/",
                    Position = $"Строка {line}, Позиция {col}"
                });
                i += 2;
                col += 2;
            }
            else
            {
                ReportError($"Unterminated multi-line comment starting at line {startLine}, col {startCol}");
            }
        }
    }

}