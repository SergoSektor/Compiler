using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using lab1_compiler.Bar;



namespace lab1_compiler
{

    public partial class Compiler : Form
    {

        //ЛЕКСЕР
        public enum TokenType
        {
            IF, ELSE, TRUE, FALSE, AND, OR, NOT,
            ID, ASSIGN, SEMICOLON,
            EOF, ERROR
        }

        public class Token
        {
            public TokenType Type { get; set; }
            public string Value { get; set; }
            public int Position { get; set; }

            public Token(TokenType type, string value, int position)
            {
                Type = type;
                Value = value;
                Position = position;
            }

            public override string ToString() => $"{Type} ({Value})";
        }

        public class Lexer
        {
            private string _text;
            private int _pos;
            private readonly List<string> _assignOps = new() { "==", "<", "<=", ">", ">=", "!=" };

            public Lexer(string text)
            {
                _text = text;
                _pos = 0;
            }

            private char Current => _pos < _text.Length ? _text[_pos] : '\0';

            private void Advance() => _pos++;

            private void SkipWhitespace()
            {
                while (char.IsWhiteSpace(Current)) Advance();
            }

            public List<Token> Tokenize()
            {
                List<Token> tokens = new();
                while (_pos < _text.Length)
                {
                    SkipWhitespace();
                    int start = _pos;

                    if (char.IsLetter(Current))
                    {
                        string ident = ReadIdentifier();

                        // ключевые слова
                        string upperIdent = ident.ToUpper();
                        switch (upperIdent)
                        {
                            case "IF": tokens.Add(new Token(TokenType.IF, ident, start)); break;
                            case "ELSE": tokens.Add(new Token(TokenType.ELSE, ident, start)); break;
                            case "TRUE": tokens.Add(new Token(TokenType.TRUE, ident, start)); break;
                            case "FALSE": tokens.Add(new Token(TokenType.FALSE, ident, start)); break;
                            case "AND": tokens.Add(new Token(TokenType.AND, ident, start)); break;
                            case "OR": tokens.Add(new Token(TokenType.OR, ident, start)); break;
                            case "NOT": tokens.Add(new Token(TokenType.NOT, ident, start)); break;
                            default: tokens.Add(new Token(TokenType.ID, ident, start)); break;
                        }
                        continue;
                    }

                    else if (Current == ';')
                    {
                        tokens.Add(new Token(TokenType.SEMICOLON, ";", _pos));
                        Advance();
                    }
                    else if (_assignOps.Any(op => _text.Substring(_pos).StartsWith(op)))
                    {
                        var op = _assignOps.First(op => _text.Substring(_pos).StartsWith(op));
                        tokens.Add(new Token(TokenType.ASSIGN, op, _pos));
                        _pos += op.Length;
                    }

                    else
                    {
                        tokens.Add(new Token(TokenType.ERROR, Current.ToString(), _pos));
                        Advance();
                    }
                }

                tokens.Add(new Token(TokenType.EOF, "", _pos));
                return tokens;
            }

            private string ReadIdentifier()
            {
                int start = _pos;
                while (char.IsLetterOrDigit(Current)) Advance(); // останавливаемся строго на первом не-алфавитном символе
                return _text.Substring(start, _pos - start);
            }

        }

        //ПАРСЕР
        public class RecursiveDescentParser
        {
            private List<Token> _tokens;
            private int _current;
            private int _stepCounter = 1;

            public List<string> Log { get; } = new();
            public List<string> Errors { get; } = new();

            public RecursiveDescentParser(List<Token> tokens)
            {
                _tokens = tokens;
                _current = 0;
            }

            private Token Peek => _current < _tokens.Count ? _tokens[_current] : new Token(TokenType.EOF, "", _current);
            private Token Advance() => _current < _tokens.Count ? _tokens[_current++] : Peek;
            private bool Match(TokenType type) => Peek.Type == type;

            private void LogStep(string method, string description)
            {
                string line = $"{_stepCounter.ToString().PadRight(4)}| {method.PadRight(12)}| {description}";
                Log.Add(line);
                _stepCounter++;
            }



            private void Error(string message)
            {
                Errors.Add($"ОШИБКА [{Peek.Position}]: {message}, токен: {Peek.Value}");
            }

            public void ParseStmt()
            {
                LogStep("ParseSTMT", "Вход в stmt");

                if (Match(TokenType.IF))
                {
                    LogStep("ParseIF", "Ключевое слово IF");
                    Advance();

                    ParseExp();

                    ParseStmt();

                    if (Match(TokenType.ELSE))
                    {
                        LogStep("ParseELSE", "Ключевое слово ELSE");
                        Advance();
                        ParseStmt();
                    }
                }
                else if (Match(TokenType.ID))
                {
                    LogStep("ParseID", Peek.Value);
                    Advance();

                    if (Match(TokenType.ASSIGN))
                    {
                        LogStep("ParseASSIGN", Peek.Value);
                        Advance();

                        ParseExp();

                        if (Match(TokenType.SEMICOLON))
                        {
                            LogStep("SEMICOLON", ";");
                            Advance();
                        }
                        else
                        {
                            Error("Ожидалась точка с запятой ';'");
                        }
                    }
                    else
                    {
                        Error("Ожидался оператор сравнения/присваивания");
                    }
                }
                else
                {
                    Error("Ожидался IF или идентификатор");
                    Advance();
                }
            }


            public void ParseExp()
            {
                LogStep("ParseEXP", "Вход в выражение");

                if (Match(TokenType.TRUE) || Match(TokenType.FALSE))
                {
                    LogStep("ParseCONST", Peek.Value);
                    Advance();
                }
                else if (Match(TokenType.ID))
                {
                    LogStep("ParseID", Peek.Value);
                    Advance();
                }
                else
                {
                    Error("Ожидался идентификатор или константа");
                    Advance();
                    return;
                }

                if (Match(TokenType.ASSIGN))
                {
                    LogStep("ParseASSIGN", Peek.Value);
                    Advance();

                    if (Match(TokenType.TRUE) || Match(TokenType.FALSE))
                    {
                        LogStep("ParseCONST", Peek.Value);
                        Advance();
                    }
                    else if (Match(TokenType.ID))
                    {
                        LogStep("ParseID", Peek.Value);
                        Advance();
                    }
                    else
                    {
                        Error("Ожидалась константа или идентификатор после оператора сравнения");
                        Advance();
                    }
                }

                while (Match(TokenType.OR) || Match(TokenType.AND))
                {
                    LogStep("ParseLOGICAL", Peek.Value);
                    Advance();
                    ParseExp();
                }
            }

        }



        private readonly List<float> _defaultFontSizes = new List<float> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24 };

        /// Берём функции для элементов меню
        private readonly FileManager _fileHandler;
        private readonly CorManager _corManager;
        private readonly RefManager _refManager;

        /// Ссылки на справочное руководство
        private const string _aboutPath = @"Resources\About.html";
        private const string _helpPath = @"Resources\Help.html";

        public Compiler()
        {
            InitializeComponent();
            InitializeFontSizeComboBox();
            _fileHandler = new FileManager(this);
            _corManager = new CorManager(richTextBox1);
            _refManager = new RefManager(_helpPath, _aboutPath);

            // Установка минимального размера (ширина, высота)
            this.MinimumSize = new Size(450, 300);

            // Изменили данные в окне ввода
            richTextBox1.DragEnter += RichTextBox_DragEnter;
            richTextBox1.DragDrop += RichTextBox_DragDrop;

            richTextBox1.TextChanged += RichTextBox_TextChanged;
            richTextBox1.VScroll += RichTextBox_VScroll;

            toolStripStatusLabel1.Text = "Compiler успешно запущена";

        }

        private void SetDefaultStyle()
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionBackColor = Color.White;
            richTextBox1.DeselectAll();
        }

        private void HighlightMatches(string pattern, Color color, FontStyle style)
        {
            foreach (Match match in Regex.Matches(richTextBox1.Text, pattern))
            {
                richTextBox1.Select(match.Index, match.Length);
                richTextBox1.SelectionColor = color;
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, style);
            }
        }


        /// <summary>
        /// НУМЕРАЦИИИЯ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RichTextBox_VScroll(object sender, EventArgs e)
        {
            // Синхронизация прокрутки между richTextBox1 и richTextBoxLineNumbers
            int verticalScrollPos = GetFirstVisibleLineNumber() * richTextBoxLineNumbers.Font.Height;
            richTextBoxLineNumbers.SelectionStart = richTextBoxLineNumbers.GetCharIndexFromPosition(new Point(0, verticalScrollPos));
            richTextBoxLineNumbers.ScrollToCaret();
        }

        private int GetFirstVisibleLineNumber()
        {
            // Определяем первую видимую строку в richTextBox1
            int firstVisibleCharIndex = richTextBox1.GetCharIndexFromPosition(new Point(0, 0));
            return richTextBox1.GetLineFromCharIndex(firstVisibleCharIndex);
        }

        private void UpdateLineNumbers()
        {
            int lineCount = richTextBox1.Lines.Length;
            string lineNumbersText = "";

            for (int i = 0; i < lineCount; i++)
            {
                lineNumbersText += (i + 1).ToString() + Environment.NewLine;
            }

            richTextBoxLineNumbers.Text = lineNumbersText;

            int firstVisibleLine = GetFirstVisibleLineNumber();
            int verticalScrollPos = firstVisibleLine * richTextBoxLineNumbers.Font.Height;

            richTextBoxLineNumbers.Select(0, 0);
            richTextBoxLineNumbers.ScrollToCaret();
            richTextBoxLineNumbers.SelectionStart = richTextBoxLineNumbers.GetCharIndexFromPosition(new Point(0, verticalScrollPos));
            richTextBoxLineNumbers.ScrollToCaret();
        }
        /// <summary>
        /// РЕАЛИЗАЦИЯ ДРАГЭНДРОПА ПОФИКСИТЬ КАРТИНКУ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_DragEnter(object sender, DragEventArgs e)
        {
            // Проверяем, что перетаскивается файл
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Разрешаем копирование
            }
            else
            {
                e.Effect = DragDropEffects.None; // Отклоняем другие типы данных
            }
        }

        private void RichTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string filePath = files[0];
                    if (IsTextFile(filePath))
                    {
                        try
                        {
                            // Явно очищаем содержимое перед загрузкой нового файла
                            richTextBox1.Clear();

                            // Загружаем содержимое файла и обновляем интерфейс
                            _fileHandler.DragFile(filePath); // Передаем путь в метод
                            UpdateLineNumbers();
                            UpdateWindowTitle();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Только текстовые файлы (.txt, .cs, .cpp, .java)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private bool IsTextFile(string filePath)
        {
            string[] allowedExtensions = { ".txt", ".cs", ".cpp", ".java", ".php" };
            string extension = Path.GetExtension(filePath).ToLower();
            return allowedExtensions.Contains(extension);
        }

        /// <summary>
        ///  РИЧТЕКСТБОКС
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_TextChanged(object? sender, EventArgs e)
        {
            // Отключаем Undo/Redo для стилей
            _corManager.PauseUndoTracking();
            //ApplySyntaxHighlighting();
            _corManager.ResumeUndoTracking();

            // Обновление файла и интерфейса
            _fileHandler.UpdateFileContent(richTextBox1.Text);
            UpdateLineNumbers();
            UpdateWindowTitle();
        }

        public string GetCurrentContent()
        {
            return richTextBox1.Text;
        }

        public void UpdateRichTextBox(string content)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => richTextBox1.Text = content));
            }
            else
            {
                richTextBox1.Text = content;
            }
        }

        public void UpdateWindowTitle()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateWindowTitle));
                return;
            }

            Text = GetWindowTitle();
        }

        private string GetWindowTitle()
        {
            var filePath = _fileHandler.CurrentFilePath;
            var fileName = string.IsNullOrEmpty(filePath)
                ? "Новый файл.txt"
                : Path.GetFileName(filePath);

            var asterisk = _fileHandler.IsFileModified ? "*" : "";
            var pathInfo = string.IsNullOrEmpty(filePath)
                ? ""
                : $" ({filePath})";

            return $"Компилятор — {fileName}{asterisk}{pathInfo}";
        }

        /// <summary>
        /// РАЗМЕР ШРИФТА
        /// </summary>

        private void InitializeFontSizeComboBox()
        {
            toolStripFontSizeComboBox.ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            toolStripFontSizeComboBox.ComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            // Заполняем только стандартными размерами
            toolStripFontSizeComboBox.ComboBox.Items.AddRange(_defaultFontSizes.Cast<object>().ToArray());

            // Устанавливаем текущий размер шрифта
            toolStripFontSizeComboBox.ComboBox.Text = richTextBox1.Font.Size.ToString();

            // Подписка на события
            toolStripFontSizeComboBox.ComboBox.KeyDown += FontSizeComboBox_KeyDown;
            toolStripFontSizeComboBox.ComboBox.TextChanged += (s, e) => ApplyFontSizeFromComboBox();
        }

        private void FontSizeComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFontSizeFromComboBox();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ApplyFontSizeFromComboBox()
        {
            if (float.TryParse(toolStripFontSizeComboBox.ComboBox.Text, out float newSize))
            {
                // Ограничиваем диапазон без добавления в список
                newSize = Math.Clamp(newSize, 1, 99);

                // Обновляем шрифт
                UpdateFontSize(richTextBox1, newSize);
                UpdateFontSize(richTextBoxLineNumbers, newSize);

                // Обновляем текст без добавления в Items
                toolStripFontSizeComboBox.ComboBox.Text = newSize.ToString();
            }
        }

        private void UpdateFontSize(RichTextBox rtb, float size)
        {
            if (rtb.Font.Size != size)
            {
                rtb.Font = new Font(rtb.Font.FontFamily, size, rtb.Font.Style);
            }
        }

        private void SetComboBoxSelectedSize(float size)
        {
            // Просто устанавливаем текст, не добавляем новые элементы
            toolStripFontSizeComboBox.ComboBox.Text = size.ToString();
        }

        /// <summary>
        /// Bar/FileManager.cs, отвечает за вкладку Файл в меню приложения
        /// </summary>

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.CreateNewFile();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.OpenFile();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.SaveFile();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.SaveAsFile();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.Exit();
        }

        /// <summary>
        /// Bar/CorManager.cs, отвечает за вкладку Правка в меню приложения
        /// </summary>

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Undo();
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Redo();
        }

        /// Реализация отменить и повторить: по одному символу за нажатие кнопки

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Copy();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Paste();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Delete();
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.SelectAll();
        }

        /// <summary>
        /// Bar/RefManager.cs, отвечает за вкладку Справка в меню приложения
        /// </summary>

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowHelp();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowAbout();
        }

        /// <summary>
        /// Крупные кнопки интерфейса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            _fileHandler.CreateNewFile();
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            _fileHandler.OpenFile();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            _fileHandler.SaveFile();
        }

        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            _corManager.Undo();
        }

        private void toolStripButtonRepeat_Click(object sender, EventArgs e)
        {
            _corManager.Redo();
        }

        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            _corManager.Copy();
        }

        private void toolStripButtonCut_Click(object sender, EventArgs e)
        {
            _corManager.Cut();
        }

        private void toolStripButtonInsert_Click(object sender, EventArgs e)
        {
            _corManager.Paste();
        }

        /// <summary>
        /// Запуск ПОКА ОСТАВИТЬ
        /// </summary>

        // Основной обработчик кнопки "Play"
        private void toolStripButtonPlay_Click(object sender, EventArgs e)
        {
            string code = richTextBox1.Text;

            // Лексер
            var lexer = new Lexer(code);
            var tokens = lexer.Tokenize();

            // Парсер
            var parser = new RecursiveDescentParser(tokens);
            parser.ParseStmt();

            // Вывод логов
            richTextBox3.Clear();
            richTextBox3.AppendText("№   | Метод       | Описание ->>\n");
            richTextBox3.AppendText("-----------------------------\n");
            foreach (var entry in parser.Log)
                richTextBox3.AppendText(entry + Environment.NewLine);

            // Ошибки
            richTextBox4.Clear();
            foreach (var error in parser.Errors)
                richTextBox4.AppendText(error + Environment.NewLine);

            // Подсветка
            HighlightErrors(code, parser.Errors);
        }






        /// <summary>
        /// Преобразует номер строки и столбца (начиная с 1) в индекс символа в строке.
        /// </summary>
        private int GetCharIndexFromLineAndColumn(string text, int line, int col)
        {
            string[] lines = text.Split('\n');
            int index = 0;
            for (int i = 0; i < line - 1 && i < lines.Length; i++)
            {
                index += lines[i].Length + 1;
            }
            index += (col - 1);
            return index;
        }

        /// <summary>
        /// Подсвечивает фрагменты, где обнаружены ошибки.
        /// Длина выделения определяется как длина ожидаемого токена (error.ExpectedToken.Length).
        /// </summary>
        private void HighlightErrors(string inputText, List<string> errorLog)
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White;

            foreach (string error in errorLog)
            {
                Match match = Regex.Match(error, @"токен: (.+)$");
                if (match.Success)
                {
                    string value = match.Groups[1].Value;
                    int index = inputText.IndexOf(value, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        richTextBox1.Select(index, value.Length);
                        richTextBox1.SelectionBackColor = Color.LightPink;
                    }
                }
            }

            richTextBox1.Select(0, 0);
        }




        /// <summary>
        /// Подсвечивает комментарии в richTextBox зеленым фоном.
        /// Для однострочных комментариев используется шаблон: "#" и все до конца строки.
        /// Для многострочных комментариев – шаблон для тройных кавычек (''' или """).
        /// </summary>

        private void HighlightCommentsInRichTextBox(RichTextBox richTextBox)
        {
            int selStart = richTextBox.SelectionStart;
            int selLength = richTextBox.SelectionLength;

            // Сброс цвета текста
            richTextBox.SelectAll();
            richTextBox.SelectionColor = Color.Black;
            richTextBox.DeselectAll();

            // Однострочные комментарии (//)
            string singleLinePattern = @"//.*";
            foreach (Match match in Regex.Matches(richTextBox.Text, singleLinePattern))
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = Color.Green;
            }

            // Многострочные комментарии (/* */)
            string multiLinePattern = @"/\*[\s\S]*?\*/";
            foreach (Match match in Regex.Matches(richTextBox.Text, multiLinePattern))
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = Color.Green;
            }

            richTextBox.Select(selStart, selLength);
            richTextBox.Focus();
        }

        // Метод для применения синтаксической подсветки (вызывается при изменении текста)
        private void ApplySyntaxHighlighting()
        {
            // Сброс стилей
            SetDefaultStyle();
            // Подсвечиваем комментарии (текст зеленый, фон стандартный)
            HighlightCommentsInRichTextBox(richTextBox1);
        }


        private int GetCharIndexFromLineAndPosition(RichTextBox rtb, int line, int position)
        {
            if (line < 0 || line >= rtb.Lines.Length) return -1;

            int charIndex = 0;
            for (int i = 0; i < line; i++)
            {
                charIndex += rtb.Lines[i].Length + 1; // учитываем символ переноса строки
            }
            return charIndex + position;
        }



        ///

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            _refManager.ShowHelp();
        }

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            _refManager.ShowAbout();
        }

        
    }
}
