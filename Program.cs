using FindXml;

Console.WriteLine(Consts.DESC);

while (true)
{
    var logger = Logger.UpdateLogFileName();
    Console.WriteLine("Путь к файлу txt со списком имен форм:");
    var inputFile = Utils.GetFile(Console.ReadLine());
    if (!File.Exists(inputFile))
    {
        Console.WriteLine($"Файл не существует: {inputFile}");
        continue;
    }

    Console.WriteLine("Путь к папке с формами (ищет рекурсивно):");
    var xmlFolder = Utils.GetPath(Console.ReadLine());
    if (!Directory.Exists(xmlFolder))
    {
        Console.WriteLine($"Папка не существует: {xmlFolder}");
        continue;
    }

    Console.WriteLine("Куда сложить найденные файлы:");
    var resultFolder = Utils.GetPath(Console.ReadLine());
    if (!Directory.Exists(resultFolder))
    {
        Console.WriteLine($"Папка не существует: {resultFolder}");
        continue;
    }

    logger.Write($"Разбор файла {Path.GetFileName(inputFile)}");
    var resultFiles = new HashSet<string>();

    if (inputFile.EndsWith(".txt"))
    {
        logger.Write($"Ищу файлы");
        var fileNames = File.ReadAllLines(inputFile);
        foreach (var fileName in fileNames)
        {
            try
            {
                var fileXML = Finder.GetFileByName(fileName, xmlFolder);
                if (string.IsNullOrEmpty(fileXML))
                {
                    logger.Write($"Не найден: {fileName}");
                }

                if (!resultFiles.Contains(fileXML) && !string.IsNullOrEmpty(fileXML))
                {
                    resultFiles.Add(fileXML);
                    logger.Write($"Обнаружен: {fileName}");
                }
            }
            catch (Exception ex)
            {
                logger.Write($"{ex}");
            }
        }
    }
    logger.Write($"Копирую файлы");
    foreach (var file in resultFiles)
    {
        try
        {
            var fileName = Path.GetFileName(file);
            var newFile = resultFolder + "\\" + fileName;
            File.Copy(file, newFile, true);
        }
        catch (Exception ex)
        {
            logger.Write($"Не удалось скопировать файл {file}: {ex}");
        }
    }

    Console.WriteLine($"Найдено файлов: {resultFiles.Count}. Результат в папке {resultFolder}");
    Console.ReadKey(); 
}