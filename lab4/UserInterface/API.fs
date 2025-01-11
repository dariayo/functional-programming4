module UserInterface.API

open Giraffe
open Microsoft.AspNetCore.Http
open System.IO
open System.Diagnostics
open System
open System.Text.Json
open Docker.DotNet
open Docker.DotNet.Models
open System.Threading.Tasks
open System.Text

type ApiResponse<'T> =
    { Data: 'T
      Success: bool
      ErrorMessage: string option }

let getLogsHandler (next: HttpFunc) (ctx: HttpContext) =
    task {
        let logsDir = "./logs"

        if Directory.Exists(logsDir) then
            let logs =
                Directory.EnumerateFiles(logsDir, "*.log")
                |> Seq.map (fun file -> Path.GetFileName(file))
                |> Seq.toList

            let response =
                { Data = logs
                  Success = true
                  ErrorMessage = None }

            return! json response next ctx
        else
            let errorResponse =
                { Data = []
                  Success = false
                  ErrorMessage = Some "Logs directory not found" }

            return! json errorResponse next ctx
    }

let getReportsHandler (next: HttpFunc) (ctx: HttpContext) =
    task {
        let reportDir = "./reports"

        if not (Directory.Exists(reportDir)) then
            Directory.CreateDirectory(reportDir) |> ignore

        let reports =
            Directory.EnumerateFiles(reportDir, "*.md")
            |> Seq.map Path.GetFileName
            |> Seq.toList

        let response =
            { Data = reports
              Success = true
              ErrorMessage = None }

        return! json response next ctx
    }

let getLogContentHandler (logName: string) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let logPath = Path.Combine("./logs", logName)

        if File.Exists(logPath) then
            let content = File.ReadAllText(logPath)

            let response =
                { Data = content
                  Success = true
                  ErrorMessage = None }

            return! json response next ctx
        else
            let errorResponse =
                { Data = ""
                  Success = false
                  ErrorMessage = Some "Log not found" }

            return! json errorResponse next ctx
    }

let getReportContentHandler (reportName: string) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let reportPath = Path.Combine("./reports", reportName)

        if File.Exists(reportPath) then
            let content = File.ReadAllText(reportPath)

            let response =
                { Data = content
                  Success = true
                  ErrorMessage = None }

            return! json response next ctx
        else
            let errorResponse =
                { Data = ""
                  Success = false
                  ErrorMessage = Some "Report not found" }

            return! json errorResponse next ctx
    }

let runReportHandler (next: HttpFunc) (ctx: HttpContext) =
    task {
        try
            let! body = ctx.ReadBodyFromRequestAsync()

            let containerName =
                match System
                          .Text
                          .Json
                          .JsonDocument
                          .Parse(body)
                          .RootElement.TryGetProperty("containerName")
                    with
                | true, value -> value.GetString()
                | false, _ -> null


            let scriptPath = "./scripts/report.sh"

            let process = new Process()
            process.StartInfo.FileName <- "bash"
            process.StartInfo.Arguments <- $"{scriptPath} {containerName}"
            process.StartInfo.RedirectStandardOutput <- true
            process.StartInfo.RedirectStandardError <- true
            process.StartInfo.UseShellExecute <- false
            process.StartInfo.CreateNoWindow <- true

            process.Start() |> ignore
            process.WaitForExit()

            let output = process.StandardOutput.ReadToEnd()
            let error = process.StandardError.ReadToEnd()

            if process.ExitCode = 0 then
                let successResponse =
                    { Success = true
                      ErrorMessage = None
                      Data = output }

                return! json successResponse next ctx
            else
                let errorResponse =
                    { Success = false
                      ErrorMessage = Some error
                      Data = null }

                return! json errorResponse next ctx
        with
        | ex ->
            let errorResponse =
                { Success = false
                  ErrorMessage = Some ex.Message
                  Data = null }

            return! json errorResponse next ctx
    }

let getContainerLogsAfterTime (client: IDockerClient) (containerId: string) (sinceTime: DateTime) : Task<string> =
    task {
        let unixTimestamp =
            (sinceTime
             - DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalSeconds
            |> int64

        let logsParams =
            ContainerLogsParameters(ShowStdout = true, ShowStderr = true, Since = unixTimestamp.ToString())

        use! stream = client.Containers.GetContainerLogsAsync(containerId, logsParams)
        use reader = new StreamReader(stream)

        let! rawLogs = reader.ReadToEndAsync()

        let cleanLogs =
            rawLogs.Split(Environment.NewLine)
            |> Array.map (fun line -> System.Text.RegularExpressions.Regex.Replace(line, @"^[^\d]*", ""))
            |> String.concat Environment.NewLine

        return cleanLogs
    }


let sendCurlRequestHandler (next: HttpFunc) (ctx: HttpContext) =
    task {
        try
            let! body = ctx.ReadBodyFromRequestAsync()

            let curlRequest =
                match JsonDocument
                          .Parse(body)
                          .RootElement.TryGetProperty("curlRequest")
                    with
                | true, value -> value.GetString()
                | false, _ -> null

            if String.IsNullOrEmpty(curlRequest) then
                let errorResponse =
                    { Success = false
                      ErrorMessage = Some "Curl request is empty."
                      Data = null }

                return! json errorResponse next ctx
            else
                use dockerClient: IDockerClient = DockerClientConfiguration().CreateClient()

                let startTime = DateTime.UtcNow

                let process = new Process()
                process.StartInfo.FileName <- "bash"
                process.StartInfo.Arguments <- $"-c \"{curlRequest}\""
                process.StartInfo.RedirectStandardOutput <- true
                process.StartInfo.RedirectStandardError <- true
                process.StartInfo.UseShellExecute <- false
                process.StartInfo.CreateNoWindow <- true

                process.Start() |> ignore
                let output = process.StandardOutput.ReadToEnd()
                let error = process.StandardError.ReadToEnd()
                process.WaitForExit()

                let logsDir = "./logs"
                let composeLogsFile = Path.Combine(logsDir, "docker_compose_logs.log")
                Directory.CreateDirectory(logsDir) |> ignore

                use writer = new StreamWriter(composeLogsFile, false, Encoding.UTF8)
                writer.WriteLine($"----- Logs after the curl request started at {startTime} -----")

                let! containers = dockerClient.Containers.ListContainersAsync(ContainersListParameters(All = true))

                let reportPaths = ref []

                for container in containers do
                    let! logs = getContainerLogsAfterTime dockerClient container.ID startTime
                    writer.WriteLine($"--- Logs from container {container.ID} ---")
                    writer.WriteLine(logs)
                    writer.WriteLine()

                    let logEntries =
                        logs.Split(Environment.NewLine)
                        |> Array.choose LogCollector.Parser.parseLog
                        |> Seq.ofArray

                    let reportPath =
                        ReportGenerator.MarkdownReport.createAndSaveReport container.Image logEntries

                    reportPaths := !reportPaths @ [ reportPath ]

                if process.ExitCode = 0 then
                    let successResponse =
                        { Success = true
                          ErrorMessage = None
                          Data = String.Join(", ", !reportPaths) }

                    return! json successResponse next ctx
                else
                    let errorResponse =
                        { Success = false
                          ErrorMessage = Some error
                          Data = null }

                    return! json errorResponse next ctx
        with
        | ex ->
            let errorResponse =
                { Success = false
                  ErrorMessage = Some ex.Message
                  Data = null }

            return! json errorResponse next ctx
    }



let apiRoutes () =
    choose [ route "/logs" >=> getLogsHandler
             route "/reports" >=> getReportsHandler
             routef "/reports/%s" getReportContentHandler
             routef "/logs/%s" getLogContentHandler
             route "/sendCurlRequest"
             >=> sendCurlRequestHandler
             POST >=> route "/runReport" >=> runReportHandler ]
