module ReportGenerator.MarkdownReport

open System
open System.IO
open ReportGenerator.ReportTemplates
open LogCollector.Parser
open LogCollector.MetricsAnalyzer

let reportDirectory = "./reports"

let initializeReportStorage () =
    if not (Directory.Exists(reportDirectory)) then
        Directory.CreateDirectory(reportDirectory)
        |> ignore
    
let generateMarkdownReport (containerName: string) (logs: LogEntry seq) =
    let metrics = analyzeMetrics logs
    let template = getMarkdownTemplate ()

    let formattedLogs =
        logs
        |> Seq.map (fun log ->
            sprintf
                "- [%s] **%s**: %s (Source: %s)"
                (log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"))
                log.LogLevel
                log.Message
                log.Source)
        |> String.concat "\n"

    let markdownReport =
        template
            .Replace("{containerName}", containerName)
            .Replace("{date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            .Replace("{errorCount}", metrics.ErrorCount.ToString())
            .Replace("{warningCount}", metrics.WarningCount.ToString())
            .Replace("{infoCount}", metrics.InfoCount.ToString())
            .Replace("{logs}", formattedLogs)

    markdownReport

let saveReport (containerName: string) (markdownContent: string) =
    let fileName = $"""{containerName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.md"""
    let filePath = Path.Combine(reportDirectory, fileName)
    File.WriteAllText(filePath, markdownContent)
    filePath

let createAndSaveReport (containerName: string) (logs: LogEntry seq) =
    initializeReportStorage ()
    let report = generateMarkdownReport containerName logs
    let filePath = saveReport containerName report
    printfn "Report saved to: %s" filePath
    filePath
