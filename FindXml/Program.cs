using FindXml;

var currentDir = AppDomain.CurrentDomain.BaseDirectory;
var in_dir = Path.Combine(currentDir, "in");
if(!Directory.Exists(in_dir))
    Directory.CreateDirectory(in_dir);

var out_dir = Path.Combine(currentDir, "out");
if(!Directory.Exists(out_dir))
    Directory.CreateDirectory(out_dir);

while (true)
{
    var logger = Logger.UpdateLogFileName();
    Console.WriteLine($"Поместите лог-файл (или список с именами файлов) в папку {in_dir}.");
    var inputFile = Directory.GetFiles(in_dir).FirstOrDefault();
    

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
        if(string.IsNullOrWhiteSpace(inputFile))
        {
            Console.WriteLine($"Папка {in_dir} пуста: ");
            continue;
        }
    }

    logger.Write($"Разбор файла {Path.GetFileName(inputFile)}");
    var sourceFileTargetFile = new Dictionary<string, string>();

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

                if (!sourceFileTargetFile.ContainsKey(fileXML) && !string.IsNullOrEmpty(fileXML))
                {
                    sourceFileTargetFile[fileXML] = "";
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
                var sourceFile = Finder.GetFileByName(transfer.FileName, xmlFolder!);
                if (string.IsNullOrEmpty(sourceFile))
                {
                    logger.Write($"Не найден: {transfer.FileName}");
                }

                if (!sourceFileTargetFile.ContainsKey(sourceFile) && !string.IsNullOrEmpty(sourceFile))
                {
                    var targetFile = Filter.GetTargetFile(transfer, sourceFile, out_dir);
                    sourceFileTargetFile[sourceFile] = targetFile;
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
    foreach (var pair in sourceFileTargetFile)
    {
        try
        {
            var fileDir = Path.GetDirectoryName(pair.Value);
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir!);
            File.Copy(pair.Key, pair.Value, true);
        }
        catch (Exception ex)
        {
            logger.Write($"Не удалось скопировать файл {pair}: {ex}");
        }
    }

    Console.WriteLine($"Найдено файлов: {sourceFileTargetFile.Count}. Результат в папке {out_dir}");
    Console.ReadLine();
}