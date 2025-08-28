using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace lab1_compiler.Bar
{
    internal class RefManager
    {
        private readonly string _helpFilePath;
        private readonly string _aboutFilePath;
        private readonly string _taskPath;
        private readonly string _grammaryPath;
        private readonly string _cgrammaryPath;
        private readonly string _manalysisPath;
        private readonly string _diagErrPath;
        private readonly string _testPath;
        private readonly string _listLibraryPath;
        private readonly string _codePath;

        public RefManager(
            string helpPath,
            string aboutPath,
            string taskPath,
            string grammaryPath,
            string cgrammaryPath,
            string manalysisPath,
            string diagErrPath,
            string testPath,
            string listLibraryPath,
            string codePath)
        {
            _helpFilePath = helpPath;
            _aboutFilePath = aboutPath;
            _taskPath = taskPath;
            _grammaryPath = grammaryPath;
            _cgrammaryPath = cgrammaryPath;
            _manalysisPath = manalysisPath;
            _diagErrPath = diagErrPath;
            _testPath = testPath;
            _listLibraryPath = listLibraryPath;
            _codePath = codePath;
        }

        // Существующие методы
        public void ShowHelp() => OpenHtmlFile(_helpFilePath, "Справка");
        public void ShowAbout() => OpenHtmlFile(_aboutFilePath, "О программе");

        // Новые методы для дополнительных ссылок
        public void ShowTask() => OpenHtmlFile(_taskPath, "Постановка задачи");
        public void ShowGrammar() => OpenHtmlFile(_grammaryPath, "Грамматика");
        public void ShowCGrammar() => OpenHtmlFile(_cgrammaryPath, "Классификация грамматики");
        public void ShowMAnalysis() => OpenHtmlFile(_manalysisPath, "Метод анализа");
        public void ShowDiagError() => OpenHtmlFile(_diagErrPath, "Диагностика ошибок");
        public void ShowTest() => OpenHtmlFile(_testPath, "Тестовый пример");
        public void ShowLibrary() => OpenHtmlFile(_listLibraryPath, "Список литературы");
        public void ShowCode() => OpenHtmlFile(_codePath, "Исходный код");

        private void OpenHtmlFile(string filePath, string title)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Файл {Path.GetFileName(filePath)} не найден!",
                                  "Ошибка",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}",
                              title,
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
    }
}