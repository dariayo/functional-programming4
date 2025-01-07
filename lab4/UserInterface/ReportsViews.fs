module ReportsView

open Giraffe.ViewEngine

let indexPage =
    html [] [
        head [] [
            title [] [ str "Reports" ]
        ]
        body [] [
            h1 [] [ str "Reports" ]
            div [ _id "reports" ] []
            script [ _src "/static/js/scripts.js" ] []
        ]
    ]
