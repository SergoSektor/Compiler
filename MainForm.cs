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
        /// <summary>
        /// выкинуть в другой файл
        /// </summary>
        /// 
        // Список ключевых слов PHP
        private readonly string[] _phpKeywords = {
    "abstract", "and", "array", "as", "break", "callable", "case", "catch",
    "class", "clone", "const", "continue", "declare", "default", "die", "do",
    "echo", "else", "elseif", "empty", "enddeclare", "endfor", "endforeach",
    "endif", "endswitch", "endwhile", "eval", "exit", "extends", "final",
    "finally", "fn", "for", "foreach", "function", "global", "goto", "if",
    "implements", "include", "include_once", "instanceof", "insteadof",
    "interface", "isset", "list", "namespace", "new", "or", "print", "private",
    "protected", "public", "require", "require_once", "return", "static",
    "switch", "throw", "trait", "try", "unset", "use", "var", "while", "xor"
};

        private readonly Color _keywordColor = Color.Blue;
        private readonly Color _variableColor = Color.DarkOrange;

        private const uint CFM_BOLD = 0x00000001;
        private const uint CFM_COLOR = 0x40000000;
        private const uint CFE_BOLD = 0x00000001;
        private const int SCF_SELECTION = 0x0001;
        [StructLayout(LayoutKind.Sequential)]
        private struct CHARFORMAT2
        {
            public uint cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
        }

        // WinAPI для безопасного изменения стилей
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x0B;
        private const int EM_GETCHARFORMAT = 0x43A;
        private const int EM_SETCHARFORMAT = 0x444;

        private readonly List<float> _defaultFontSizes = new List<float> { 8, 9, 10,11, 12, 14, 16, 18, 20,24 };

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

            //richTextBox1.TextChanged += (sender, e) =>
            //{
            //    ApplySyntaxHighlighting(); // Вызов подсветки
            //                               // Другие действия (обновление статуса и т.д.)
            //};
        }

        //22222222222222222222222222222222



        private void ApplySyntaxHighlighting()
        {
            // Блокируем обновление интерфейса
            SendMessage(richTextBox1.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);

            // Сохраняем текущее состояние
            int originalStart = richTextBox1.SelectionStart;
            Color originalColor = richTextBox1.SelectionColor;
            Font originalFont = richTextBox1.SelectionFont;

            // Очищаем предыдущие стили
            SetDefaultStyle();

            // Подсветка ключевых слов
            foreach (var keyword in _phpKeywords)
            {
                HighlightMatches(@"\b" + Regex.Escape(keyword) + @"\b", _keywordColor, FontStyle.Bold);
            }

            // Подсветка переменных ($var)
            HighlightMatches(@"\$[a-zA-Z_]\w*", _variableColor, FontStyle.Regular);

            // Восстанавливаем состояние
            richTextBox1.SelectionStart = originalStart;
            richTextBox1.SelectionColor = originalColor;
            richTextBox1.SelectionFont = originalFont;

            // Разблокируем обновление интерфейса
            SendMessage(richTextBox1.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            richTextBox1.Invalidate();
        }
        private void SetDefaultStyle()
        {
            // Устанавливает стиль по умолчанию для всего текста
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
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


        ///
        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Проверяем, пустой ли RichTextBox
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                toolStripStatusLabel1.Text = "Ошибок нет";
            }
            else
            {
                toolStripStatusLabel1.Text = "Обнаружены ошибки";
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
                UpdateFontSize(richTextBox2, newSize);
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

        private void toolStripButtonPlay_Click(object sender, EventArgs e)
        {

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
