﻿namespace lab1_compiler
{
    partial class Compiler
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Compiler));
            файлToolStripMenuItem = new ToolStripMenuItem();
            создатьToolStripMenuItem = new ToolStripMenuItem();
            открытьToolStripMenuItem = new ToolStripMenuItem();
            сохранитьToolStripMenuItem = new ToolStripMenuItem();
            сохранитьКакToolStripMenuItem = new ToolStripMenuItem();
            выходToolStripMenuItem = new ToolStripMenuItem();
            правкаToolStripMenuItem = new ToolStripMenuItem();
            отменитьToolStripMenuItem = new ToolStripMenuItem();
            повторитьToolStripMenuItem = new ToolStripMenuItem();
            вырезатьToolStripMenuItem = new ToolStripMenuItem();
            копироватьToolStripMenuItem = new ToolStripMenuItem();
            вставитьToolStripMenuItem = new ToolStripMenuItem();
            удалитьToolStripMenuItem = new ToolStripMenuItem();
            выделитьВсеToolStripMenuItem = new ToolStripMenuItem();
            текстToolStripMenuItem = new ToolStripMenuItem();
            постановкаЗадачиToolStripMenuItem = new ToolStripMenuItem();
            грамматикаToolStripMenuItem = new ToolStripMenuItem();
            классификацияГрамматикиToolStripMenuItem = new ToolStripMenuItem();
            методАнализаToolStripMenuItem = new ToolStripMenuItem();
            диагностикаИНейтрализацияОшибокToolStripMenuItem = new ToolStripMenuItem();
            тестовыйПримерToolStripMenuItem = new ToolStripMenuItem();
            списокЛитературыToolStripMenuItem = new ToolStripMenuItem();
            исходныйКодПрограммыToolStripMenuItem = new ToolStripMenuItem();
            пускToolStripMenuItem = new ToolStripMenuItem();
            справкаToolStripMenuItem = new ToolStripMenuItem();
            вызовСправкиToolStripMenuItem = new ToolStripMenuItem();
            оПрограммеToolStripMenuItem = new ToolStripMenuItem();
            локализацияToolStripMenuItem = new ToolStripMenuItem();
            видToolStripMenuItem = new ToolStripMenuItem();
            menuStrip2 = new MenuStrip();
            toolStrip1 = new ToolStrip();
            toolStripButtonAdd = new ToolStripButton();
            toolStripButtonOpen = new ToolStripButton();
            toolStripButtonSave = new ToolStripButton();
            toolStripButtonCancel = new ToolStripButton();
            toolStripButtonRepeat = new ToolStripButton();
            toolStripButtonCopy = new ToolStripButton();
            toolStripButtonCut = new ToolStripButton();
            toolStripButtonInsert = new ToolStripButton();
            toolStripButtonPlay = new ToolStripButton();
            toolStripButtonHelp = new ToolStripButton();
            toolStripButtonAbout = new ToolStripButton();
            toolStripFontSizeComboBox = new ToolStripComboBox();
            NiceWindow = new SplitContainer();
            splitContainer1 = new SplitContainer();
            richTextBoxLineNumbers = new RichTextBox();
            richTextBox1 = new RichTextBox();
            richTextBox2 = new RichTextBox();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            menuStrip2.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NiceWindow).BeginInit();
            NiceWindow.Panel1.SuspendLayout();
            NiceWindow.Panel2.SuspendLayout();
            NiceWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // файлToolStripMenuItem
            // 
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { создатьToolStripMenuItem, открытьToolStripMenuItem, сохранитьToolStripMenuItem, сохранитьКакToolStripMenuItem, выходToolStripMenuItem });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Size = new Size(59, 24);
            файлToolStripMenuItem.Text = "Файл";
            // 
            // создатьToolStripMenuItem
            // 
            создатьToolStripMenuItem.Name = "создатьToolStripMenuItem";
            создатьToolStripMenuItem.Size = new Size(192, 26);
            создатьToolStripMenuItem.Text = "Создать";
            создатьToolStripMenuItem.Click += создатьToolStripMenuItem_Click;
            // 
            // открытьToolStripMenuItem
            // 
            открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            открытьToolStripMenuItem.Size = new Size(192, 26);
            открытьToolStripMenuItem.Text = "Открыть";
            открытьToolStripMenuItem.Click += открытьToolStripMenuItem_Click;
            // 
            // сохранитьToolStripMenuItem
            // 
            сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            сохранитьToolStripMenuItem.Size = new Size(192, 26);
            сохранитьToolStripMenuItem.Text = "Сохранить";
            сохранитьToolStripMenuItem.Click += сохранитьToolStripMenuItem_Click;
            // 
            // сохранитьКакToolStripMenuItem
            // 
            сохранитьКакToolStripMenuItem.Name = "сохранитьКакToolStripMenuItem";
            сохранитьКакToolStripMenuItem.Size = new Size(192, 26);
            сохранитьКакToolStripMenuItem.Text = "Сохранить как";
            сохранитьКакToolStripMenuItem.Click += сохранитьКакToolStripMenuItem_Click;
            // 
            // выходToolStripMenuItem
            // 
            выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            выходToolStripMenuItem.Size = new Size(192, 26);
            выходToolStripMenuItem.Text = "Выход";
            выходToolStripMenuItem.Click += выходToolStripMenuItem_Click;
            // 
            // правкаToolStripMenuItem
            // 
            правкаToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { отменитьToolStripMenuItem, повторитьToolStripMenuItem, вырезатьToolStripMenuItem, копироватьToolStripMenuItem, вставитьToolStripMenuItem, удалитьToolStripMenuItem, выделитьВсеToolStripMenuItem });
            правкаToolStripMenuItem.Name = "правкаToolStripMenuItem";
            правкаToolStripMenuItem.Size = new Size(74, 24);
            правкаToolStripMenuItem.Text = "Правка";
            // 
            // отменитьToolStripMenuItem
            // 
            отменитьToolStripMenuItem.Name = "отменитьToolStripMenuItem";
            отменитьToolStripMenuItem.Size = new Size(186, 26);
            отменитьToolStripMenuItem.Text = "Отменить";
            отменитьToolStripMenuItem.Click += отменитьToolStripMenuItem_Click;
            // 
            // повторитьToolStripMenuItem
            // 
            повторитьToolStripMenuItem.Name = "повторитьToolStripMenuItem";
            повторитьToolStripMenuItem.Size = new Size(186, 26);
            повторитьToolStripMenuItem.Text = "Повторить";
            повторитьToolStripMenuItem.Click += повторитьToolStripMenuItem_Click;
            // 
            // вырезатьToolStripMenuItem
            // 
            вырезатьToolStripMenuItem.Name = "вырезатьToolStripMenuItem";
            вырезатьToolStripMenuItem.Size = new Size(186, 26);
            вырезатьToolStripMenuItem.Text = "Вырезать";
            вырезатьToolStripMenuItem.Click += вырезатьToolStripMenuItem_Click;
            // 
            // копироватьToolStripMenuItem
            // 
            копироватьToolStripMenuItem.Name = "копироватьToolStripMenuItem";
            копироватьToolStripMenuItem.Size = new Size(186, 26);
            копироватьToolStripMenuItem.Text = "Копировать";
            копироватьToolStripMenuItem.Click += копироватьToolStripMenuItem_Click;
            // 
            // вставитьToolStripMenuItem
            // 
            вставитьToolStripMenuItem.Name = "вставитьToolStripMenuItem";
            вставитьToolStripMenuItem.Size = new Size(186, 26);
            вставитьToolStripMenuItem.Text = "Вставить";
            вставитьToolStripMenuItem.Click += вставитьToolStripMenuItem_Click;
            // 
            // удалитьToolStripMenuItem
            // 
            удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            удалитьToolStripMenuItem.Size = new Size(186, 26);
            удалитьToolStripMenuItem.Text = "Удалить";
            удалитьToolStripMenuItem.Click += удалитьToolStripMenuItem_Click;
            // 
            // выделитьВсеToolStripMenuItem
            // 
            выделитьВсеToolStripMenuItem.Name = "выделитьВсеToolStripMenuItem";
            выделитьВсеToolStripMenuItem.Size = new Size(186, 26);
            выделитьВсеToolStripMenuItem.Text = "Выделить все";
            выделитьВсеToolStripMenuItem.Click += выделитьВсеToolStripMenuItem_Click;
            // 
            // текстToolStripMenuItem
            // 
            текстToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { постановкаЗадачиToolStripMenuItem, грамматикаToolStripMenuItem, классификацияГрамматикиToolStripMenuItem, методАнализаToolStripMenuItem, диагностикаИНейтрализацияОшибокToolStripMenuItem, тестовыйПримерToolStripMenuItem, списокЛитературыToolStripMenuItem, исходныйКодПрограммыToolStripMenuItem });
            текстToolStripMenuItem.Name = "текстToolStripMenuItem";
            текстToolStripMenuItem.Size = new Size(59, 24);
            текстToolStripMenuItem.Text = "Текст";
            // 
            // постановкаЗадачиToolStripMenuItem
            // 
            постановкаЗадачиToolStripMenuItem.Name = "постановкаЗадачиToolStripMenuItem";
            постановкаЗадачиToolStripMenuItem.Size = new Size(363, 26);
            постановкаЗадачиToolStripMenuItem.Text = "Постановка задачи";
            // 
            // грамматикаToolStripMenuItem
            // 
            грамматикаToolStripMenuItem.Name = "грамматикаToolStripMenuItem";
            грамматикаToolStripMenuItem.Size = new Size(363, 26);
            грамматикаToolStripMenuItem.Text = "Грамматика";
            // 
            // классификацияГрамматикиToolStripMenuItem
            // 
            классификацияГрамматикиToolStripMenuItem.Name = "классификацияГрамматикиToolStripMenuItem";
            классификацияГрамматикиToolStripMenuItem.Size = new Size(363, 26);
            классификацияГрамматикиToolStripMenuItem.Text = "Классификация грамматики";
            // 
            // методАнализаToolStripMenuItem
            // 
            методАнализаToolStripMenuItem.Name = "методАнализаToolStripMenuItem";
            методАнализаToolStripMenuItem.Size = new Size(363, 26);
            методАнализаToolStripMenuItem.Text = "Метод анализа";
            // 
            // диагностикаИНейтрализацияОшибокToolStripMenuItem
            // 
            диагностикаИНейтрализацияОшибокToolStripMenuItem.Name = "диагностикаИНейтрализацияОшибокToolStripMenuItem";
            диагностикаИНейтрализацияОшибокToolStripMenuItem.Size = new Size(363, 26);
            диагностикаИНейтрализацияОшибокToolStripMenuItem.Text = "Диагностика и нейтрализация ошибок";
            // 
            // тестовыйПримерToolStripMenuItem
            // 
            тестовыйПримерToolStripMenuItem.Name = "тестовыйПримерToolStripMenuItem";
            тестовыйПримерToolStripMenuItem.Size = new Size(363, 26);
            тестовыйПримерToolStripMenuItem.Text = "Тестовый пример";
            // 
            // списокЛитературыToolStripMenuItem
            // 
            списокЛитературыToolStripMenuItem.Name = "списокЛитературыToolStripMenuItem";
            списокЛитературыToolStripMenuItem.Size = new Size(363, 26);
            списокЛитературыToolStripMenuItem.Text = "Список литературы";
            // 
            // исходныйКодПрограммыToolStripMenuItem
            // 
            исходныйКодПрограммыToolStripMenuItem.Name = "исходныйКодПрограммыToolStripMenuItem";
            исходныйКодПрограммыToolStripMenuItem.Size = new Size(363, 26);
            исходныйКодПрограммыToolStripMenuItem.Text = "Исходный код программы";
            // 
            // пускToolStripMenuItem
            // 
            пускToolStripMenuItem.Name = "пускToolStripMenuItem";
            пускToolStripMenuItem.Size = new Size(55, 24);
            пускToolStripMenuItem.Text = "Пуск";
            // 
            // справкаToolStripMenuItem
            // 
            справкаToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { вызовСправкиToolStripMenuItem, оПрограммеToolStripMenuItem });
            справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            справкаToolStripMenuItem.Size = new Size(81, 24);
            справкаToolStripMenuItem.Text = "Справка";
            // 
            // вызовСправкиToolStripMenuItem
            // 
            вызовСправкиToolStripMenuItem.Name = "вызовСправкиToolStripMenuItem";
            вызовСправкиToolStripMenuItem.Size = new Size(197, 26);
            вызовСправкиToolStripMenuItem.Text = "Вызов справки";
            вызовСправкиToolStripMenuItem.Click += вызовСправкиToolStripMenuItem_Click;
            // 
            // оПрограммеToolStripMenuItem
            // 
            оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            оПрограммеToolStripMenuItem.Size = new Size(197, 26);
            оПрограммеToolStripMenuItem.Text = "О программе";
            оПрограммеToolStripMenuItem.Click += оПрограммеToolStripMenuItem_Click;
            // 
            // локализацияToolStripMenuItem
            // 
            локализацияToolStripMenuItem.Name = "локализацияToolStripMenuItem";
            локализацияToolStripMenuItem.Size = new Size(115, 24);
            локализацияToolStripMenuItem.Text = "Локализация";
            // 
            // видToolStripMenuItem
            // 
            видToolStripMenuItem.Name = "видToolStripMenuItem";
            видToolStripMenuItem.Size = new Size(49, 24);
            видToolStripMenuItem.Text = "Вид";
            // 
            // menuStrip2
            // 
            menuStrip2.ImageScalingSize = new Size(20, 20);
            menuStrip2.Items.AddRange(new ToolStripItem[] { файлToolStripMenuItem, правкаToolStripMenuItem, текстToolStripMenuItem, пускToolStripMenuItem, справкаToolStripMenuItem, локализацияToolStripMenuItem, видToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Padding = new Padding(7, 3, 0, 3);
            menuStrip2.Size = new Size(807, 30);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonAdd, toolStripButtonOpen, toolStripButtonSave, toolStripButtonCancel, toolStripButtonRepeat, toolStripButtonCopy, toolStripButtonCut, toolStripButtonInsert, toolStripButtonPlay, toolStripButtonHelp, toolStripButtonAbout, toolStripFontSizeComboBox });
            toolStrip1.Location = new Point(0, 30);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(807, 57);
            toolStrip1.TabIndex = 16;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAdd
            // 
            toolStripButtonAdd.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonAdd.Image = (Image)resources.GetObject("toolStripButtonAdd.Image");
            toolStripButtonAdd.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonAdd.ImageTransparentColor = Color.Magenta;
            toolStripButtonAdd.Name = "toolStripButtonAdd";
            toolStripButtonAdd.Size = new Size(54, 54);
            toolStripButtonAdd.Text = "Создать";
            toolStripButtonAdd.Click += toolStripButtonAdd_Click;
            // 
            // toolStripButtonOpen
            // 
            toolStripButtonOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonOpen.Image = (Image)resources.GetObject("toolStripButtonOpen.Image");
            toolStripButtonOpen.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonOpen.ImageTransparentColor = Color.Magenta;
            toolStripButtonOpen.Name = "toolStripButtonOpen";
            toolStripButtonOpen.Size = new Size(54, 54);
            toolStripButtonOpen.Text = "Открыть";
            toolStripButtonOpen.Click += toolStripButtonOpen_Click;
            // 
            // toolStripButtonSave
            // 
            toolStripButtonSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonSave.Image = (Image)resources.GetObject("toolStripButtonSave.Image");
            toolStripButtonSave.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonSave.ImageTransparentColor = Color.Magenta;
            toolStripButtonSave.Name = "toolStripButtonSave";
            toolStripButtonSave.Size = new Size(54, 54);
            toolStripButtonSave.Text = "Сохранить";
            toolStripButtonSave.Click += toolStripButtonSave_Click;
            // 
            // toolStripButtonCancel
            // 
            toolStripButtonCancel.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonCancel.Image = (Image)resources.GetObject("toolStripButtonCancel.Image");
            toolStripButtonCancel.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonCancel.ImageTransparentColor = Color.Magenta;
            toolStripButtonCancel.Name = "toolStripButtonCancel";
            toolStripButtonCancel.Size = new Size(54, 54);
            toolStripButtonCancel.Text = "Отменить";
            toolStripButtonCancel.Click += toolStripButtonCancel_Click;
            // 
            // toolStripButtonRepeat
            // 
            toolStripButtonRepeat.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonRepeat.Image = (Image)resources.GetObject("toolStripButtonRepeat.Image");
            toolStripButtonRepeat.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonRepeat.ImageTransparentColor = Color.Magenta;
            toolStripButtonRepeat.Name = "toolStripButtonRepeat";
            toolStripButtonRepeat.Size = new Size(54, 54);
            toolStripButtonRepeat.Text = "Повторить";
            toolStripButtonRepeat.Click += toolStripButtonRepeat_Click;
            // 
            // toolStripButtonCopy
            // 
            toolStripButtonCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonCopy.Image = (Image)resources.GetObject("toolStripButtonCopy.Image");
            toolStripButtonCopy.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonCopy.ImageTransparentColor = Color.Magenta;
            toolStripButtonCopy.Name = "toolStripButtonCopy";
            toolStripButtonCopy.Size = new Size(54, 54);
            toolStripButtonCopy.Text = "Копировать";
            toolStripButtonCopy.Click += toolStripButtonCopy_Click;
            // 
            // toolStripButtonCut
            // 
            toolStripButtonCut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonCut.Image = (Image)resources.GetObject("toolStripButtonCut.Image");
            toolStripButtonCut.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonCut.ImageTransparentColor = Color.Magenta;
            toolStripButtonCut.Name = "toolStripButtonCut";
            toolStripButtonCut.Size = new Size(54, 54);
            toolStripButtonCut.Text = "Вырезать";
            toolStripButtonCut.Click += toolStripButtonCut_Click;
            // 
            // toolStripButtonInsert
            // 
            toolStripButtonInsert.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonInsert.Image = (Image)resources.GetObject("toolStripButtonInsert.Image");
            toolStripButtonInsert.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonInsert.ImageTransparentColor = Color.Magenta;
            toolStripButtonInsert.Name = "toolStripButtonInsert";
            toolStripButtonInsert.Size = new Size(54, 54);
            toolStripButtonInsert.Text = "Вставить";
            toolStripButtonInsert.Click += toolStripButtonInsert_Click;
            // 
            // toolStripButtonPlay
            // 
            toolStripButtonPlay.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonPlay.Image = (Image)resources.GetObject("toolStripButtonPlay.Image");
            toolStripButtonPlay.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonPlay.ImageTransparentColor = Color.Magenta;
            toolStripButtonPlay.Name = "toolStripButtonPlay";
            toolStripButtonPlay.Size = new Size(54, 54);
            toolStripButtonPlay.Text = "Пуск";
            toolStripButtonPlay.Click += toolStripButtonPlay_Click;
            // 
            // toolStripButtonHelp
            // 
            toolStripButtonHelp.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonHelp.Image = (Image)resources.GetObject("toolStripButtonHelp.Image");
            toolStripButtonHelp.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonHelp.ImageTransparentColor = Color.Magenta;
            toolStripButtonHelp.Name = "toolStripButtonHelp";
            toolStripButtonHelp.Size = new Size(54, 54);
            toolStripButtonHelp.Text = "Вызов справки";
            toolStripButtonHelp.Click += toolStripButtonHelp_Click;
            // 
            // toolStripButtonAbout
            // 
            toolStripButtonAbout.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonAbout.Image = (Image)resources.GetObject("toolStripButtonAbout.Image");
            toolStripButtonAbout.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButtonAbout.ImageTransparentColor = Color.Magenta;
            toolStripButtonAbout.Name = "toolStripButtonAbout";
            toolStripButtonAbout.Size = new Size(54, 54);
            toolStripButtonAbout.Text = "О программе";
            toolStripButtonAbout.Click += toolStripButtonAbout_Click;
            // 
            // toolStripFontSizeComboBox
            // 
            toolStripFontSizeComboBox.Font = new Font("Segoe UI", 14F);
            toolStripFontSizeComboBox.Name = "toolStripFontSizeComboBox";
            toolStripFontSizeComboBox.Size = new Size(85, 57);
            // 
            // NiceWindow
            // 
            NiceWindow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            NiceWindow.ImeMode = ImeMode.NoControl;
            NiceWindow.Location = new Point(14, 112);
            NiceWindow.Margin = new Padding(3, 4, 3, 4);
            NiceWindow.Name = "NiceWindow";
            NiceWindow.Orientation = Orientation.Horizontal;
            // 
            // NiceWindow.Panel1
            // 
            NiceWindow.Panel1.AutoScroll = true;
            NiceWindow.Panel1.Controls.Add(splitContainer1);
            NiceWindow.Panel1.RightToLeft = RightToLeft.No;
            // 
            // NiceWindow.Panel2
            // 
            NiceWindow.Panel2.AutoScroll = true;
            NiceWindow.Panel2.Controls.Add(richTextBox2);
            NiceWindow.Panel2.RightToLeft = RightToLeft.No;
            NiceWindow.Size = new Size(779, 527);
            NiceWindow.SplitterDistance = 262;
            NiceWindow.SplitterWidth = 5;
            NiceWindow.TabIndex = 19;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(3, 4, 3, 4);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(richTextBoxLineNumbers);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(richTextBox1);
            splitContainer1.Size = new Size(779, 262);
            splitContainer1.SplitterDistance = 44;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // richTextBoxLineNumbers
            // 
            richTextBoxLineNumbers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            richTextBoxLineNumbers.Location = new Point(3, 4);
            richTextBoxLineNumbers.Margin = new Padding(3, 4, 3, 4);
            richTextBoxLineNumbers.Name = "richTextBoxLineNumbers";
            richTextBoxLineNumbers.ReadOnly = true;
            richTextBoxLineNumbers.ScrollBars = RichTextBoxScrollBars.None;
            richTextBoxLineNumbers.Size = new Size(43, 252);
            richTextBoxLineNumbers.TabIndex = 0;
            richTextBoxLineNumbers.Text = "";
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.EnableAutoDragDrop = true;
            richTextBox1.Location = new Point(3, 4);
            richTextBox1.Margin = new Padding(3, 4, 3, 4);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(722, 252);
            richTextBox1.TabIndex = 25;
            richTextBox1.Text = "";
            richTextBox1.WordWrap = false;
            // 
            // richTextBox2
            // 
            richTextBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox2.BackColor = SystemColors.ControlDark;
            richTextBox2.Location = new Point(3, 4);
            richTextBox2.Margin = new Padding(3, 4, 3, 4);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.ReadOnly = true;
            richTextBox2.Size = new Size(775, 253);
            richTextBox2.TabIndex = 19;
            richTextBox2.Text = "тестовый текст";
            richTextBox2.WordWrap = false;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 694);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(807, 26);
            statusStrip1.TabIndex = 20;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(151, 20);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // Compiler
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(807, 720);
            Controls.Add(statusStrip1);
            Controls.Add(NiceWindow);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip2);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Compiler";
            Text = "Compiler";
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            NiceWindow.Panel1.ResumeLayout(false);
            NiceWindow.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NiceWindow).EndInit();
            NiceWindow.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem правкаToolStripMenuItem;
        private ToolStripMenuItem текстToolStripMenuItem;
        private ToolStripMenuItem пускToolStripMenuItem;
        private ToolStripMenuItem справкаToolStripMenuItem;
        private ToolStripMenuItem локализацияToolStripMenuItem;
        private ToolStripMenuItem видToolStripMenuItem;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem создатьToolStripMenuItem;
        private ToolStripMenuItem открытьToolStripMenuItem;
        private ToolStripMenuItem сохранитьToolStripMenuItem;
        private ToolStripMenuItem сохранитьКакToolStripMenuItem;
        private ToolStripMenuItem выходToolStripMenuItem;
        private ToolStripMenuItem отменитьToolStripMenuItem;
        private ToolStripMenuItem повторитьToolStripMenuItem;
        private ToolStripMenuItem вырезатьToolStripMenuItem;
        private ToolStripMenuItem копироватьToolStripMenuItem;
        private ToolStripMenuItem вставитьToolStripMenuItem;
        private ToolStripMenuItem выделитьВсеToolStripMenuItem;
        private ToolStripMenuItem постановкаЗадачиToolStripMenuItem;
        private ToolStripMenuItem грамматикаToolStripMenuItem;
        private ToolStripMenuItem классификацияГрамматикиToolStripMenuItem;
        private ToolStripMenuItem методАнализаToolStripMenuItem;
        private ToolStripMenuItem диагностикаИНейтрализацияОшибокToolStripMenuItem;
        private ToolStripMenuItem тестовыйПримерToolStripMenuItem;
        private ToolStripMenuItem списокЛитературыToolStripMenuItem;
        private ToolStripMenuItem исходныйКодПрограммыToolStripMenuItem;
        private ToolStripMenuItem вызовСправкиToolStripMenuItem;
        private ToolStripMenuItem оПрограммеToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonAdd;
        private ToolStripButton toolStripButtonOpen;
        private ToolStripButton toolStripButtonSave;
        private ToolStripButton toolStripButtonCancel;
        private ToolStripButton toolStripButtonRepeat;
        private ToolStripButton toolStripButtonCopy;
        private ToolStripButton toolStripButtonCut;
        private ToolStripButton toolStripButtonInsert;
        private ToolStripButton toolStripButtonPlay;
        private ToolStripButton toolStripButtonHelp;
        private ToolStripButton toolStripButtonAbout;
        private ToolStripMenuItem удалитьToolStripMenuItem;
        private SplitContainer NiceWindow;
        private RichTextBox richTextBox2;
        private ToolStripComboBox toolStripFontSizeComboBox;
        private SplitContainer splitContainer1;
        private RichTextBox richTextBoxLineNumbers;
        private RichTextBox richTextBox1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
    }
}
