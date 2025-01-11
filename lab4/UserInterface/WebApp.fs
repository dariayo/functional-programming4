module UserInterface.WebApp

open Giraffe
open Microsoft.AspNetCore.Builder
open System.IO
open Microsoft.Extensions.FileProviders
open UserInterface.API

let webApp =
    choose [ route "/"
             >=> text "Welcome to the Log Collector Dashboard!"
             route "/logs" >=> htmlFile "Static/html/logs.html"
             route "/reports"
             >=> htmlFile "Static/html/reports.html"
             route "/run-report"
             >=> htmlFile "Static/html/index.html"
             subRoute "/api" (apiRoutes ()) ]

let configureServices (builder: WebApplicationBuilder) = builder.Services.AddGiraffe() |> ignore

let configureApp (app: WebApplication) =
    let staticPath = Path.Combine(Directory.GetCurrentDirectory(), "Static")

    if not (Directory.Exists(staticPath)) then
        printfn "Directory not found: %s" staticPath
    else
        printfn "Serving static files from: %s" staticPath

    app
        .UseStaticFiles(StaticFileOptions(FileProvider = new PhysicalFileProvider(staticPath), RequestPath = "/static"))
        .UseGiraffe(webApp)
