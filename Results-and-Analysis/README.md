# Results and Analysis

This directory contains experimental results and analysis logs for the S.P.E.E.D racing agents project. The following agent-specific log folders are included:

- **Enhanced Natural DRL Logs**: Results for the DRL agent with enhanced naturalistic behavior.
- **Heuristic Agent Logs**: Baseline waypoint-following agent logs.
- **Manual Player Logs**: Human/manual player runs.
- **Simple DRL Logs**: DRL agent with simple network/reward.

Each folder contains:
- CSV files of run summaries (lap time, speed, steering, and path deviation metrics)
- (If present) raw or detailed logs for each experiment run

## How to Use
- Browse each folder to access specific CSV experiment results
- For per-folder structure, see included CSVs (standard header: Metric,Value...)
- For comparison, open CSVs in Excel/Sheets or process by script

## Recommendations for Reproducibility
To further improve this directory and ensure analysis is reproducible:
- Add Python or Jupyter scripts that aggregate or plot the CSV results (e.g., 'scripts/summarize_results.py')
- Add reference plots (as .png) generated from the analysis scripts
- Document in each subfolder with a README.txt exactly how those logs were generated (agent config, track, date)

## Contribution
If adding new results:
- Use a clear subfolder structure
- Include a README.txt or metadata in any new experiment folder you add
- Provide scripts or detail for how to analyze or visualize your results.
