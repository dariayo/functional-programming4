import { Record } from "./fable_modules/fable-library-js.4.24.0/Types.js";
import { option_type, bool_type, record_type, string_type } from "./fable_modules/fable-library-js.4.24.0/Reflection.js";
import { fetch$, Types_RequestProperties } from "./fable_modules/Fable.Fetch.2.7.0/Fetch.fs.js";
import { isEmpty, map as map_1, iterate, empty, ofArray } from "./fable_modules/fable-library-js.4.24.0/List.js";
import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "./fable_modules/Fable.Promise.3.2.0/Promise.fs.js";
import { promise } from "./fable_modules/Fable.Promise.3.2.0/PromiseImpl.fs.js";
import { some } from "./fable_modules/fable-library-js.4.24.0/Option.js";
import { map, delay, toList } from "./fable_modules/fable-library-js.4.24.0/Seq.js";
import { rangeDouble } from "./fable_modules/fable-library-js.4.24.0/Range.js";
import { defaultOf } from "./fable_modules/fable-library-js.4.24.0/Util.js";
import { printf, toText, join } from "./fable_modules/fable-library-js.4.24.0/String.js";

export class RunReportData extends Record {
    constructor(containerName) {
        super();
        this.containerName = containerName;
    }
}

export function RunReportData_$reflection() {
    return record_type("Program.RunReportData", [], RunReportData, () => [["containerName", string_type]]);
}

export class CurlRequestData extends Record {
    constructor(curlRequest) {
        super();
        this.curlRequest = curlRequest;
    }
}

export function CurlRequestData_$reflection() {
    return record_type("Program.CurlRequestData", [], CurlRequestData, () => [["curlRequest", string_type]]);
}

export class ApiResponse$1 extends Record {
    constructor(success, data, errorMessage) {
        super();
        this.success = success;
        this.data = data;
        this.errorMessage = errorMessage;
    }
}

export function ApiResponse$1_$reflection(gen0) {
    return record_type("Program.ApiResponse`1", [gen0], ApiResponse$1, () => [["success", bool_type], ["data", option_type(gen0)], ["errorMessage", option_type(string_type)]]);
}

export function handleRunReportFormSubmit(event) {
    let pr;
    event.preventDefault();
    const containerName = document.getElementById("containerName").value;
    const data = new RunReportData(containerName);
    const defaultProps = ofArray([new Types_RequestProperties(0, ["POST"]), new Types_RequestProperties(1, [{
        "Content-Type": "application/json",
    }]), new Types_RequestProperties(2, [JSON.stringify(data)])]);
    (pr = PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fetch$("/api/runReport", defaultProps).then((_arg) => {
        const res = _arg;
        return res.text().then((_arg_1) => {
            const txt = _arg_1;
            const outputDiv = document.getElementById("output1");
            outputDiv.classList.add("output__body-bordered");
            if (!(txt === "")) {
                outputDiv.textContent = "Report generated successfully";
                return Promise.resolve();
            }
            else {
                outputDiv.textContent = "Error";
                return Promise.resolve();
            }
        });
    })))), pr.catch((error) => {
        console.error(some("Error:"), error);
    }));
}

export function handleCurlRequestFormSubmit(event) {
    let pr;
    event.preventDefault();
    const curlRequest = document.getElementById("curlRequest").value;
    const data = new CurlRequestData(curlRequest);
    const defaultProps = ofArray([new Types_RequestProperties(0, ["POST"]), new Types_RequestProperties(1, [{
        "Content-Type": "application/json",
    }]), new Types_RequestProperties(2, [JSON.stringify(data)])]);
    (pr = PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fetch$("/api/sendCurlRequest", defaultProps).then((_arg) => {
        const res = _arg;
        return res.text().then((_arg_1) => {
            const txt = _arg_1;
            const outputDiv = document.getElementById("output2");
            outputDiv.classList.add("output__body-bordered");
            if (!(txt === "")) {
                outputDiv.textContent = "Request successful. Logs collected and report generated";
                return Promise.resolve();
            }
            else {
                outputDiv.textContent = "Error";
                return Promise.resolve();
            }
        });
    })))), pr.catch((error) => {
        console.error(some("Error:"), error);
    }));
}

export function fetchLogContent(logName) {
    let pr;
    (pr = PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fetch$(`/api/logs/${logName}`, empty()).then((_arg) => {
        const res = _arg;
        return res.json().then((_arg_1) => {
            const json = _arg_1;
            const matchValue_1 = json.data;
            let matchResult;
            if (json.success) {
                if (matchValue_1 != null) {
                    matchResult = 0;
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
            switch (matchResult) {
                case 0: {
                    const logContent = matchValue_1;
                    const logContentDiv = document.getElementById("log-content");
                    logContentDiv.innerHTML = (`
                <h2 class="logs__header">Log: ${logName}</h2>
                <div class="logs__content-wrapper">
                    <pre>${logContent}</pre>
                </div>
            `);
                    return Promise.resolve();
                }
                default: {
                    console.error(some("Error fetching log content:"), json.errorMessage);
                    return Promise.resolve();
                }
            }
        });
    })))), pr.catch((error) => {
        console.error(some("Error:"), error);
    }));
}

export function setupLogLinks() {
    const logLinks = document.querySelectorAll(".log-link");
    const logLinksList = toList(delay(() => map((i) => logLinks.item(i), rangeDouble(0, 1, logLinks.length - 1))));
    iterate((link) => {
        link.addEventListener("click", (event) => {
            event.preventDefault();
            const logName = link.getAttribute("data-log");
            if (logName === defaultOf()) {
            }
            else {
                fetchLogContent(logName);
            }
        });
    }, logLinksList);
}

export function fetchLogs() {
    let pr;
    (pr = PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fetch$("/api/logs", empty()).then((_arg) => {
        const res = _arg;
        return res.json().then((_arg_1) => {
            let logs;
            const json = _arg_1;
            const matchValue_1 = json.data;
            let matchResult, logs_1, logs_2;
            if (json.success) {
                if (matchValue_1 != null) {
                    if ((logs = matchValue_1, isEmpty(logs))) {
                        matchResult = 0;
                        logs_1 = matchValue_1;
                    }
                    else {
                        matchResult = 1;
                        logs_2 = matchValue_1;
                    }
                }
                else {
                    matchResult = 2;
                }
            }
            else {
                matchResult = 2;
            }
            switch (matchResult) {
                case 0: {
                    const logsDiv = document.getElementById("logs");
                    logsDiv.innerHTML = "<p>No logs available</p>";
                    return Promise.resolve();
                }
                case 1: {
                    const logsDiv_1 = document.getElementById("logs");
                    logsDiv_1.innerHTML = (`
                <h2 class="logs__header">Available logs</h2>
                <div class="logs__with-logs-wrapper">
                    <ul class="logs__list">
                        ${join("", map_1((log) => toText(printf("<li class=\"logs__list-item\"><a href=\"#\" class=\"log-link logs__list-link\" data-log=\"%s\">%s</a></li>"))(log)(log), logs_2))}
                    </ul>
                </div>
            `);
                    setupLogLinks();
                    return Promise.resolve();
                }
                default: {
                    console.error(some("Error fetching logs:"), json.errorMessage);
                    return Promise.resolve();
                }
            }
        });
    })))), pr.catch((error) => {
        console.error(some("Error:"), error);
    }));
}

export function fetchReportContent(reportName) {
    let pr;
    (pr = PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fetch$(`/api/reports/${reportName}`, empty()).then((_arg) => {
        const res = _arg;
        return res.json().then((_arg_1) => {
            const json = _arg_1;
            const matchValue_1 = json.data;
            let matchResult;
            if (json.success) {
                if (matchValue_1 != null) {
                    matchResult = 0;
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
            switch (matchResult) {
                case 0: {
                    const reportContent = matchValue_1;
                    const reportContentDiv = document.getElementById("report-content");
                    reportContentDiv.innerHTML = (`
                <h2 class="logs__header">Report: ${reportName}</h2>
                <div class="logs__content-wrapper">
                    <pre>${reportContent}</pre>
                </div>
            `);
                    return Promise.resolve();
                }
                default: {
                    console.error(some("Error fetching report content:"), json.errorMessage);
                    return Promise.resolve();
                }
            }
        });
    })))), pr.catch((error) => {
        console.error(some("Error:"), error);
    }));
}

export function setupReportLinks() {
    const reportLinks = document.querySelectorAll(".report-link");
    const reportLinksList = toList(delay(() => map((i) => reportLinks.item(i), rangeDouble(0, 1, reportLinks.length - 1))));
    iterate((link) => {
        link.addEventListener("click", (event) => {
            event.preventDefault();
            const reportName = link.getAttribute("data-report");
            if (reportName === defaultOf()) {
            }
            else {
                fetchReportContent(reportName);
            }
        });
    }, reportLinksList);
}

export function fetchReports() {
    let pr;
    (pr = PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fetch$("/api/reports", empty()).then((_arg) => {
        const res = _arg;
        return res.json().then((_arg_1) => {
            let reports;
            const json = _arg_1;
            const matchValue_1 = json.data;
            let matchResult, reports_1, reports_2;
            if (json.success) {
                if (matchValue_1 != null) {
                    if ((reports = matchValue_1, isEmpty(reports))) {
                        matchResult = 0;
                        reports_1 = matchValue_1;
                    }
                    else {
                        matchResult = 1;
                        reports_2 = matchValue_1;
                    }
                }
                else {
                    matchResult = 2;
                }
            }
            else {
                matchResult = 2;
            }
            switch (matchResult) {
                case 0: {
                    const reportsDiv = document.getElementById("reports");
                    reportsDiv.innerHTML = "<p>No reports available</p>";
                    return Promise.resolve();
                }
                case 1: {
                    const reportsDiv_1 = document.getElementById("reports");
                    reportsDiv_1.innerHTML = (`
                <h2 class="logs__header">Available Reports</h2>
                <div class="logs__with-logs-wrapper">
                    <ul class="logs__list">
                        ${join("", map_1((report) => toText(printf("<li class=\"logs__list-item\"><a href=\"#\" class=\"report-link logs__list-link\" data-report=\"%s\">%s</a></li>"))(report)(report), reports_2))}
                    </ul>
                </div>
            `);
                    setupReportLinks();
                    return Promise.resolve();
                }
                default: {
                    console.error(some("Error fetching reports:"), json.errorMessage);
                    return Promise.resolve();
                }
            }
        });
    })))), pr.catch((error) => {
        console.error(some("Error:"), error);
    }));
}

export function setupEventListeners() {
    document.addEventListener("DOMContentLoaded", (_arg) => {
        fetchLogs();
        fetchReports();
        document.getElementById("runReportForm").addEventListener("submit", (event) => {
            handleRunReportFormSubmit(event);
        });
        document.getElementById("curlRequestForm").addEventListener("submit", (event_1) => {
            handleCurlRequestFormSubmit(event_1);
        });
    });
}

setupEventListeners();

