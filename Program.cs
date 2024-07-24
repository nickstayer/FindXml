using FindXml;

Console.WriteLine(Consts.DESC);

while (true)
{
    var logger = Logger.UpdateLogFileName();
    Console.WriteLine("Путь к файлу лога:");
    var inputFile = Utils.GetFile(Console.ReadLine());
    if (!File.Exists(inputFile))
    {
        Console.WriteLine($"Файл не существует: {inputFile}");
        continue;
    }

    Console.WriteLine("Путь к папке с XML:");
    var xmlFolder = Utils.GetPath(Console.ReadLine());
    if (!Directory.Exists(xmlFolder))
    {
        Console.WriteLine($"Папка не существует: {xmlFolder}");
        continue;
    }

    Console.WriteLine("Путь к папке с результатом:");
    var resultFolder = Utils.GetPath(Console.ReadLine());
    if (!Directory.Exists(resultFolder))
    {
        Console.WriteLine($"Папка не существует: {resultFolder}");
        continue;
    }

    logger.Write($"Разбор файла {Path.GetFileName(inputFile)}");
    logger.Write($"Обработка записей");
    var resultFiles = new HashSet<string>();


    // не исользуемый код начало
    if (inputFile.EndsWith(".csv")) 
    {
        var records = new ParserCSV(inputFile).Parse();

        foreach (var record in records)
        {
            try
            {
                var fileXML = Finder.GetFileByRecord(record, xmlFolder);
                if (string.IsNullOrEmpty(fileXML))
                {
                    logger.Write($"Не найдено: {record.FullName}");
                }

                if (!resultFiles.Contains(fileXML) && !string.IsNullOrEmpty(fileXML))
                {
                    resultFiles.Add(fileXML);
                    logger.Write($"{record.FullName}");
                }
            }
            catch (Exception ex)
            {
                logger.Write($"{ex}");
            }
        }
    }
    // не исользуемый код окончание

    else if (inputFile.EndsWith(".txt"))
    {
        var parser = new ParserEIRRMULog(inputFile);
        var fileNames = parser.GetFileNames();
        foreach (var fileName in fileNames)
        {
            try
            {
                var fileXML = Finder.GetFileByName(fileName, xmlFolder);
                if (string.IsNullOrEmpty(fileXML))
                {
                    logger.Write($"Не найдено: {fileName}");
                }

                if (!resultFiles.Contains(fileXML) && !string.IsNullOrEmpty(fileXML))
                {
                    resultFiles.Add(fileXML);
                    logger.Write($"{fileName}");
                }
            }
            catch (Exception ex)
            {
                logger.Write($"{ex}");
            }
        }
    }

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