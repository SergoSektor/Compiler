using System;
using System.Collections.Generic;

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
        // Код токенов: 1 - начало однострочного комментария, 2 - начало многострочного комментария,
        // 3 - конец многострочного комментария, 4 - текст комментария.
        private readonly Dictionary<string, int> _tokenTypes = new Dictionary<string, int>
        {
            { "SingleLineCommentStart", 1 },
            { "MultiLineCommentStart", 2 },
            { "MultiLineCommentEnd", 3 },
            { "CommentText", 4 }
        };

        public List<LexicalToken> Tokens { get; } = new List<LexicalToken>();
        public List<string> Errors { get; } = new List<string>();

        public void Analyze(string text)
        {
            Tokens.Clear();
            Errors.Clear();

            int i = 0;
            int line = 1;
            int col = 1;
            int length = text.Length;

            while (i < length)
            {
                char current = text[i];

                // Если встретили символ новой строки, обновляем line и col
                if (current == '\n')
                {
                    line++;
                    col = 1;
                    i++;
                    continue;
                }

                // Проверка на однострочный комментарий
                if (current == '/' && (i + 1) < length && text[i + 1] == '/')
                {
                    int startLine = line, startCol = col;
                    // Токен для начала однострочного комментария
                    Tokens.Add(new LexicalToken
                    {
                        Code = _tokenTypes["SingleLineCommentStart"],
                        Type = "Начало однострочного комментария",
                        Value = "//",
                        Position = $"Строка {startLine}, Позиция {startCol}"
                    });
                    i += 2;
                    col += 2;
                    int commentStart = i;
                    // Считываем до конца строки или до конца файла
                    while (i < length && text[i] != '\n')
                    {
                        i++;
                        col++;
                    }
                    string commentText = text.Substring(commentStart, i - commentStart).Trim();
                    Tokens.Add(new LexicalToken
                    {
                        Code = _tokenTypes["CommentText"],
                        Type = "Текст комментария",
                        Value = commentText,
                        Position = $"Строка {startLine}, Позиция {startCol + 2}"
                    });
                    continue;
                }

                // Проверка на многострочный комментарий
                if (current == '/' && (i + 1) < length && text[i + 1] == '*')
                {
                    int startLine = line, startCol = col;
                    // Токен для начала многострочного комментария
                    Tokens.Add(new LexicalToken
                    {
                        Code = _tokenTypes["MultiLineCommentStart"],
                        Type = "Начало многострочного комментария",
                        Value = "/*",
                        Position = $"Строка {startLine}, Позиция {startCol}"
                    });
                    i += 2;
                    col += 2;
                    int commentTextStart = i;
                    bool endFound = false;
                    // Считываем текст до появления "*/"
                    while (i < length)
                    {
                        // Обработка новой строки
                        if (text[i] == '\n')
                        {
                            line++;
                            col = 1;
                            i++;
                            continue;
                        }
                        // Если найден конец комментария
                        if (text[i] == '*' && (i + 1) < length && text[i + 1] == '/')
                        {
                            endFound = true;
                            break;
                        }
                        i++;
                        col++;
                    }
                    string multiCommentText = text.Substring(commentTextStart, i - commentTextStart).Trim();
                    Tokens.Add(new LexicalToken
                    {
                        Code = _tokenTypes["CommentText"],
                        Type = "Текст комментария",
                        Value = multiCommentText,
                        Position = $"Строка {startLine}, Позиция {startCol + 2}"
                    });
                    // Если конец найден, добавляем токен конца комментария
                    if (endFound)
                    {
                        Tokens.Add(new LexicalToken
                        {
                            Code = _tokenTypes["MultiLineCommentEnd"],
                            Type = "Конец многострочного комментария",
                            Value = "*/",
                            Position = $"Строка {line}, Позиция {col}"
                        });
                        i += 2; // пропускаем "*/"
                        col += 2;
                    }
                    
                    continue;
                }

                // Если текущий символ не является началом комментария, просто пропускаем его
                i++;
                col++;
            }
        }
    }
}