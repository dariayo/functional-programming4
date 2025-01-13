open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Fetch
open Browser.Types

type RunReportData = {
    containerName: string
}

// Определение записи для данных формы "Curl Request"
type CurlRequestData = {
    curlRequest: string
}

let handleRunReportFormSubmit (event: Browser.Types.Event) =
    event.preventDefault()

    // Получение значения поля ввода
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

        // Обработка ответа от сервера
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

    // Получение значения поля ввода
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

        // Обработка ответа от сервера
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

let setupEventListeners () =
    document
        .getElementById("runReportForm")
        .addEventListener("submit", handleRunReportFormSubmit)
    document
        .getElementById("curlRequestForm")
        .addEventListener("submit", handleCurlRequestFormSubmit)

// Инициализация
setupEventListeners ()
