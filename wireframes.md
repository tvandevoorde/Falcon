# Falcon Monitoring Platform — Wireframes

Below are low‑fidelity wireframes for the main screens of Falcon. These are conceptual sketches in text/ASCII form, suitable for UX iteration.

---

## **1. Dashboard (Home)**

```
+---------------------------------------------------------------+
| FALCON • Dashboard                                            |
+---------------------------------------------------------------+
| [Dev] [Test] [Prod]   Search: [______________] (Filters)     |
+---------------------------------------------------------------+
|  STATUS SUMMARY                                               |
|  ┌────────────┬────────────┬────────────┬──────────────┐     |
|  | Servers    | Services   | Tasks      | Alerts Open   |     |
|  |   32       |   128      |    54      |     6         |     |
|  └────────────┴────────────┴────────────┴──────────────┘     |
+---------------------------------------------------------------+
|  RECENT ALERTS                                                |
|  -----------------------------------------------------------  |
|  [CRITICAL] web-01  IIS app pool stopped   2m ago (View)      |
|  [WARN]     dev-03  Disk 85%               10m ago (View)     |
|  [INFO]     test-02 CPU stabilized          1h ago (View)     |
+---------------------------------------------------------------+
|  SERVER LIST                                                  |
|  -----------------------------------------------------------  |
|  Host         Env   CPU  Mem  Status   Last Heartbeat         |
|  web-01       Prod  23%  65%  OK       2m ago                 |
|  app-02       Test  14%  44%  WARN     5m ago                 |
|  dev-03       Dev   64%  92%  CRIT     10m ago                |
+---------------------------------------------------------------+
```

---

## **2. Server Detail Page**

```
+---------------------------------------------------------------+
| FALCON • Server: web-01                                      |
+---------------------------------------------------------------+
|  [Overview] [Services] [Scheduled Tasks] [IIS] [Logs] [Metrics]|
+---------------------------------------------------------------+
| OVERVIEW                                                      |
| Hostname: web-01        Environment: Prod                     |
| IP: 10.1.2.34           Last Heartbeat: 2m ago                |
+---------------------------------------------------------------+
| METRICS (Last 2h)                                            |
| CPU: [Graph]                                                |
| Mem: [Graph]                                                |
| Disk: C: 18GB free                                           |
+---------------------------------------------------------------+
| ALERTS                                                       |
| [CRITICAL] IIS AppPool crashed (View)                        |
+---------------------------------------------------------------+
```

---

## **3. Services Tab**

```
+---------------------------------------------------------------+
| SERVICES on web-01                                           |
+---------------------------------------------------------------+
| Service Name      Desired   Current   Critical   Actions       |
| ----------------  --------  --------  --------   --------       |
| MyService         Running   Running   Yes        (Restart)     |
| Spooler           Running   Stopped   No         (Start)       |
| SearchIndex       Running   Running   No         (Restart)     |
+---------------------------------------------------------------+
| Service Details Panel (when selected)                         |
| ------------------------------------------------------------- |
| Display Name: MyService                                       |
| Last Change: 10:12                                            |
| Recent Events:                                                |
|   - Running -> Stopped (09:50)                                |
|   - Stopped -> Running (09:55)                                |
+---------------------------------------------------------------+
```

---

## **4. Scheduled Tasks Tab**

```
+---------------------------------------------------------------+
| SCHEDULED TASKS on web-01                                    |
+---------------------------------------------------------------+
| Task Name       Enabled   Last Run       Result     Next Run  |
| --------------  --------  -------------  ---------  ---------- |
| DailyCleanup    Yes       23:00          Success    23:00      |
| SyncMetadata    Yes       03:00          Failed     03:00      |
| BackupDB        No        01:00          Skipped    N/A        |
+---------------------------------------------------------------+
| [Trigger Run]  [View History]                                 |
```

---

## **5. IIS Tab (App Pools + Sites)**

```
+---------------------------------------------------------------+
| IIS on web-01                                                |
+---------------------------------------------------------------+
| APP POOLS                                                    |
| Name           State     Last Recycle     Actions             |
| -------------  --------  --------------   ------------------  |
| MyAppPool      Started   2025-11-30       (Recycle)           |
| ReportsPool    Stopped   2025-11-29       (Start)             |
+---------------------------------------------------------------+
| SITES                                                        |
| Name             Status    Last Check   Endpoint              |
| ---------------  --------  -----------  --------------------- |
| CustomerPortal   Started   OK           /health               |
| AdminPortal      Started   200          /health               |
+---------------------------------------------------------------+
```

---

## **6. Log Search Page**

```
+---------------------------------------------------------------+
| LOG SEARCH                                                   |
+---------------------------------------------------------------+
| Server: [Any ▼]  Time: [Today ▼]  Query: [Exception ...]     |
| [Search]                                                     |
+---------------------------------------------------------------+
| RESULTS                                                      |
| Timestamp        Server     Severity   Message                |
| ---------------- ---------- --------- ------------------------ |
| 10:12:34         web-01     ERROR      Exception in module X  |
| 09:55:14         test-02    CRIT       Out of memory          |
+---------------------------------------------------------------+
| [View Full Log Entry]                                        |
```

---

## **7. Alerts Center**

```
+---------------------------------------------------------------+
| ALERTS CENTER                                                |
+---------------------------------------------------------------+
| Filters: [Open ▼] [Severity ▼] [Environment ▼]               |
+---------------------------------------------------------------+
| Alerts                                                       |
| ------------------------------------------------------------ |
| [CRITICAL] web-01 • IIS AppPool stopped • 2m ago (Ack)       |
| [WARN] test-02 • Disk 85% • 10m ago (Ack)                    |
| [INFO] dev-03 • CPU spike normalized • 1h ago                |
+---------------------------------------------------------------+
| Selected Alert Details                                       |
| ------------------------------------------------------------ |
| Message: IIS AppPool crashed                                 |
| Server: web-01                                               |
| Created: 10:09                                               |
| Status: OPEN                                                 |
| Buttons: [Acknowledge] [Close]                               |
+---------------------------------------------------------------+
```

---

## **8. Admin — Users & Roles**

```
+---------------------------------------------------------------+
| ADMIN • Users & Roles                                        |
+---------------------------------------------------------------+
| USERS                                                        |
| Username        Roles                    Actions              |
| --------------  -----------------------  -------------------- |
| john.doe       admin, operator          (Edit) (Delete)       |
| lisa.smith     viewer                  (Edit) (Delete)        |
+---------------------------------------------------------------+
| ROLES                                                        |
| Role Name       Members   Actions                            |
| --------------  --------  ---------------------------------- |
| admin           4         (View) (Edit)                       |
| operator        7         (View) (Edit)                       |
| viewer          12        (View) (Edit)                       |
+---------------------------------------------------------------+
```
