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
        
        private readonly List<float> _defaultFontSizes = new List<float> { 8, 9, 10,11, 12, 14, 16, 18, 20,24 };

        private readonly LexicalAnalyzer _lexer = new LexicalAnalyzer();

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

        }

        private void InitializeDataGridViews()
        {
            // ��� dataGridView1
            dataGridView1.Columns.Add("Code", "���");
            dataGridView1.Columns.Add("Type", "���");
            dataGridView1.Columns.Add("Value", "�������");
            dataGridView1.Columns.Add("Position", "�������");

            // ��� dataGridView2
            dataGridView2.Columns.Add("Number", "�");
            dataGridView2.Columns.Add("Message", "������");
            dataGridView2.Columns.Add("Start", "������");
            dataGridView2.Columns.Add("End", "�����");
            dataGridView2.Columns.Add("Expected", "���������");
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


        /// ��������� ������
        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            // ��������� ����������� ���������� ��� ��������� ������
            _lexer.Analyze(richTextBox1.Text);

            int tokenCount = _lexer.Tokens.Count;
            int errorCount = _lexer.Errors.Count;

            // ��������� ��������� � ����������� �� ����������� ������������
            if (errorCount == 0)
            {
                toolStripStatusLabel1.Text = $"������������ ��������� �������. �������: {tokenCount}";
            }
            else
            {
                toolStripStatusLabel1.Text = $"���������� ������: {errorCount}. �������: {tokenCount}";
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

            return $"���������� � {fileName}{asterisk}{pathInfo}";
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
            // ����������� ������
            _lexer.Analyze(richTextBox1.Text);
            dataGridView1.Rows.Clear();

            foreach (var token in _lexer.Tokens)
            {
                dataGridView1.Rows.Add(
                    token.Code,
                    token.Type,
                    token.Value,
                    token.Position
                );
            }

            // �������������� ������ (�� ������, �� �� �������!)
            var parser = new RawTextParser();
            var errors = parser.Parse(richTextBox1.Text);

            dataGridView2.Rows.Clear();
            foreach (var error in errors)
            {
                dataGridView2.Rows.Add(
                    error.NumberOfError,
                    error.Message,
                    error.ExpectedToken,
                    $"������ {error.Line}, ������� {error.Column}"
                );
            }
        }




        private int GetCharIndexFromLineAndPosition(RichTextBox rtb, int line, int position)
        {
            if (line < 0 || line >= rtb.Lines.Length) return -1;

            int charIndex = 0;
            for (int i = 0; i < line; i++)
            {
                charIndex += rtb.Lines[i].Length + 1; // ��������� ������ �������� ������
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
