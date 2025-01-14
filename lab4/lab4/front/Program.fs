open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Fetch
open Browser.Types

type RunReportData = {
    containerName: string
}

type CurlRequestData = {
    curlRequest: string
}

type ApiResponse<'T> = {
    success: bool
    data: 'T option
    errorMessage: string option
}

type Log = string
type Report = string

let handleRunReportFormSubmit (event: Browser.Types.Event) =
    event.preventDefault()

    let containerName =
        (document.getElementById("containerName") :?> HTMLInputElement).value

    let data = 
        { containerName = containerName }

    let defaultProps =
        [ RequestProperties.Method HttpMethod.POST
        ; requestHeaders [ContentType "application/json"]
        ; RequestProperties.Body (unbox (JS.JSON.stringify data)) ]
    
    promise {
        let! res = fetch "/api/runReport" defaultProps
        let! txt = res.text()

        let outputDiv = document.getElementById("output1") :?> HTMLDivElement
        outputDiv.classList.add("output__body-bordered")

        if not (txt = "") then
            outputDiv.textContent <- "Report generated successfully"
        else
            outputDiv.textContent <- "Error"
    }
    |> Promise.catch (fun error ->
        console.error("Error:", error)
    )
    |> ignore

let handleCurlRequestFormSubmit (event: Browser.Types.Event) =
    event.preventDefault()

    let curlRequest =
        (document.getElementById("curlRequest") :?> HTMLInputElement).value

    let data = 
        { curlRequest = curlRequest }

    let defaultProps =
        [ RequestProperties.Method HttpMethod.POST
        ; requestHeaders [ContentType "application/json"]
        ; RequestProperties.Body (unbox (JS.JSON.stringify data)) ]
    
    promise {
        let! res = fetch "/api/sendCurlRequest" defaultProps
        let! txt = res.text()

        let outputDiv = document.getElementById("output2") :?> HTMLDivElement
        outputDiv.classList.add("output__body-bordered")

        if not (txt = "") then
            outputDiv.textContent <- "Request successful. Logs collected and report generated"
        else
            outputDiv.textContent <- "Error"
    }
    |> Promise.catch (fun error ->
        console.error("Error:", error)
    )
    |> ignore

let fetchLogContent logName =
    promise {
        let! res = fetch $"/api/logs/{logName}" []
        let! json = res.json<ApiResponse<string>>()

        match json.success, json.data with
        | true, Some logContent ->
            let logContentDiv = document.getElementById("log-content") :?> HTMLDivElement
            logContentDiv.innerHTML <- $"""
                <h2 class="logs__header">Log: {logName}</h2>
                <div class="logs__content-wrapper">
                    <pre>{logContent}</pre>
                </div>
            """
        | _ ->
            console.error("Error fetching log content:", json.errorMessage)
    }
    |> Promise.catch (fun error ->
        console.error("Error:", error)
    )
    |> ignore

let setupLogLinks () =
    let logLinks = document.querySelectorAll(".log-link")
    let logLinksList =
        [for i in 0 .. logLinks.length - 1 do
            yield logLinks.item(i) :?> HTMLElement]
    logLinksList
    |> List.iter (fun link ->
        link.addEventListener("click", fun (event: Event) ->
            event.preventDefault()
            let logName = link.getAttribute("data-log")
            match logName with
            | null -> ()
            | _ -> fetchLogContent logName
        )
    )

let fetchLogs () =
    promise {
        let! res = fetch "/api/logs" []
        let! json = res.json<ApiResponse<Log list>>()

        match json.success, json.data with
        | true, Some logs when List.isEmpty logs ->
            let logsDiv = document.getElementById("logs") :?> HTMLDivElement
            logsDiv.innerHTML <- "<p>No logs available</p>"
        | true, Some logs ->
            let logsDiv = document.getElementById("logs") :?> HTMLDivElement
            logsDiv.innerHTML <- $"""
                <h2 class="logs__header">Available logs</h2>
                <div class="logs__with-logs-wrapper">
                    <ul class="logs__list">
                        {logs
                         |> List.map (fun log ->
                             sprintf "<li class=\"logs__list-item\"><a href=\"#\" class=\"log-link logs__list-link\" data-log=\"%s\">%s</a></li>" log log)
                         |> String.concat ""}
                    </ul>
                </div>
            """
            setupLogLinks ()
        | _ ->
            console.error("Error fetching logs:", json.errorMessage)
    }
    |> Promise.catch (fun error ->
        console.error("Error:", error)
    )
    |> ignore

let fetchReportContent reportName =
    promise {
        let! res = fetch $"/api/reports/{reportName}" []
        let! json = res.json<ApiResponse<string>>()

        match json.success, json.data with
        | true, Some reportContent ->
            let reportContentDiv = document.getElementById("report-content") :?> HTMLDivElement
            reportContentDiv.innerHTML <- $"""
                <h2 class="logs__header">Report: {reportName}</h2>
                <div class="logs__content-wrapper">
                    <pre>{reportContent}</pre>
                </div>
            """
        | _ ->
            console.error("Error fetching report content:", json.errorMessage)
    }
    |> Promise.catch (fun error ->
        console.error("Error:", error)
    )
    |> ignore

let setupReportLinks () =
    let reportLinks = document.querySelectorAll(".report-link")
    let reportLinksList =
        [for i in 0 .. reportLinks.length - 1 do
            yield reportLinks.item(i) :?> HTMLElement]
    reportLinksList
    |> List.iter (fun link ->
        link.addEventListener("click", fun (event: Event) ->
            event.preventDefault()
            let reportName = link.getAttribute("data-report")
            match reportName with
            | null -> ()
            | _ -> fetchReportContent reportName
        )
    )
    
let fetchReports () =
    promise {
        let! res = fetch "/api/reports" []
        let! json = res.json<ApiResponse<Report list>>()

        match json.success, json.data with
        | true, Some reports when List.isEmpty reports ->
            let reportsDiv = document.getElementById("reports") :?> HTMLDivElement
            reportsDiv.innerHTML <- "<p>No reports available</p>"
        | true, Some reports ->
            let reportsDiv = document.getElementById("reports") :?> HTMLDivElement
            reportsDiv.innerHTML <- $"""
                <h2 class="logs__header">Available Reports</h2>
                <div class="logs__with-logs-wrapper">
                    <ul class="logs__list">
                        {reports
                         |> List.map (fun report ->
                             sprintf "<li class=\"logs__list-item\"><a href=\"#\" class=\"report-link logs__list-link\" data-report=\"%s\">%s</a></li>" report report)
                         |> String.concat ""}
                    </ul>
                </div>
            """
            setupReportLinks ()
        | _ ->
            console.error("Error fetching reports:", json.errorMessage)
    }
    |> Promise.catch (fun error ->
        console.error("Error:", error)
    )
    |> ignore
let setupEventListeners () =
    document.addEventListener("DOMContentLoaded", fun _ ->
        fetchLogs ()
        fetchReports ()
        document
            .getElementById("runReportForm")
            .addEventListener("submit", handleRunReportFormSubmit)
        document
            .getElementById("curlRequestForm")
            .addEventListener("submit", handleCurlRequestFormSubmit)
    )

setupEventListeners ()
