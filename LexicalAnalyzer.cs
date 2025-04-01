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
        // Код токенов:
        // 1 - начало однострочного комментария
        // 2 - начало многострочного комментария
        // 3 - конец многострочного комментария
        // 4 - текст комментария
        // 5 - ошибка (символ не является комментарием)
        private readonly Dictionary<string, int> _tokenTypes = new Dictionary<string, int>
        {
            { "SingleLineCommentStart", 1 },
            { "MultiLineCommentStart", 2 },
            { "MultiLineCommentEnd", 3 },
            { "CommentText", 4 },
            { "Error", 5 }
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

                // Обработка новой строки
                if (current == '\n')
                {
                    line++;
                    col = 1;
                    i++;
                    continue;
                }

                // Проверка на однострочный комментарий: начинается с "//"
                if (current == '/' && (i + 1) < length && text[i + 1] == '/')
                {
                    int startLine = line, startCol = col;
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

                // Проверка на многострочный комментарий: начинается с "/*"
                if (current == '/' && (i + 1) < length && text[i + 1] == '*')
                {
                    int startLine = line, startCol = col;
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
                    // Считываем содержимое многострочного комментария
                    while (i < length)
                    {
                        if (text[i] == '\n')
                        {
                            line++;
                            col = 1;
                            i++;
                            continue;
                        }
                        // Если найден конец комментария "*/"
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
                    // Если конец найден, фиксируем его
                    if (endFound)
                    {
                        Tokens.Add(new LexicalToken
                        {
                            Code = _tokenTypes["MultiLineCommentEnd"],
                            Type = "Конец многострочного комментария",
                            Value = "*/",
                            Position = $"Строка {line}, Позиция {col}"
                        });
                        i += 2; // Пропускаем символы "*/"
                        col += 2;
                    }
                    else
                    {
                        Errors.Add($"Не найден конец многострочного комментария, начатого на строке {startLine}, позиция {startCol}");
                    }
                    continue;
                }

                // Если текущий символ не является началом комментария, считаем его ошибкой
                Tokens.Add(new LexicalToken
                {
                    Code = _tokenTypes["Error"],
                    Type = "Ошибка: не являющийся комментарием символ",
                    Value = current.ToString(),
                    Position = $"Строка {line}, Позиция {col}"
                });
                Errors.Add($"Ошибка: символ '{current}' на строке {line}, позиция {col} не является частью комментария.");
                i++;
                col++;
            }
        }
    }
}
