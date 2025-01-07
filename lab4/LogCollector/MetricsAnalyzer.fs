module LogCollector.MetricsAnalyzer

open LogCollector.Parser

type Metrics =
    { ErrorCount: int
      WarningCount: int
      InfoCount: int }

let analyzeMetrics (logs: LogEntry seq) =
    logs
    |> Seq.fold
        (fun acc log ->
            match log.LogLevel with
            | "ERROR" -> { acc with ErrorCount = acc.ErrorCount + 1 }
            | "WARN" -> { acc with WarningCount = acc.WarningCount + 1 }
            | "INFO" -> { acc with InfoCount = acc.InfoCount + 1 }
            | _ -> acc)
        { ErrorCount = 0
          WarningCount = 0
          InfoCount = 0 }

let printMetricsReport metrics =
    printfn "Error Count: %d" metrics.ErrorCount
    printfn "Warning Count: %d" metrics.WarningCount
    printfn "Info Count: %d" metrics.InfoCount
