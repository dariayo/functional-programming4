module UserInterface.API

open Giraffe
open Microsoft.AspNetCore.Http
open System.IO
open System.Diagnostics

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

let apiRoutes () =
    choose [ route "/logs" >=> getLogsHandler
             route "/reports" >=> getReportsHandler
             routef "/reports/%s" getReportContentHandler
             routef "/logs/%s" getLogContentHandler
             POST >=> route "/runReport" >=> runReportHandler ]
