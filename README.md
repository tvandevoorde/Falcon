# Falcon
Full-stack Alerting &amp; Log Control Observability Network

## Requirements

### Must-Have

Core features required for daily operational monitoring.

#### Server & Service Health

- Dashboard showing overall server health (CPU, memory, disk usage, uptime).
- Automatic discovery or configuration of monitored servers.
- Real-time or near-real-time status of Windows Services, including:
  - Running/stopped state
  - Restart attempts or failure alerts
  - Last start time

#### Scheduled Tasks Monitoring

- List of all scheduled tasks per server.
- Last run result, timestamp, and next scheduled run.
- Alerts for failed runs, missed runs, or disabled tasks.

#### IIS Monitoring

- Status of Application Pools:
  - Started/stopped state
  - Crash or rapid-fail detection
- Status of Websites & Web Applications:
  - Ping/health check endpoint
  - HTTP status code monitoring
  - SSL certificate validity & expiration

#### Log File Monitoring

- Configurable log file locations per server.
- Automated scanning for defined error patterns (e.g. ERROR, EXCEPTION, CRITICAL).
- Daily summary of new errors.
- Alerts when new critical errors appear.

#### Notifications & Alerts

- Email alerts for:
  - Failed scheduled tasks
  - Service crashes
  - IIS application pool stops
  - New critical log errors
- Severity-based notifications (info/warning/critical).

#### Security & Access

- Authentication.
- Role-based access (operator, viewer, admin).

### Should-Have

Important features that improve efficiency but aren't absolute essentials.

#### Automation & Recovery

- Self-service restart actions for:
  - Windows services
  - IIS application pools
- Ability to trigger scheduled tasks manually.

#### Historical Data & Reporting

- Trend graphs for CPU, memory, disk, and service uptime.
- History of scheduled task results.
- Exportable reports (PDF/Excel) for audits.

#### Custom Health Checks

- Custom scripts or endpoints for application-specific checks.
- Rules engine to define alerts (e.g. “warn if service restarts more than 3 times in 1 hour”).

#### Mobile-Friendly UI

- Works well on a smartphone (but not necessarily a native app).

### Could-Have

Nice-to-have features that add convenience or long-term value.

#### AI / Anomaly Detection

- Detect unusual patterns in logs or CPU/memory usage.
- Predict upcoming failures based on trends.

#### ChatOps Integration

- Teams notifications.
- Slash commands to get status or restart services.

#### Remote Desktop / SSH Launcher

- Quick shortcuts to open RDP to the selected server.

#### Custom Dashboards

- Personalized views by role (developers, testers, admins).

#### Maintenance Scheduling

- Show planned downtime and mute alerts during maintenance windows.

### Won’t-Have (for now)

Features explicitly not in scope or not needed.

- Full-fledged configuration management (like Puppet/Chef/Ansible).
- Deployment automation tools.
- Replacing existing monitoring solutions platform-wide (e.g., not meant to replace SCOM/Zabbix/Nagios if already used).
- Native mobile applications.
- AI-based automatic remediation actions that modify servers autonomously without human approval.

## High-Level Architecture Diagram

``` txt
                    ┌───────────────────────────────┐
                    │          Web UI (Angular)      │
                    │  Dashboards, Logs, Health, UX  │
                    └───────────────┬───────────────┘
                                    │ REST API
                                    ▼
                     ┌────────────────────────────────┐
                     │       Backend API (.NET)       │
                     │ Authentication                 │
                     │ Authorization (RBAC)           │
                     │ Alerting & Notification Engine │
                     │ Orchestration of collectors    │
                     └──────────────────┬─────────────┘
                                        │
                                Message Bus / Jobs
                                        │
                                        ▼
          ┌─────────────────────────────────────────────────────────┐
          │                 Monitoring Collectors                   │
          │                                                         │
          │  • Windows Service Collector                            │
          │  • Scheduled Tasks Collector                            │
          │  • IIS Collector (App Pools + Sites)                    │
          │  • Log File Collector (pattern-based)                   │
          │  • Custom Health Checks Runner                          │
          │                                                         │
          │  Runs via agents or remote PowerShell/WinRM             │
          └───────────────────────┬─────────────────────────────────┘
                                  │
                                  ▼
                     ┌──────────────────────────────┐
                     │   Data Storage (SQL Server)   │
                     │ Health States, Logs, Metrics  │
                     │ Historical Data, Audit Logs   │
                     └──────────────────────────────┘
```

## Key Architectural Components

### 1. Web UI (Angular or Blazor)

- Real-time dashboards (SignalR optional)
- Responsive layouts
- Role-based visibility

### 2. API Layer (.NET 10)

- Modular endpoints: Services, Tasks, IIS, Logs
- Authentication via Azure AD / AD FS / LDAP
- Alerting engine (rules, thresholds, notifications)
- Command endpoints for restarting services or triggering tasks

### 3. Monitoring Collectors

- Agentless (remote PowerShell & WMI/WinRM)

### 4. Storage

- SQL Server
- Tables for:
  - Server inventory
  - Health checks
  - Logs
  - Historical metrics
  - Alerts & notifications

### 5. Notification System

- SMTP for email
- Optional Teams webhooks
- Severity levels (Info / Warning / Critical)

## Local Development

1. Install the .NET 10 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/). Verify with `dotnet --list-sdks`.
2. Restore dependencies: `dotnet restore Falcon.sln`
3. Run the full solution build: `dotnet build Falcon.sln`
4. Execute automated tests: `dotnet test Falcon.sln`
5. Launch the API locally:
  - `dotnet run --project src/Falcon.Api/Falcon.Api.csproj`
  - API base address defaults to `https://localhost:5001`.
6. Explore Swagger UI at `https://localhost:5001/swagger` for interactive contract review.

### Configuration

- `appsettings.Development.json` contains local overrides (connection strings, logging). Provide secrets via user secrets or environment variables rather than committing them.
- JWT authentication uses placeholders; integrate with your identity provider by replacing the `JwtBearer` configuration in `Program.cs`.
- Health checks are exposed under `/healthz`; secure the endpoint when deploying to shared environments.

## Testing Strategy

- Unit tests live in the `tests` directory and use xUnit with FluentAssertions.
- Favor Test-Driven Development for new application handlers and domain behaviors; keep tests self-contained and deterministic.
- For integration coverage, introduce a dedicated project (e.g., `Falcon.Api.Tests.Integration`) and host the API with `WebApplicationFactory`.
- Capture regression scenarios for maintenance-window muting, alert state transitions, and collector heartbeat handling.

## Continuous Delivery Guidance

- The repository includes a GitHub Actions workflow at `.github/workflows/ci.yml` that executes on pushes and pull requests targeting `main`, `develop`, and `features/*` branches.
- The workflow runs a two-OS matrix build (`ubuntu-latest`, `windows-latest`), restores packages, builds in `Release`, and runs the test suite while publishing TRX artifacts.
- A security baseline job performs `dotnet list package --vulnerable --include-transitive`; extend it with SAST or container scanning as the platform matures.
- The deployment job is a placeholder. Replace the echo script with platform-specific steps (Azure Web App, Azure Container Apps, or on-prem) and protect the environment with approvals.
- For faster feedback loops (DORA metrics), keep pull requests small, embrace feature flags, and aim for pipeline durations under 10 minutes.

## Database Schema

# 1. Overview

FALCON monitors Windows servers, services, scheduled tasks, IIS app pools/websites, logfiles and emits alerts. The schema and API are designed for flexibility (agents or agentless collectors), auditability, and performant queries for dashboards.

---

# 2. ER Overview (conceptual)

Entities (brief):

* **server**: inventory of machines
* **collector**: agent or remote runner configuration
* **service**: Windows services to monitor
* **scheduled_task** & **task_run**: scheduled tasks and history
* **iis_site** & **app_pool**: IIS items
* **log_file** & **log_entry**: log metadata and parsed entries
* **log_pattern**: rules to scan for
* **metric**: time-series metrics (cpu/mem/disk) stored in a metrics table
* **alert**: generated alerts
* **notification**: notification attempts
* **user / role / role_assignment**: auth and RBAC
* **action**: recorded operator actions (restart, trigger task)

Relationships: servers own services, tasks, iis items, logs, metrics. Alerts reference server+entity. Collectors push metrics/logs.

---

# 3. Database Schema (DDL)

Below are recommended tables with essential columns and constraints. This is written for SQL Server / PostgreSQL compatible DDL; adapt types as needed.

```sql
-- 1. Server inventory
CREATE TABLE "server" (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  hostname VARCHAR(255) NOT NULL,
  display_name VARCHAR(255),
  environment VARCHAR(50) NOT NULL, -- dev/test/prod
  ip_address VARCHAR(45),
  os VARCHAR(255),
  last_heartbeat TIMESTAMP WITH TIME ZONE,
  is_managed BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);
CREATE INDEX ix_server_env ON "server" (environment);
CREATE INDEX ix_server_hostname ON "server" (hostname);

-- 2. Collector (agent or runner)
CREATE TABLE collector (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(200) NOT NULL,
  type VARCHAR(50) NOT NULL, -- agent | winrm | powershell | hybrid
  config JSONB NULL,
  last_seen TIMESTAMP WITH TIME ZONE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 3. Service monitored
CREATE TABLE service (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  server_id UUID REFERENCES "server"(id) ON DELETE CASCADE,
  service_name VARCHAR(255) NOT NULL,
  display_name VARCHAR(255),
  desired_state VARCHAR(20) DEFAULT 'running', -- running/stopped
  critical BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);
CREATE INDEX ix_service_server ON service(server_id);

-- 4. Service events (history)
CREATE TABLE service_event (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  service_id UUID REFERENCES service(id) ON DELETE CASCADE,
  state VARCHAR(20) NOT NULL,
  message TEXT,
  event_time TIMESTAMP WITH TIME ZONE DEFAULT now()
);
CREATE INDEX ix_service_event_time ON service_event(service_id, event_time DESC);

-- 5. Scheduled tasks
CREATE TABLE scheduled_task (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  server_id UUID REFERENCES "server"(id) ON DELETE CASCADE,
  task_name VARCHAR(255) NOT NULL,
  schedule_desc TEXT, -- human readable schedule
  is_enabled BOOLEAN DEFAULT TRUE,
  last_run_time TIMESTAMP WITH TIME ZONE,
  last_run_result VARCHAR(50),
  next_run_time TIMESTAMP WITH TIME ZONE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- scheduled task runs history
CREATE TABLE task_run (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  scheduled_task_id UUID REFERENCES scheduled_task(id) ON DELETE CASCADE,
  start_time TIMESTAMP WITH TIME ZONE,
  end_time TIMESTAMP WITH TIME ZONE,
  result VARCHAR(50), -- success/failure/timeout
  exit_code INT NULL,
  output TEXT NULL
);
CREATE INDEX ix_task_run_task ON task_run(scheduled_task_id, start_time DESC);

-- 6. IIS items
CREATE TABLE app_pool (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  server_id UUID REFERENCES "server"(id) ON DELETE CASCADE,
  name VARCHAR(255) NOT NULL,
  state VARCHAR(50), -- Started/Stopped
  last_recycle TIMESTAMP WITH TIME ZONE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

CREATE TABLE iis_site (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  server_id UUID REFERENCES "server"(id) ON DELETE CASCADE,
  name VARCHAR(255) NOT NULL,
  bindings JSONB NULL,
  status VARCHAR(50), -- Started/Stopped
  ping_endpoint VARCHAR(1024),
  last_http_status INT,
  last_checked TIMESTAMP WITH TIME ZONE
);

-- 7. Log files and parsed entries
CREATE TABLE log_file (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  server_id UUID REFERENCES "server"(id) ON DELETE CASCADE,
  path TEXT NOT NULL,
  parser VARCHAR(100), -- optional
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

CREATE TABLE log_entry (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  log_file_id UUID REFERENCES log_file(id) ON DELETE CASCADE,
  timestamp TIMESTAMP WITH TIME ZONE,
  severity VARCHAR(50),
  message TEXT,
  json_payload JSONB NULL,
  inserted_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);
CREATE INDEX ix_log_entry_time ON log_entry(log_file_id, timestamp DESC);
CREATE INDEX ix_log_entry_severity ON log_entry(severity);

-- 8. Log patterns (rules)
CREATE TABLE log_pattern (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(200),
  pattern TEXT NOT NULL,
  severity_default VARCHAR(20) DEFAULT 'warning',
  enabled BOOLEAN DEFAULT TRUE
);

-- 9. Alerts
CREATE TABLE alert (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  server_id UUID REFERENCES "server"(id) NULL,
  source_type VARCHAR(50) NOT NULL, -- service | task | iis | log | metric
  source_id UUID NULL,
  alert_type VARCHAR(100) NOT NULL,
  severity VARCHAR(20) NOT NULL,
  status VARCHAR(20) DEFAULT 'open', -- open/acknowledged/closed
  message TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  resolved_at TIMESTAMP WITH TIME ZONE NULL
);
CREATE INDEX ix_alert_server ON alert(server_id);
CREATE INDEX ix_alert_status ON alert(status);

-- 10. Notifications
CREATE TABLE notification (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  alert_id UUID REFERENCES alert(id) ON DELETE CASCADE,
  channel VARCHAR(50), -- email / teams / slack
  recipient TEXT,
  status VARCHAR(20), -- pending/sent/failed
  attempt_count INT DEFAULT 0,
  last_attempt TIMESTAMP WITH TIME ZONE,
  payload JSONB NULL
);

-- 11. Users / Roles / RBAC
CREATE TABLE "role" (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(100) UNIQUE NOT NULL,
  description TEXT
);
CREATE TABLE "app_user" (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  username VARCHAR(200) UNIQUE NOT NULL,
  display_name VARCHAR(255),
  email VARCHAR(255),
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);
CREATE TABLE role_assignment (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES app_user(id) ON DELETE CASCADE,
  role_id UUID REFERENCES "role"(id) ON DELETE CASCADE,
  scope JSONB NULL -- optional server or resource scope
);

-- 12. Metrics (simple time series bucket)
CREATE TABLE metric_point (
  id BIGSERIAL PRIMARY KEY,
  server_id UUID REFERENCES "server"(id) ON DELETE CASCADE,
  metric_name VARCHAR(100) NOT NULL, -- cpu.user, mem.used, disk.free
  metric_value DOUBLE PRECISION NOT NULL,
  measured_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);
CREATE INDEX ix_metric_server_time ON metric_point(server_id, metric_name, measured_at DESC);

-- 13. Operator actions (audit)
CREATE TABLE operator_action (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES app_user(id),
  server_id UUID REFERENCES "server"(id),
  action_type VARCHAR(100), -- restart_service, trigger_task
  target_id UUID NULL,
  payload JSONB NULL,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 14. Maintenance windows
CREATE TABLE maintenance_window (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(255),
  start_time TIMESTAMP WITH TIME ZONE,
  end_time TIMESTAMP WITH TIME ZONE,
  servers JSONB NULL,
  muted BOOLEAN DEFAULT TRUE
);
```

### Notes and retention

* Consider separate time-series store (InfluxDB/Prometheus) if metrics volume increases. The `metric_point` table suffices for MVP/time-bounded retention.
* Add partitioning / TTL or scheduled cleanup jobs for `log_entry`, `metric_point` and `task_run` to manage storage.

---
