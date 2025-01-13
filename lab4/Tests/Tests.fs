open System
open LogCollector.Parser
open LogCollector.MetricsAnalyzer
open FsUnit.Xunit
open Xunit

module ParserTests =

    [<Fact>]
    let ``parseLog should return a Some LogEntry for valid log`` () =
        let log = "2025-01-13T12:00:00.000Z INFO 123 --- [main] myapp : POST /api/runReport"
        let result = parseLog log

        match result with
        | Some entry ->
            entry.Timestamp
            |> should equal (DateTime.Parse("2025-01-13T12:00:00.000Z"))

            entry.LogLevel |> should equal "INFO"

            entry.Message
            |> should equal "POST /api/runReport"

            entry.Source |> should equal "myapp"
            entry.Thread |> should equal "main"
        | None -> failwith "Expected Some, but got None"

    [<Fact>]
    let ``parseLog should return None for invalid log`` () =
        let log = "Invalid log format"
        let result = parseLog log
        result |> should equal None

module MetricsAnalyzerTests =

    [<Fact>]
    let ``analyzeMetrics should count error logs correctly`` () =
        let logs =
            [ { Timestamp = DateTime.Now
                LogLevel = "ERROR"
                Message = "Error message"
                Source = "Source1"
                Thread = "Thread1" }
              { Timestamp = DateTime.Now
                LogLevel = "INFO"
                Message = "Info message"
                Source = "Source2"
                Thread = "Thread2" } ]

        let result = analyzeMetrics logs
        result.ErrorCount |> should equal 1

    [<Fact>]
    let ``analyzeMetrics should count warning logs correctly`` () =
        let logs =
            [ { Timestamp = DateTime.Now
                LogLevel = "WARN"
                Message = "Warning message"
                Source = "Source1"
                Thread = "Thread1" }
              { Timestamp = DateTime.Now
                LogLevel = "INFO"
                Message = "Info message"
                Source = "Source2"
                Thread = "Thread2" } ]

        let result = analyzeMetrics logs
        result.WarningCount |> should equal 1

    [<Fact>]
    let ``analyzeMetrics should count info logs correctly`` () =
        let logs =
            [ { Timestamp = DateTime.Now
                LogLevel = "INFO"
                Message = "Info message"
                Source = "Source1"
                Thread = "Thread1" }
              { Timestamp = DateTime.Now
                LogLevel = "INFO"
                Message = "Another info message"
                Source = "Source2"
                Thread = "Thread2" } ]

        let result = analyzeMetrics logs
        result.InfoCount |> should equal 2

    [<Fact>]
    let ``analyzeMetrics should identify active threads`` () =
        let logs =
            [ { Timestamp = DateTime.Now
                LogLevel = "INFO"
                Message = "Message 1"
                Source = "Source1"
                Thread = "Thread1" }
              { Timestamp = DateTime.Now
                LogLevel = "INFO"
                Message = "Message 2"
                Source = "Source2"
                Thread = "Thread1" }
              { Timestamp = DateTime.Now
                LogLevel = "INFO"
                Message = "Message 3"
                Source = "Source3"
                Thread = "Thread2" } ]

        let result = analyzeMetrics logs

        result.ActiveThreads
        |> should contain "Thread: Thread1 (handling Message 1)"

        result.ActiveThreads
        |> should contain "Thread: Thread2 (handling Message 3)"

module MarkdownReportTests =

    open System.IO
    open ReportGenerator.MarkdownReport

    let sampleLogs =
        seq {
            { Timestamp = DateTime(2025, 1, 12, 13, 54, 16)
              LogLevel = "INFO"
              Message = "Starting Main v3.3.4 using Java 17.0.2 with PID 1"
              Source = "com.example.Main"
              Thread = "main" }

            { Timestamp = DateTime(2025, 1, 12, 13, 54, 19)
              LogLevel = "INFO"
              Message = "Tomcat initialized with port 8084 (http)"
              Source = "o.s.b.w.embedded.tomcat.TomcatWebServer"
              Thread = "main" }
        }

    [<Fact>]
    let ``initializeReportStorage create folder for reports, if it not exists`` () =
        let testDirectory = "./reports"

        if Directory.Exists(testDirectory) then
            Directory.Delete(testDirectory, true)

        initializeReportStorage ()

        Assert.True(Directory.Exists(testDirectory))
