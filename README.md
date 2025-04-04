# Лабораторная работа 1  
**Тема:** Разработка пользовательского интерфейса (GUI) для языкового процессора  
- Цель: Разработать приложение – текстовый редактор.
---


## Основные функции  

### Файл  
- **Создать**  
  Создает новый файл или проект.  
- **Открыть**  
  Открывает существующий файл или проект из файловой системы.  
- **Сохранить**  
  Сохраняет текущий файл.  
- **Сохранить как**  
  Сохраняет текущий файл с новым именем или в новом месте.  
- **Выход**  
  Закрывает IDE.  

### Правка  
- **Отменить**  
  Отменяет последнее действие.  
- **Повторить**  
  Повторяет отмененное действие.  
- **Вырезать**  
  Удаляет выделенный текст или элемент и помещает его в буфер обмена.  
- **Копировать**  
  Копирует выделенный текст или элемент в буфер обмена.  
- **Вставить**  
  Вставляет содержимое буфера обмена в текущее место курсора.  
- **Удалить**  
  Удаляет выделенный текст или элемент без помещения в буфер обмена.  
- **Выделить все**  
  Выделяет весь текст или элемент в текущем окне или документе.  

---

# Лабораторная работа 2  
**Тема:** Разработка лексического анализатора (сканера)
- **Вариант 57. Комментарии языка PHP**
- **Вводные данные:** // Output "Hello GeeksforGeeks" 
/* It will print the 
   message   "Hello geeks" */
---
- **Цель работы:** Изучить назначение лексического анализатора. Спроектировать алгоритм и выполнить программную реализацию сканера.
- В соответствии с вариантом задания необходимо:
- Спроектировать диаграмму состояний сканера (примеры диаграмм представлены в прикрепленных файлах).
- Разработать лексический анализатор, позволяющий выделить в тексте лексемы, иные символы считать недопустимыми (выводить ошибку).
- Встроить сканер в ранее разработанный интерфейс текстового редактора. Учесть, что текст для разбора может состоять из множества строк.
- 
![Диаграмма сканера](https://github.com/user-attachments/assets/bc1ba0cb-119b-4fdd-af31-1c66ea73df20)
- рис.1 Диаграмма сканера

![изображение](https://github.com/user-attachments/assets/4998e098-1d80-4233-8dfd-f18ae1e1d808)
- рис.2 Работа сканера
