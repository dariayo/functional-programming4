document.addEventListener("DOMContentLoaded", function () {
    fetch("/api/logs")
      .then((response) => response.json())
      .then((data) => {
        if (data.success) {
          const logsDiv = document.getElementById("logs");
          logsDiv.innerHTML = `
              <h2 class="logs__header">Available logs</h2>
              <div class="logs__with-logs-wrapper">
                  <ul class="logs__list">
                      ${data.data
                        .map(
                          (log) => `
                          <li class="logs__list-item">
                              <a href="#" class="log-link logs__list-link" data-log="${log}">${log}</a>
                          </li>
                      `
                        )
                        .join("")}
                  </ul>
              </div>`;
  
          const logLinks = document.querySelectorAll(".log-link");
          logLinks.forEach((link) => {
            link.addEventListener("click", function (event) {
              event.preventDefault();
              const logName = event.target.getAttribute("data-log");
              fetchLogContent(logName);
            });
          });
        } else {
          console.error("Error fetching logs:", data.errorMessage);
        }
      });
  
    function fetchLogContent(logName) {
      fetch(`/api/logs/${logName}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            const logContentDiv = document.getElementById("log-content");
            logContentDiv.innerHTML = `
                          <h2 class="logs__header">Log: ${logName}</h2>
                          <div class="logs__content-wrapper">
                              <pre>${data.data}</pre>
                          </div>
                      `;
          } else {
            console.error("Error fetching log content:", data.errorMessage);
          }
        });
    }
  
    fetch("/api/reports")
      .then((response) => response.json())
      .then((data) => {
        if (data.success) {
          const reportsDiv = document.getElementById("reports");
          reportsDiv.innerHTML = `
                      <h2 class="logs__header">Available Reports</h2>
                      <div class="logs__with-logs-wrapper">
                          <ul class="logs__list">
                              ${data.data
                                .map(
                                  (report) =>
                                    `<li class="logs__list-item">
                                      <a href="#" class="report-link logs__list-link" data-report="${report}">${report}</a>
                                  </li>`
                                )
                                .join("")}
                          </ul>
                      </div>`;
  
          document.querySelectorAll(".report-link").forEach((link) => {
            link.addEventListener("click", function (event) {
              event.preventDefault();
              const reportName = this.getAttribute("data-report");
              fetch(`/api/reports/${reportName}`)
                .then((response) => response.json())
                .then((reportData) => {
                  if (reportData.success) {
                    const reportContentDiv =
                      document.getElementById("report-content");
                    reportContentDiv.innerHTML = `
                                          <h2 class="logs__header">${reportName}</h2>
                                          <div class="logs__content-wrapper">
                                              <pre>${reportData.data}</pre>
                                          </div>
                                      `;
                  } else {
                    console.error(
                      "Error fetching report content:",
                      reportData.errorMessage
                    );
                  }
                })
                .catch((error) => console.error("Fetch error:", error));
            });
          });
        } else {
          console.error("Error fetching reports:", data.errorMessage);
        }
      })
      .catch((error) => console.error("Fetch error:", error));
  });
  