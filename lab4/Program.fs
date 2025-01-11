module Program

open System.IO
open LogCollector.Parser
open ReportGenerator.MarkdownReport
open UserInterface.WebApp
open Microsoft.AspNetCore.Builder


[<EntryPoint>]
let main args =
    if args.Length < 3 then
        let builder = WebApplication.CreateBuilder(args)
        configureServices builder

        let app = builder.Build()
        configureApp app

        app.Run()
        0

    else
        let containerName = args.[0]
        let logsPath = args.[1]
        let reportsPath = args.[2]

        if not (Directory.Exists(reportsPath)) then
            Directory.CreateDirectory(reportsPath) |> ignore

        if not (File.Exists(logsPath)) then
            printfn "Logs file not found: %s" logsPath
            1
        else

            let logs = File.ReadLines(logsPath)
            let parsedLogs = logs |> Seq.choose parseLog

            let report = generateMarkdownReport containerName parsedLogs
            let reportPath = saveReport containerName report
            printfn "Report generated and saved to: %s" reportPath
            0
