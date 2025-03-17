using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        private readonly Dictionary<string, int> _tokenTypes = new Dictionary<string, int>
        {
            { "SingleLineComment", 1 },
            { "MultiLineCommentStart", 2 },
            { "MultiLineCommentEnd", 3 },
            { "StringLiteral", 4 }
        };

        public List<LexicalToken> Tokens { get; } = new List<LexicalToken>();
        public List<string> Errors { get; } = new List<string>();

        public void Analyze(string text)
        {
            Tokens.Clear();
            Errors.Clear();

            bool inMultiLineComment = false;
            int currentLine = 1;
            // Паттерн ищет: однострочный комментарий, начало комментария, конец комментария, строковый литерал или прочую лексему.
            string pattern = @"(//.*)|(/\*)|(\*/)|(""[^""]*"")|(\S+)";

            string[] lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int pos = 0;

                while (pos < line.Length)
                {
                    // Если не получается найти совпадение, просто переходим к следующему символу
                    Match match = Regex.Match(line.Substring(pos), pattern, RegexOptions.Compiled);
                    if (!match.Success)
                    {
                        pos++;
                        continue;
                    }

                    string value = match.Value;
                    int tokenCode = -1;
                    string tokenType = "";

                    // Если мы не внутри многострочного комментария
                    if (!inMultiLineComment)
                    {
                        if (match.Groups[1].Success)
                        {
                            // Однострочный комментарий – сразу добавляем и завершаем анализ строки
                            tokenCode = _tokenTypes["SingleLineComment"];
                            tokenType = "Однострочный комментарий";
                            Tokens.Add(new LexicalToken
                            {
                                Code = tokenCode,
                                Type = tokenType,
                                Value = value,
                                Position = $"Строка {currentLine}, Позиция {pos + 1}"
                            });
                            break; // остальная часть строки считается комментарием
                        }
                        else if (match.Groups[2].Success)
                        {
                            // Начало многострочного комментария
                            tokenCode = _tokenTypes["MultiLineCommentStart"];
                            tokenType = "Начало комментария";
                            Tokens.Add(new LexicalToken
                            {
                                Code = tokenCode,
                                Type = tokenType,
                                Value = value,
                                Position = $"Строка {currentLine}, Позиция {pos + 1}"
                            });
                            inMultiLineComment = true;
                        }
                        else if (match.Groups[4].Success)
                        {
                            // Строковый литерал вне комментария
                            tokenCode = _tokenTypes["StringLiteral"];
                            tokenType = "Строковый литерал";
                            Tokens.Add(new LexicalToken
                            {
                                Code = tokenCode,
                                Type = tokenType,
                                Value = value,
                                Position = $"Строка {currentLine}, Позиция {pos + 1}"
                            });
                        }
                        else if (match.Groups[3].Success)
                        {
                            // Неожиданный маркер конца комментария вне многострочного комментария – ошибка
                            Errors.Add($"Ошибка в строке {currentLine}, позиция {pos + 1}: Неожиданный маркер конца комментария");
                        }
                        else if (match.Groups[5].Success)
                        {
                            // Любая другая лексема вне комментария – считаем ошибкой, так как анализатор предназначен только для комментариев и строковых литералов
                            Errors.Add($"Ошибка в строке {currentLine}, позиция {pos + 1}: Недопустимая лексема '{value}'");
                        }
                    }
                    else
                    {
                        // Находимся внутри многострочного комментария:
                        // Обрабатываем только маркер конца комментария и строковые литералы.
                        if (match.Groups[3].Success)
                        {
                            tokenCode = _tokenTypes["MultiLineCommentEnd"];
                            tokenType = "Конец комментария";
                            Tokens.Add(new LexicalToken
                            {
                                Code = tokenCode,
                                Type = tokenType,
                                Value = value,
                                Position = $"Строка {currentLine}, Позиция {pos + 1}"
                            });
                            inMultiLineComment = false;
                        }
                        else if (match.Groups[4].Success)
                        {
                            tokenCode = _tokenTypes["StringLiteral"];
                            tokenType = "Строковый литерал";
                            Tokens.Add(new LexicalToken
                            {
                                Code = tokenCode,
                                Type = tokenType,
                                Value = value,
                                Position = $"Строка {currentLine}, Позиция {pos + 1}"
                            });
                        }
                        // Остальные совпадения (например, группа 5) просто игнорируем внутри комментария
                    }

                    pos += match.Length;
                }
                currentLine++;
            }
        }
    }
}