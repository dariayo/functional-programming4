module ReportGenerator.ReportTemplates

let getEnhancedMarkdownTemplate () =
    """
# Log Report for {containerName}

**Generated on:** {date}

---

## Metrics Summary
- **Errors:** {errorCount}
- **Warnings:** {warningCount}
- **Info:** {infoCount}

---

## Performance Metrics
- {performanceMetrics}

---

## Key Actions
- {keyActions}

---

## Potential Improvements or Anomalies
- {potentialImprovements}

---

## Active Threads
- {activeThreads}

---

## Request Types
- {requestTypes}

---

## Logs
{logs}

---

*End of Report*
"""
