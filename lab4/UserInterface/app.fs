module App

open Fable.Core.JsInterop
open Fable.Core
open Fable.Import.Browser

let fetchLogs () =
    promise {
        let! response = fetch "/api/logs" []
        let! json = response.json()
        let data = json |> unbox<{| success: bool; data: string[]; errorMessage: string |}>
        
        if data.success then
            let logsDiv = document.getElementById("logs")
            logsDiv.innerHTML <- $"""
                <h2 class="logs__header">Click on a log entry to view log content</h2>
                <div class="logs__with-logs-wrapper">
                    <ul class="logs__list">
                        {data.data
                        |> Array.map (fun log -> $"""
                            <li class="logs__list-item">
                                <a href="#" class="log-link logs__list-link" data-log="{log}">{log}</a>
                            </li>""")
                        |> String.concat ""}
                    </ul>
                </div>"""

            document.querySelectorAll(".log-link")
            |> Seq.cast<HTMLElement>
            |> Seq.iter (fun link ->
                link.addEventListener_click (fun event ->
                    event.preventDefault()
                    let logName = link.getAttribute("data-log")
                    fetchLogContent logName
                )
            )
        else
            console.error("Error fetching logs:", data.errorMessage)
    }

and fetchLogContent logName =
    promise {
        let! response = fetch ($"/api/logs/{logName}") []
        let! json = response.json()
        let data = json |> unbox<{| success: bool; data: string; errorMessage: string |}>
        
        if data.success then
            let logContentDiv = document.getElementById("log-content")
            logContentDiv.innerHTML <- $"""
                <h2 class="logs__header">Log: {logName}</h2>
                <div class="logs__content-wrapper">
                    <pre>{data.data}</pre>
                </div>"""
        else
            console.error("Error fetching log content:", data.errorMessage)
    }

// Initialize application on DOMContentLoaded
document.addEventListener("DOMContentLoaded", fun _ ->
    fetchLogs () |> ignore
)
