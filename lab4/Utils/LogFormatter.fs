module Utils.LogFormatter

open LogCollector.Parser

let formatLogEntry (log: LogEntry) =
    sprintf
        "[%s] %s: %s (Source: %s)"
        (log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"))
        log.LogLevel
        log.Message
        log.Source

let formatLogs (logs: LogEntry seq) =
    logs
    |> Seq.map formatLogEntry
    |> String.concat "\n"

let formatLogsAsMarkdown (logs: LogEntry seq) =
    logs
    |> Seq.map (fun log ->
        sprintf
            "- **[%s] %s**: %s (Source: %s)"
            (log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"))
            log.LogLevel
            log.Message
            log.Source)
    |> String.concat "\n"
