import { Record } from "./fable_modules/fable-library-js.4.24.0/Types.js";
import { record_type, string_type } from "./fable_modules/fable-library-js.4.24.0/Reflection.js";
import { fetch$, Types_RequestProperties } from "./fable_modules/Fable.Fetch.2.7.0/Fetch.fs.js";
import { ofArray } from "./fable_modules/fable-library-js.4.24.0/List.js";
import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "./fable_modules/Fable.Promise.3.2.0/Promise.fs.js";
import { promise } from "./fable_modules/Fable.Promise.3.2.0/PromiseImpl.fs.js";
import { some } from "./fable_modules/fable-library-js.4.24.0/Option.js";

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

export function setupEventListeners() {
    document.getElementById("runReportForm").addEventListener("submit", (event) => {
        handleRunReportFormSubmit(event);
    });
    document.getElementById("curlRequestForm").addEventListener("submit", (event_1) => {
        handleCurlRequestFormSubmit(event_1);
    });
}

setupEventListeners();

