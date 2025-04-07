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

        private readonly List<float> _defaultFontSizes = new List<float> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24 };
        private RawTextParser _recoveryParser = new RawTextParser();
        private readonly LexicalAnalyzer _lexer = new LexicalAnalyzer();

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
            richTextBox1.TextChanged += RichTextBox1_TextChanged;

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
            // Лексический анализ
            _lexer.Analyze(richTextBox1.Text);
            dataGridView1.Rows.Clear();
            foreach (var token in _lexer.Tokens)
            {
                dataGridView1.Rows.Add(token.Code, token.Type, token.Value, token.Position);
            }

            // Синтаксический анализ (по тексту, не по токенам!)
            var parser = new RawTextParser();
            var errors = parser.ParseWithRecovery(richTextBox1.Text);
            dataGridView2.Rows.Clear();
            foreach (var error in errors)
            {
                dataGridView2.Rows.Add(
                    error.NumberOfError,
                    error.Message,
                    error.ExpectedToken,
                    $"Строка {error.Line}, Позиция {error.Column}"
                );
            }

            // Сначала сбросим стиль (чтобы убрать предыдущую подсветку)
            SetDefaultStyle();
            // Подсветка комментариев (зелёным)
            HighlightCommentsInRichTextBox(richTextBox1);
            // Подсветка ошибок (розовым)
            HighlightErrorsInRichTextBox(richTextBox1, errors);
        }

        private void нейтрализацияОшибокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Исправляем текст и получаем ошибки исходного текста
            _recoveryParser.AutoCorrectErrors();
            string originalText = richTextBox1.Text;
            var errors = _recoveryParser.ParseWithRecovery(originalText);
            string correctedText = _recoveryParser.GetCorrectedText();

            // Обновляем текст
            richTextBox1.Text = correctedText;

            // Пересчитываем ошибки для исправленного текста
            _recoveryParser.AutoCorrectErrors();
            var correctedErrors = _recoveryParser.ParseWithRecovery(correctedText);

            // Обновляем таблицу ошибок
            dataGridView2.Rows.Clear();
            foreach (var error in correctedErrors)
            {
                dataGridView2.Rows.Add(
                    error.NumberOfError,
                    error.Message,
                    error.ExpectedToken,
                    $"Строка {error.Line}, Позиция {error.Column}"
                );
            }

            // Обновляем подсветку
            SetDefaultStyle();
            HighlightCommentsInRichTextBox(richTextBox1);
            HighlightErrorsInRichTextBox(richTextBox1, correctedErrors);
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
        private void HighlightErrorsInRichTextBox(RichTextBox richTextBox, List<ParsingError> errors)
        {
            foreach (var error in errors)
            {
                int startIndex = GetCharIndexFromLineAndColumn(richTextBox.Text, error.Line, error.Column);
                int length = error.ExpectedToken.Length;
                if (startIndex + length > richTextBox.Text.Length)
                    length = richTextBox.Text.Length - startIndex;
                richTextBox.Select(startIndex, length);
                richTextBox.SelectionBackColor = Color.LightPink;
            }
            richTextBox.DeselectAll();
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
