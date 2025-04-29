using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using lab1_compiler.Bar;
using System.Text;



namespace lab1_compiler
{

    public partial class Compiler : Form
    {
        private bool _exitHandled = false;
        private readonly List<float> _defaultFontSizes = new List<float> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24 };
        private readonly LexicalAnalyzer _lexer = new LexicalAnalyzer();

        /// Берём функции для элементов меню
        private readonly FileManager _fileHandler;
        private readonly CorManager _corManager;
        private readonly RefManager _refManager;

        /// Ссылки на справочное руководство
        private const string _aboutPath = @"Resources\About.html";
        private const string _helpPath = @"Resources\Help.html";
        private const string _taskPath = @"Resources\Task.html";
        private const string _grammaryPath = @"Resources\Grammary.html";
        private const string _cgrammaryPath = @"Resources\CGrammary.html";
        private const string _manalysisPath = @"Resources\MAnalysis.html";
        private const string _diagErrPath = @"Resources\DiagErr.html";
        private const string _testPath = @"Resources\Test.html";
        private const string _listLibraryPath = @"Resources\ListLibrary.html";
        private const string _codePath = @"Resources\Code.html";

        public Compiler()
        {
            InitializeComponent();
            InitializeFontSizeComboBox();
            this.FormClosing += Form1_FormClosing;

            _fileHandler = new FileManager(this);
            _corManager = new CorManager(richTextBox1);
            _refManager = new RefManager(_helpPath, _aboutPath, _taskPath, _grammaryPath, _cgrammaryPath, _manalysisPath, _diagErrPath, _testPath, _listLibraryPath, _codePath);

            // Установка минимального размера (ширина, высота)
            this.MinimumSize = new Size(450, 300);

            // Изменили данные в окне ввода
            richTextBox1.DragEnter += RichTextBox_DragEnter;
            richTextBox1.DragDrop += RichTextBox_DragDrop;

            richTextBox1.TextChanged += RichTextBox_TextChanged;
            richTextBox1.VScroll += RichTextBox_VScroll;

            toolStripStatusLabel1.Text = "Compiler успешно запущена";
            richTextBox1.TextChanged += RichTextBox1_TextChanged;

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


        /// статусная строка
        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Запускаем лексический анализатор при изменении текста
            _lexer.Analyze(richTextBox1.Text);

            int tokenCount = _lexer.Tokens.Count;
            int errorCount = _lexer.Errors.Count;

            // Формируем сообщение в зависимости от результатов сканирования
            if (errorCount == 0)
            {
                toolStripStatusLabel1.Text = $"Сканирование выполнено успешно. Токенов: {tokenCount}";
            }
            else
            {
                toolStripStatusLabel1.Text = $"Обнаружено ошибок: {errorCount}. Токенов: {tokenCount}";
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
            // Очистка лидирующих пробелов перед комментариями
            CleanLeadingSpaces(richTextBox1);

            // Анализ текста и обновление таблицы
            _lexer.Analyze(richTextBox1.Text);
            dataGridView1.Rows.Clear();
            foreach (var token in _lexer.Tokens)
                dataGridView1.Rows.Add(token.Code, token.Type, token.Value, token.Position);

            // Сброс стилей (фон и цвет шрифта)
            SetDefaultStyle();
            // Подсветка комментариев: зеленый цвет шрифта
            HighlightComments(richTextBox1);
            // Подсветка первой ошибки: желтый фон
            HighlightFirstError(richTextBox1);
        }

        private void SetDefaultStyle()
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = richTextBox1.BackColor;
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            richTextBox1.DeselectAll();
        }

        private void CleanLeadingSpaces(RichTextBox rtb)
        {
            var lines = rtb.Lines;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var m = Regex.Match(line, "(?=//|#|/\\*)");
                if (m.Success)
                {
                    int pos = m.Index;
                    string before = line.Substring(0, pos);
                    string after = line.Substring(pos);
                    if (string.IsNullOrWhiteSpace(before))
                        lines[i] = after;
                    else
                        lines[i] = before.TrimEnd() + " " + after;
                }
            }
            rtb.Lines = lines;
        }

        private void HighlightComments(RichTextBox rtb)
        {
            var pattern = @"(//.*?$|#.*?$|/\*.*?\*/?)";
            foreach (Match m in Regex.Matches(rtb.Text, pattern, RegexOptions.Multiline | RegexOptions.Singleline))
            {
                rtb.Select(m.Index, m.Length);
                rtb.SelectionColor = Color.Green;
            }
            rtb.DeselectAll();
        }

        private void HighlightFirstError(RichTextBox rtb)
        {
            if (_lexer.Errors.Count == 0) return;
            string err = _lexer.Errors[0];
            var m = Regex.Match(err, @"line (\d+), col (\d+)");
            if (!m.Success) return;
            int line = int.Parse(m.Groups[1].Value);
            int col = int.Parse(m.Groups[2].Value);
            int idx = rtb.GetFirstCharIndexFromLine(line - 1) + (col - 1);
            rtb.Select(idx, 1);
            rtb.SelectionBackColor = Color.Yellow;
            rtb.DeselectAll();
            MessageBox.Show(err, "Ошибка анализа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        /// Подсвечивает комментарии в richTextBox зеленым фоном.
        /// Для однострочных комментариев используется шаблон: "#" и все до конца строки.
        /// Для многострочных комментариев – шаблон для тройных кавычек (''' или """).
        /// </summary>




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

        private void нейтрализацияОшибокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lexer.Errors.Count == 0) return;
            string err = _lexer.Errors[0];
            string text = richTextBox1.Text;

            if (err.StartsWith("Unterminated multi-line comment")) // многострочный
            {
                int openIdx = text.LastIndexOf("/*");
                if (openIdx >= 0)
                {
                    int pos = text.Length - 1;
                    char last = text[pos];
                    if (last == '*' || last == '/')
                        richTextBox1.Text = text.Substring(0, pos) + "*/";
                    else if (!text.Contains("*/"))
                        richTextBox1.AppendText("*/");
                }
                else if (!text.Contains("*/"))
                {
                    richTextBox1.AppendText("*/");
                }
            }
            else if (err.StartsWith("Неправильное начало комментария")) // однострочный
            {
                // там сообщение формируется так:
                // "Неправильное начало комментария '/x' at line L, col C"
                var match = Regex.Match(err,
                    @"Неправильное начало комментария '/(.)' at line (\d+), col (\d+)");
                if (match.Success)
                {
                    int line = int.Parse(match.Groups[2].Value);
                    int col = int.Parse(match.Groups[3].Value);

                    // абсолютный индекс в тексте
                    int idx = richTextBox1.GetFirstCharIndexFromLine(line - 1) + (col - 1);

                    // через StringBuilder вставляем второй '/'
                    var sb = new StringBuilder(richTextBox1.Text);
                    sb.Insert(idx + 1, "/");
                    richTextBox1.Text = sb.ToString();

                    // возвращаем курсор прямо после '//'
                    richTextBox1.SelectionStart = idx + 2;
                    richTextBox1.ScrollToCaret();
                }
            }

            _lexer.NeutralizeErrors();
            SetDefaultStyle();
        }



        private void постановкаЗадачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowTask();
        }

        private void грамматикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowGrammar();
        }

        private void классификацияГрамматикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowCGrammar();
        }

        private void методАнализаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowMAnalysis();
        }

        private void диагностикаИНейтрализацияОшибокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowDiagError();
        }

        private void тестовыйПримерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowTest();
        }

        private void списокЛитературыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowLibrary();
        }

        private void исходныйКодПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowCode();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_exitHandled) return;
            _exitHandled = true;
            _fileHandler.Exit();
        }
    }
}
