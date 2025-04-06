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
        // 1 - начало однострочного комментария,
        // 2 - начало многострочного комментария,
        // 3 - конец многострочного комментария,
        // 4 - текст комментария.
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

                // Обновляем line и col для новой строки
                if (current == '\n')
                {
                    line++;
                    col = 1;
                    i++;
                    continue;
                }

                // Обработка однострочного комментария: //
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

                // Обработка многострочного комментария: корректный случай "/*"
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
                    while (i < length)
                    {
                        if (text[i] == '\n')
                        {
                            line++;
                            col = 1;
                            i++;
                            continue;
                        }
                        // Если найдено окончание комментария
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
                    if (endFound)
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
                    continue;
                }

                

                // Если встретился случай корректного закрытия комментария "*/" вне блока (например, когда комментарий не был открыт)
                if (current == '*' && (i + 1) < length && text[i + 1] == '/')
                {
                    int startLine = line, startCol = col;
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

                // Пропускаем остальные символы
                i++;
                col++;
            }
        }
    }
}
