module LogsView

open Giraffe.ViewEngine

let indexPage =
    html [] [
        head [] [
            title [] [ str "Logs" ]
        ]
        body [] [
            h1 [] [ str "Logs" ]
            div [ _id "logs" ] []
            script [ _src "/static/js/scripts.js" ] []
        ]
    ]
