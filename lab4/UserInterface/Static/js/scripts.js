document.addEventListener('DOMContentLoaded', function () {

    fetch('/api/logs')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const logsDiv = document.getElementById('logs');
                logsDiv.innerHTML = `
                    <h2>Available Logs</h2>
                    <ul>
                        ${data.data.map(log => `<li><a href="#" class="log-link" data-log="${log}">${log}</a></li>`).join('')}
                    </ul>`;

                const logLinks = document.querySelectorAll('.log-link');
                logLinks.forEach(link => {
                    link.addEventListener('click', function (event) {
                        event.preventDefault();
                        const logName = event.target.getAttribute('data-log');
                        fetchLogContent(logName);
                    });
                });
            } else {
                console.error('Error fetching logs:', data.errorMessage);
            }
        });

    function fetchLogContent(logName) {
        fetch(`/api/logs/${logName}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    const logContentDiv = document.getElementById('log-content');
                    logContentDiv.innerHTML = `
                        <h2>Log: ${logName}</h2>
                        <pre>${data.data}</pre>
                    `;
                } else {
                    console.error('Error fetching log content:', data.errorMessage);
                }
            });
    }

    fetch('/api/reports')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const reportsDiv = document.getElementById('reports');
                reportsDiv.innerHTML = `
                    <h2>Available Reports</h2>
                    <ul>
                        ${data.data
                        .map(
                            report =>
                                `<li><a href="#" class="report-link" data-report="${report}">${report}</a></li>`
                        )
                        .join('')}
                    </ul>`;

                document.querySelectorAll('.report-link').forEach(link => {
                    link.addEventListener('click', function (event) {
                        event.preventDefault();
                        const reportName = this.getAttribute('data-report');
                        fetch(`/api/reports/${reportName}`)
                            .then(response => response.json())
                            .then(reportData => {
                                if (reportData.success) {
                                    const reportContentDiv = document.getElementById('report-content');
                                    reportContentDiv.innerHTML = `
                                        <h2>${reportName}</h2>
                                        <pre>${reportData.data}</pre>
                                    `;
                                } else {
                                    console.error('Error fetching report content:', reportData.errorMessage);
                                }
                            })
                            .catch(error => console.error('Fetch error:', error));
                    });
                });
            } else {
                console.error('Error fetching reports:', data.errorMessage);
            }
        })
        .catch(error => console.error('Fetch error:', error));
});
