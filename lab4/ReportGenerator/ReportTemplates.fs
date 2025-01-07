module ReportGenerator.ReportTemplates

let getMarkdownTemplate () =
    """
# Log Report for {containerName}

**Generated on:** {date}

---

## Metrics Summary
- **Errors:** {errorCount}
- **Warnings:** {warningCount}
- **Info:** {infoCount}

---

## Logs
{logs}

---

*End of Report*
"""
