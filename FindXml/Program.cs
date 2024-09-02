using FindXml;

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
    var resultFileFolderName = new Dictionary<string, string>();
    
    // поиск по списку имен файлов
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

                if (!resultFileFolderName.ContainsKey(fileXML) && !string.IsNullOrEmpty(fileXML))
                {
                    resultFileFolderName[fileXML] = "";
                    logger.Write($"Обнаружен: {fileName}");
                }
            }
            catch (Exception ex)
            {
                logger.Write($"{ex}");
            }
        }
    }

    // парсинг лога еир рму
    else if (inputFile!.EndsWith(".txt") && inputFile.Contains("report"))
    {
        Console.WriteLine("Включаю разделы лога:");
        foreach (var section in Filter.INCLUDE_TRANSFER_STATUS)
            Console.WriteLine("\t" + section);

        Console.WriteLine("Исключаю описание ошибок, содержащее:");
        foreach (var keyword in Filter.EXCLUDE_ERROR_DESCRIPTION_KEYWORDS)
            Console.WriteLine("\t" + keyword);

        logger.Write($"Ищу файлы");
        var parser = new ParserEIRRMULog(inputFile);
        var transfers = parser.Parse();
        var filtredTransfers = Filter.Do(transfers);
        foreach (var transfer in filtredTransfers)
        {
            try
            {
                var fileXML = Finder.GetFileByName(transfer.FileName, xmlFolder!);
                if (string.IsNullOrEmpty(fileXML))
                {
                    logger.Write($"Не найден: {transfer.FileName}");
                }

                if (!resultFileFolderName.ContainsKey(fileXML) && !string.IsNullOrEmpty(fileXML))
                {
                    resultFileFolderName[fileXML] = Filter.GetFolderName(transfer);
                    logger.Write($"Обнаружен: {transfer.FileName}");
                }
            }
            catch (Exception ex)
            {
                logger.Write($"{ex}");
            }
        }
    }

    logger.Write($"Копирую найденные файлы");
    foreach (var pair in resultFileFolderName)
    {
        try
        {
            var fileName = Path.GetFileName(pair.Key);
            var subfolder = string.IsNullOrWhiteSpace(pair.Value) ? string.Empty : pair.Value;
            var newFile = Path.Combine(resultFolder!, pair.Value, fileName);
            var fileDir = Path.GetDirectoryName(newFile);
            if(!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir!);
            File.Copy(pair.Key, newFile, true);
        }
        catch (Exception ex)
        {
            logger.Write($"Не удалось скопировать файл {pair}: {ex}");
        }
    }

    Console.WriteLine($"Найдено файлов: {resultFileFolderName.Count}. Результат в папке {resultFolder}");
    Console.ReadLine();
}