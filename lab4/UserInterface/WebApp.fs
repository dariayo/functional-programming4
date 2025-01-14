module UserInterface.WebApp

open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Builder
open System.IO
open Microsoft.Extensions.FileProviders
open UserInterface.API

let runReportPage =
    html [] [
        head [] [
            meta [ _charset "UTF-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ str "Reports" ]
            link [ _rel "stylesheet"; _href "/Static/css/styles.css" ]
        ]
        body [] [
            div [ _class "logs__wrapper" ] [
                header [ _class "header" ] [
                    h1 [] [ str "Reports Dashboard" ]
                ]
                div [ _class "container logs__container" ] [
                    div [ _id "reports"; _class "logs" ] [
                        h2 [ _class "logs__header" ] [ str "Available reports" ]
                        div [ _class "logs__no-logs-wrapper" ] [
                            p [ _class "logs__no-logs" ] [ str "No reports :((((( " ]
                        ]
                    ]
                    div [ _id "report-content"; _class "report-content logs-content" ] [
                        h2 [ _class "logs__header" ] [ str "Report Content" ]
                        div [ _class "logs__content-empty-wrapper" ] [
                            p [ _class "logs__content-empty" ] [ str "Click on a report to view its content." ]
                        ]
                    ]
                ]
            ]
            script [ _type "module"; _src "/static/js/Program.fs.js" ] []
        ]
    ] |> htmlView
let runLogPage =
    html [] [
        head [] [
            meta [ _charset "UTF-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ str "Logs" ]
            link [ _rel "stylesheet"; _href "/Static/css/styles.css" ]
        ]
        body [] [
            div [ _class "logs__wrapper" ] [
                header [ _class "header" ] [
                    h1 [] [ str "Logs Dashboard" ]
                ]
                div [ _class "container logs__container" ] [
                    div [ _id "logs"; _class "logs" ] [
                        h2 [ _class "logs__header" ] [ str "Available logs" ]
                        div [ _class "logs__no-logs-wrapper" ] [
                            p [ _class "logs__no-logs" ] [ str "No logs :((((( " ]
                        ]
                    ]
                    div [ _id "log-content"; _class "logs-content" ] [
                        h2 [ _class "logs__header" ] [ str "Log Content" ]
                        div [ _class "logs__content-empty-wrapper" ] [
                            p [ _class "logs__content-empty" ] [ str "Click on a log entry to view its content." ]
                        ]
                    ]
                ]
            ]
            script [ _type "module"; _src "/static/js/Program.fs.js" ] []
        ]
    ] |> htmlView

let runMainPage =
    html [] [
        head [] [
            meta [ _charset "UTF-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ str "Run Report" ]
            link [ _rel "stylesheet"; _href "/Static/css/styles.css" ]
        ]
        body [] [
            div [ _class "wrapper" ] [
                header [ _class "header" ] [
                    h1 [] [ str "Run Report" ]
                ]
                div [ _class "container" ] [
                    div [ _class "body__wrapper" ] [
                        div [ _class "item__wrapper" ] [
                            form [ _id "runReportForm"; _class "report-form" ] [
                                label [ _for "containerName"; _class "report-form__label" ] [
                                    str "Container Name:"
                                ]
                                div [ _class "report-form__wrapper" ] [
                                    input [ 
                                        _type "text"
                                        _id "containerName"
                                        _class "report-form__input"
                                        _placeholder "Enter name"
                                        _required
                                    ]
                                    button [ _class "report-form__button"; _type "submit" ] [
                                        str "Run Report"
                                    ]
                                ]
                            ]
                            div [ _class "output__wrapper" ] [
                                div [ _class "output__label" ] [ str "Output:" ]
                                div [ _class "output__body"; _id "output1" ] []
                            ]
                        ]
                        div [ _class "item__wrapper" ] [
                            form [ _id "curlRequestForm"; _class "report-form" ] [
                                label [ _for "curlRequest"; _class "report-form__label" ] [
                                    str "Curl Request:"
                                ]
                                div [ _class "report-form__wrapper" ] [
                                    input [ 
                                        _type "text"
                                        _id "curlRequest"
                                        _class "report-form__input"
                                        _placeholder "curl -X POST \"http://user-service:8084/users/register?username=john2\""
                                        _required
                                    ]
                                    button [ _type "submit"; _class "report-form__button" ] [
                                        str "Send Request and Collect Logs"
                                    ]
                                ]
                            ]
                            div [ _class "output__wrapper" ] [
                                div [ _class "output__label" ] [ str "Output:" ]
                                div [ _class "output__body"; _id "output2" ] []
                            ]
                        ]
                    ]
                ]
                div [ _class "container" ] [
                    ul [ _class "footer" ] [
                        li [ _class "footer__item" ] [
                            a [ _class "footer__link"; _href "http://localhost:8080/logs" ] [
                                str "Logs"
                            ]
                        ]
                        li [ _class "footer__item" ] [
                            a [ _class "footer__link"; _href "http://localhost:8080/reports" ] [
                                str "Reports"
                            ]
                        ]
                    ]
                ]
            ]
            script [ _type "module"; _src "/static/js/Program.fs.js" ] []
        ]
    ] |> htmlView
let webApp =
    choose [
        route "/" >=> text "Welcome to the Log Collector Dashboard!"
        route "/logs" >=> runLogPage
        route "/reports" >=> runReportPage
        route "/run-report" >=> runMainPage 
        subRoute "/api" (apiRoutes ())
    ]

let configureServices (builder: WebApplicationBuilder) =
    builder.Services.AddGiraffe() |> ignore

let configureApp (app: WebApplication) =
    let staticPath = Path.Combine(Directory.GetCurrentDirectory(), "Static")

    if not (Directory.Exists(staticPath)) then
        printfn "Directory not found: %s" staticPath
    else
        printfn "Serving static files from: %s" staticPath

    app
        .UseStaticFiles(StaticFileOptions(
            FileProvider = new PhysicalFileProvider(staticPath),
            RequestPath = "/static"
        ))
        .UseGiraffe(webApp)
