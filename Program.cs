using FindXml;

Console.WriteLine(Consts.DESC);

while (true)
{
    var logger = Logger.UpdateLogFileName();
    Console.WriteLine("Путь к файлу txt (со списком имен форм)/(лог-файлу еир рму):");
    var inputFile = Console.ReadLine();
    if (!string.IsNullOrEmpty(inputFile))
    {
        inputFile = Utils.NormalizePath(inputFile);
        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Файл не существует: {inputFile}");
            continue;
        }
    }

    Console.WriteLine("Путь к папке с формами (ищет рекурсивно):");
    var xmlFolder = Console.ReadLine();
    if (!string.IsNullOrEmpty(xmlFolder))
    {
        xmlFolder = Utils.NormalizePath(xmlFolder);
        if (!Directory.Exists(xmlFolder))
        {
            Console.WriteLine($"Папка не существует: {xmlFolder}");
            continue;
        }
    }

    Console.WriteLine("Куда сложить найденные файлы:");
    var resultFolder = Console.ReadLine();
    if (!string.IsNullOrEmpty(resultFolder))
    {
        resultFolder = Utils.NormalizePath(resultFolder);
        if (!Directory.Exists(resultFolder))
        {
            Console.WriteLine($"Папка не существует: {resultFolder}");
            continue;
        }
    }

    logger.Write($"Разбор файла {Path.GetFileName(inputFile)}");
    var resultFiles = new HashSet<string>();

    if (inputFile!.EndsWith(".txt") && !inputFile.Contains("report"))
    {
        logger.Write($"Ищу файлы");
        var fileNames = File.ReadAllLines(inputFile).Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();
        foreach (var fileName in fileNames)
        {
            try
            {
                var fileXML = Finder.GetFileByName(fileName, xmlFolder!);
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

    else if (inputFile!.EndsWith(".txt") && inputFile.Contains("report"))
    {
        logger.Write($"Ищу файлы");
        var parser = new ParserEIRRMULog(inputFile);
        var fileNames = parser.GetFileNames();
        foreach (var fileName in fileNames)
        {
            try
            {
                var fileXML = Finder.GetFileByName(fileName, xmlFolder!);
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

    logger.Write($"Копирую найденные файлы");
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