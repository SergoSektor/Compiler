using System;
using System.Collections.Generic;

namespace lab1_compiler.Bar
{
    public class LexicalToken
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Position { get; set; }
    }

    internal class LexicalAnalyzer
    {
        private int _position;
        private string _input;
        private readonly List<string> _errors = new List<string>();

        public List<LexicalToken> Tokens { get; } = new List<LexicalToken>();
        public List<string> Errors => _errors;

        public void Analyze(string input)
        {
            _input = input;
            _position = 0;
            Tokens.Clear();
            _errors.Clear();

            while (_position < _input.Length)
            {
                char current = _input[_position];

                if (char.IsWhiteSpace(current))
                {
                    _position++;
                    continue;
                }

                if (char.IsDigit(current))
                {
                    ReadNumber();
                }
                else if (current == '+' || current == '-' || current == '*' || current == '/')
                {
                    Tokens.Add(new LexicalToken { Type = "OPERATOR", Value = current.ToString(), Position = _position });
                    _position++;
                }
                else if (current == '(' || current == ')')
                {
                    Tokens.Add(new LexicalToken { Type = "BRACKET", Value = current.ToString(), Position = _position });
                    _position++;
                }

                else
                {
                    _errors.Add($"Недопустимый символ '{current}' в позиции {_position}");
                    _position++;
                }
            }
        }

        private void ReadNumber()
        {
            int start = _position;
            while (_position < _input.Length && char.IsDigit(_input[_position]))
            {
                _position++;
            }
            string num = _input.Substring(start, _position - start);
            Tokens.Add(new LexicalToken { Type = "NUMBER", Value = num, Position = start });
        }
    }
}