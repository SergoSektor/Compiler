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
        /// �������� � ������ ����
        /// </summary>
        /// 
        // ������ �������� ���� PHP
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

        // WinAPI ��� ����������� ��������� ������
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x0B;
        private const int EM_GETCHARFORMAT = 0x43A;
        private const int EM_SETCHARFORMAT = 0x444;

        private readonly List<float> _defaultFontSizes = new List<float> { 8, 9, 10,11, 12, 14, 16, 18, 20,24 };

        /// ���� ������� ��� ��������� ����
        private readonly FileManager _fileHandler;
        private readonly CorManager _corManager;
        private readonly RefManager _refManager;

        /// ������ �� ���������� �����������
        private const string _aboutPath = @"Resources\About.html";
        private const string _helpPath = @"Resources\Help.html";

        public Compiler()
        {
            InitializeComponent();
            InitializeFontSizeComboBox();
            _fileHandler = new FileManager(this);
            _corManager = new CorManager(richTextBox1);
            _refManager = new RefManager(_helpPath, _aboutPath);

            // ��������� ������������ ������� (������, ������)
            this.MinimumSize = new Size(450, 300);

            // �������� ������ � ���� �����
            richTextBox1.DragEnter += RichTextBox_DragEnter;
            richTextBox1.DragDrop += RichTextBox_DragDrop;

            richTextBox1.TextChanged += RichTextBox_TextChanged;
            richTextBox1.VScroll += RichTextBox_VScroll;

            toolStripStatusLabel1.Text = "Compiler ������� ��������";
            richTextBox1.TextChanged += RichTextBox1_TextChanged;

            //richTextBox1.TextChanged += (sender, e) =>
            //{
            //    ApplySyntaxHighlighting(); // ����� ���������
            //                               // ������ �������� (���������� ������� � �.�.)
            //};
        }

        //22222222222222222222222222222222



        private void ApplySyntaxHighlighting()
        {
            // ��������� ���������� ����������
            SendMessage(richTextBox1.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);

            // ��������� ������� ���������
            int originalStart = richTextBox1.SelectionStart;
            Color originalColor = richTextBox1.SelectionColor;
            Font originalFont = richTextBox1.SelectionFont;

            // ������� ���������� �����
            SetDefaultStyle();

            // ��������� �������� ����
            foreach (var keyword in _phpKeywords)
            {
                HighlightMatches(@"\b" + Regex.Escape(keyword) + @"\b", _keywordColor, FontStyle.Bold);
            }

            // ��������� ���������� ($var)
            HighlightMatches(@"\$[a-zA-Z_]\w*", _variableColor, FontStyle.Regular);

            // ��������������� ���������
            richTextBox1.SelectionStart = originalStart;
            richTextBox1.SelectionColor = originalColor;
            richTextBox1.SelectionFont = originalFont;

            // ������������ ���������� ����������
            SendMessage(richTextBox1.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            richTextBox1.Invalidate();
        }
        private void SetDefaultStyle()
        {
            // ������������� ����� �� ��������� ��� ����� ������
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
            // ���������, ������ �� RichTextBox
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                toolStripStatusLabel1.Text = "������ ���";
            }
            else
            {
                toolStripStatusLabel1.Text = "���������� ������";
            }


        }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RichTextBox_VScroll(object sender, EventArgs e)
        {
            // ������������� ��������� ����� richTextBox1 � richTextBoxLineNumbers
            int verticalScrollPos = GetFirstVisibleLineNumber() * richTextBoxLineNumbers.Font.Height;
            richTextBoxLineNumbers.SelectionStart = richTextBoxLineNumbers.GetCharIndexFromPosition(new Point(0, verticalScrollPos));
            richTextBoxLineNumbers.ScrollToCaret();
        }

        private int GetFirstVisibleLineNumber()
        {
            // ���������� ������ ������� ������ � richTextBox1
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
        /// ���������� ����������� ��������� ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_DragEnter(object sender, DragEventArgs e)
        {
            // ���������, ��� ��������������� ����
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // ��������� �����������
            }
            else
            {
                e.Effect = DragDropEffects.None; // ��������� ������ ���� ������
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
                            // ���� ������� ���������� ����� ��������� ������ �����
                            richTextBox1.Clear();

                            // ��������� ���������� ����� � ��������� ���������
                            _fileHandler.DragFile(filePath); // �������� ���� � �����
                            UpdateLineNumbers();
                            UpdateWindowTitle();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("������ ��������� ����� (.txt, .cs, .cpp, .java)", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        ///  ������������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_TextChanged(object? sender, EventArgs e)
        {
            // ��������� Undo/Redo ��� ������
            _corManager.PauseUndoTracking();
            //ApplySyntaxHighlighting();
            _corManager.ResumeUndoTracking();

            // ���������� ����� � ����������
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
                ? "����� ����.txt"
                : Path.GetFileName(filePath);

            var asterisk = _fileHandler.IsFileModified ? "*" : "";
            var pathInfo = string.IsNullOrEmpty(filePath)
                ? ""
                : $" ({filePath})";

            return $"Compiler � {fileName}{asterisk}{pathInfo}";
        }

        /// <summary>
        /// ������ ������
        /// </summary>

        private void InitializeFontSizeComboBox()
        {
            toolStripFontSizeComboBox.ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            toolStripFontSizeComboBox.ComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            // ��������� ������ ������������ ���������
            toolStripFontSizeComboBox.ComboBox.Items.AddRange(_defaultFontSizes.Cast<object>().ToArray());

            // ������������� ������� ������ ������
            toolStripFontSizeComboBox.ComboBox.Text = richTextBox1.Font.Size.ToString();

            // �������� �� �������
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
                // ������������ �������� ��� ���������� � ������
                newSize = Math.Clamp(newSize, 1, 99);

                // ��������� �����
                UpdateFontSize(richTextBox1, newSize);
                UpdateFontSize(richTextBox2, newSize);
                UpdateFontSize(richTextBoxLineNumbers, newSize);

                // ��������� ����� ��� ���������� � Items
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
            // ������ ������������� �����, �� ��������� ����� ��������
            toolStripFontSizeComboBox.ComboBox.Text = size.ToString();
        }

        /// <summary>
        /// Bar/FileManager.cs, �������� �� ������� ���� � ���� ����������
        /// </summary>

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.CreateNewFile();
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.OpenFile();
        }

        private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.SaveFile();
        }

        private void ������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.SaveAsFile();
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fileHandler.Exit();
        }

        /// <summary>
        /// Bar/CorManager.cs, �������� �� ������� ������ � ���� ����������
        /// </summary>

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Undo();
        }

        private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Redo();
        }

        /// ���������� �������� � ���������: �� ������ ������� �� ������� ������

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Cut();
        }

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Copy();
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Paste();
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.Delete();
        }

        private void �����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _corManager.SelectAll();
        }

        /// <summary>
        /// Bar/RefManager.cs, �������� �� ������� ������� � ���� ����������
        /// </summary>

        private void ������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowHelp();
        }

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _refManager.ShowAbout();
        }

        /// <summary>
        /// ������� ������ ����������
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
        /// ������ ���� ��������
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
