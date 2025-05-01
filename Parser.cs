using System;
using System.Collections.Generic;

namespace lab1_compiler.Bar
{
    public class ParsingError
    {
        public string Message { get; set; }
        public int Position { get; set; }
    }

    public class Parser
    {
        private List<LexicalToken> _tokens;
        private int _current;
        private readonly List<ParsingError> _errors = new List<ParsingError>();
        private List<string> _poliz = new List<string>();
        private Stack<string> _operatorStack = new Stack<string>();
        public List<string> GetPOLIZ() => _poliz;
        public List<ParsingError> Parse(List<LexicalToken> tokens)
        {
            _tokens = tokens;
            _current = 0;
            _errors.Clear();
            _poliz.Clear();
            _operatorStack.Clear();

            try
            {
                E();
                while (_operatorStack.Count > 0)
                {
                    _poliz.Add(_operatorStack.Pop());
                }
                if (_current != _tokens.Count)
                    AddError("Ожидается конец выражения", _tokens[_current].Position);
            }
            catch (Exception ex)
            {
                AddError(ex.Message, _current < _tokens.Count ? _tokens[_current].Position : 0);
            }

            return _errors;
        }

        private void E()
        {
            T();
            while (_current < _tokens.Count && (IsAddOp()))
            {
                PushOperator(_tokens[_current].Value);
                _current++;
                T();
            }
        }

        private void T()
        {
            O();
            while (_current < _tokens.Count && (IsMulOp()))
            {
                PushOperator(_tokens[_current].Value);
                _current++;
                O();
            }
        }

        private void O()
        {
            if (_current < _tokens.Count && _tokens[_current].Type == "NUMBER")
            {
                _poliz.Add(_tokens[_current].Value);
                _current++;
            }
            else if (_current < _tokens.Count && _tokens[_current].Value == "(")
            {
                _operatorStack.Push("(");
                _current++;
                E();
                if (_current >= _tokens.Count || _tokens[_current].Value != ")")
                    AddError("Ожидается закрывающая скобка", _current);
                else
                {
                    _current++;
                    while (_operatorStack.Peek() != "(")
                    {
                        _poliz.Add(_operatorStack.Pop());
                    }
                    _operatorStack.Pop();
                }
            }
            else
            {
                throw new Exception("Ожидается число или скобка");
            }
        }

        private void AddError(string message, int position)
        {
            _errors.Add(new ParsingError
            {
                Message = message,
                Position = position
            });
        }

        private void PushOperator(string op)
        {
            while (_operatorStack.Count > 0 &&
                   GetPriority(_operatorStack.Peek()) >= GetPriority(op)) // Возвращаем ">=" для левой ассоциативности
            {
                _poliz.Add(_operatorStack.Pop());
            }
            _operatorStack.Push(op);
        }



        private int GetPriority(string op) => op switch
        {
            "+" => 1,   // плюс — самый низкий
            "-" => 2,   // минус — чуть повыше
            "*" or "/" => 3,   // умножение и деление — самые высокие
            _ => 0
        };

        private bool IsAddOp() => _tokens[_current].Value == "+" || _tokens[_current].Value == "-";
        private bool IsMulOp() => _tokens[_current].Value == "*" || _tokens[_current].Value == "/";
    }
}