openapi: 3.1.0
info:
  title: FALCON Monitoring API
  version: "1.0.0"
  description: |
    FALCON is a Windows servers monitoring platform that tracks server health,
    windows services, scheduled tasks, IIS app pools/sites, log files, metrics and alerts.
    Base path is /api/v1. Timestamps use ISO-8601 UTC.
servers:
  - url: /api/v1
    description: Local / reverse-proxy base path for API v1

tags:
  - name: auth
    description: Authentication and current user
  - name: servers
    description: Server inventory and per-server endpoints
  - name: services
    description: Windows service monitoring
  - name: tasks
    description: Scheduled tasks and run history
  - name: iis
    description: IIS app pools and sites
  - name: logs
    description: Log file ingestion and search
  - name: alerts
    description: Alerts and notifications
  - name: metrics
    description: Metrics time-series
  - name: collectors
    description: Collectors / Agents
  - name: admin
    description: Administration endpoints

security:
  - bearerAuth: []

components:
  securitySchemes:
    bearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT
    oauth2:
      type: oauth2
      flows:
        authorizationCode:
          authorizationUrl: https://auth.example.com/oauth2/authorize
          tokenUrl: https://auth.example.com/oauth2/token
          scopes:
            openid: OpenID Connect scope
            profile: Access profile information
            email: Access email
  schemas:
    # Common primitives
    UUID:
      type: string
      format: uuid
      description: UUID v4 identifier
    Timestamp:
      type: string
      format: date-time
      description: ISO-8601 timestamp in UTC

    # Auth / User
    User:
      type: object
      required: [id, username, email, roles]
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        username:
          type: string
        displayName:
          type: string
        email:
          type: string
          format: email
        roles:
          type: array
          items:
            type: string
        createdAt:
          $ref: '#/components/schemas/Timestamp'

    TokenResponse:
      type: object
      properties:
        access_token:
          type: string
        token_type:
          type: string
        expires_in:
          type: integer
        refresh_token:
          type: string
        scope:
          type: string

    # Server model
    ServerSummary:
      type: object
      required: [id, hostname, environment, status]
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        hostname:
          type: string
        displayName:
          type: string
        environment:
          type: string
          description: dev/test/prod
        ipAddress:
          type: string
        os:
          type: string
        lastHeartbeat:
          $ref: '#/components/schemas/Timestamp'
        status:
          type: string
          enum: [healthy, warning, down, unknown]
        cpu:
          type: number
          format: float
          description: CPU usage percent (approx)
        memoryPercent:
          type: number
          format: float
        tags:
          type: array
          items:
            type: string

    ServerDetail:
      allOf:
        - $ref: '#/components/schemas/ServerSummary'
        - type: object
          properties:
            metrics:
              type: object
              properties:
                cpuPercent:
                  type: number
                memoryPercent:
                  type: number
                disks:
                  type: array
                  items:
                    type: object
                    properties:
                      drive:
                        type: string
                      freeGB:
                        type: number
            recentAlerts:
              type: array
              items:
                $ref: '#/components/schemas/Alert'
            serviceSummary:
              type: object
              properties:
                total:
                  type: integer
                failed:
                  type: integer
            tasksSummary:
              type: object
              properties:
                total:
                  type: integer
                failed:
                  type: integer
            iisSummary:
              type: object
              properties:
                appPoolsHealthy:
                  type: integer
                sitesHealthy:
                  type: integer

    # Services
    Service:
      type: object
      required: [id, serviceName, desiredState, currentState]
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        serverId:
          $ref: '#/components/schemas/UUID'
        serviceName:
          type: string
        displayName:
          type: string
        desiredState:
          type: string
          enum: [running, stopped]
        currentState:
          type: string
          enum: [running, stopped, paused, unknown]
        critical:
          type: boolean
        lastChange:
          $ref: '#/components/schemas/Timestamp'

    ServiceEvent:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        serviceId:
          $ref: '#/components/schemas/UUID'
        state:
          type: string
        message:
          type: string
        eventTime:
          $ref: '#/components/schemas/Timestamp'

    # Tasks
    ScheduledTask:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        serverId:
          $ref: '#/components/schemas/UUID'
        taskName:
          type: string
        scheduleDesc:
          type: string
        isEnabled:
          type: boolean
        lastRunTime:
          $ref: '#/components/schemas/Timestamp'
        lastRunResult:
          type: string
        nextRunTime:
          $ref: '#/components/schemas/Timestamp'
        createdAt:
          $ref: '#/components/schemas/Timestamp'

    TaskRun:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        scheduledTaskId:
          $ref: '#/components/schemas/UUID'
        startTime:
          $ref: '#/components/schemas/Timestamp'
        endTime:
          $ref: '#/components/schemas/Timestamp'
        result:
          type: string
          enum: [success, failure, timeout, cancelled, unknown]
        exitCode:
          type: integer
        output:
          type: string

    # IIS
    AppPool:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        serverId:
          $ref: '#/components/schemas/UUID'
        name:
          type: string
        state:
          type: string
          enum: [Started, Stopped, Unknown]
        lastRecycle:
          $ref: '#/components/schemas/Timestamp'

    IISSite:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        serverId:
          $ref: '#/components/schemas/UUID'
        name:
          type: string
        status:
          type: string
          enum: [Started, Stopped]
        bindings:
          type: object
        pingEndpoint:
          type: string
        lastHttpStatus:
          type: integer
        lastChecked:
          $ref: '#/components/schemas/Timestamp'

    # Logs
    LogFile:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        serverId:
          $ref: '#/components/schemas/UUID'
        path:
          type: string
        parser:
          type: string
        createdAt:
          $ref: '#/components/schemas/Timestamp'

    LogEntry:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        logFileId:
          $ref: '#/components/schemas/UUID'
        serverId:
          $ref: '#/components/schemas/UUID'
        timestamp:
          $ref: '#/components/schemas/Timestamp'
        severity:
          type: string
          enum: [TRACE, DEBUG, INFO, WARNING, ERROR, CRITICAL]
        message:
          type: string
        jsonPayload:
          type: object

    LogPattern:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        name:
          type: string
        pattern:
          type: string
        severityDefault:
          type: string
        enabled:
          type: boolean

    # Alerts & notifications
    Alert:
      type: object
      required: [id, alertType, severity, status, createdAt]
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        serverId:
          $ref: '#/components/schemas/UUID'
        sourceType:
          type: string
          description: service | task | iis | log | metric
        sourceId:
          $ref: '#/components/schemas/UUID'
        alertType:
          type: string
        severity:
          type: string
          enum: [info, warning, critical]
        status:
          type: string
          enum: [open, acknowledged, closed]
        message:
          type: string
        createdAt:
          $ref: '#/components/schemas/Timestamp'
        resolvedAt:
          $ref: '#/components/schemas/Timestamp'
        relatedLogs:
          type: array
          items:
            $ref: '#/components/schemas/LogEntry'

    Notification:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        alertId:
          $ref: '#/components/schemas/UUID'
        channel:
          type: string
          enum: [email, teams, slack, webhook]
        recipient:
          type: string
        status:
          type: string
          enum: [pending, sent, failed]
        attemptCount:
          type: integer
        lastAttempt:
          $ref: '#/components/schemas/Timestamp'
        payload:
          type: object

    MetricPoint:
      type: object
      properties:
        id:
          type: integer
          format: int64
        serverId:
          $ref: '#/components/schemas/UUID'
        metricName:
          type: string
        metricValue:
          type: number
        measuredAt:
          $ref: '#/components/schemas/Timestamp'

    Collector:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        name:
          type: string
        type:
          type: string
          enum: [agent, winrm, powershell, hybrid]
        config:
          type: object
        lastSeen:
          $ref: '#/components/schemas/Timestamp'
        createdAt:
          $ref: '#/components/schemas/Timestamp'

    Role:
      type: object
      properties:
        id:
          $ref: '#/components/schemas/UUID'
        name:
          type: string
        description:
          type: string

    PagedServers:
      type: object
      properties:
        total:
          type: integer
        items:
          type: array
          items:
            $ref: '#/components/schemas/ServerSummary'

    PagedLogEntries:
      type: object
      properties:
        total:
          type: integer
        items:
          type: array
          items:
            $ref: '#/components/schemas/LogEntry'

    Error:
      type: object
      properties:
        code:
          type: integer
        message:
          type: string

  parameters:
    serverId:
      name: serverId
      in: path
      required: true
      schema:
        $ref: '#/components/schemas/UUID'
    serviceId:
      name: serviceId
      in: path
      required: true
      schema:
        $ref: '#/components/schemas/UUID'
    taskId:
      name: taskId
      in: path
      required: true
      schema:
        $ref: '#/components/schemas/UUID'
    appPoolId:
      name: appPoolId
      in: path
      required: true
      schema:
        $ref: '#/components/schemas/UUID'
    alertId:
      name: alertId
      in: path
      required: true
      schema:
        $ref: '#/components/schemas/UUID'
    notificationId:
      name: notificationId
      in: path
      required: true
      schema:
        $ref: '#/components/schemas/UUID'
    collectorId:
      name: collectorId
      in: path
      required: true
      schema:
        $ref: '#/components/schemas/UUID'

paths:
  /auth/me:
    get:
      tags: [auth]
      summary: Get authenticated user's profile and roles
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Authenticated user profile
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'
        '401':
          description: Unauthorized
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Error'

  /servers:
    get:
      tags: [servers]
      summary: List servers
      parameters:
        - in: query
          name: environment
          schema:
            type: string
            enum: [dev, test, prod]
          description: Filter by environment
        - in: query
          name: search
          schema:
            type: string
          description: Search by hostname or display name
        - in: query
          name: page
          schema:
            type: integer
            default: 1
        - in: query
          name: pageSize
          schema:
            type: integer
            default: 50
      security:
        - bearerAuth: []
      responses:
        '200':
          description: List of servers (paged)
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/PagedServers'
    post:
      tags: [servers, admin]
      summary: Register a server
      security:
        - bearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [hostname, environment]
              properties:
                hostname:
                  type: string
                displayName:
                  type: string
                environment:
                  type: string
                ipAddress:
                  type: string
                collectorId:
                  $ref: '#/components/schemas/UUID'
      responses:
        '201':
          description: Server created
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ServerDetail'
        '400':
          description: Validation error
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Error'

  /servers/{serverId}:
    parameters:
      - $ref: '#/components/parameters/serverId'
    get:
      tags: [servers]
      summary: Get server details
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Server details
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ServerDetail'
        '404':
          description: Not found
    put:
      tags: [servers, admin]
      summary: Update server metadata
      security:
        - bearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                displayName:
                  type: string
                environment:
                  type: string
                ipAddress:
                  type: string
                isManaged:
                  type: boolean
                tags:
                  type: array
                  items:
                    type: string
      responses:
        '200':
          description: Updated server
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ServerDetail'
    delete:
      tags: [servers, admin]
      summary: Delete (soft) a server
      security:
        - bearerAuth: []
      responses:
        '204':
          description: Deleted

  /servers/{serverId}/actions/restart-service:
    post:
      tags: [servers, services]
      summary: Restart a service on the server by service name
      security:
        - bearerAuth: []
      parameters:
        - $ref: '#/components/parameters/serverId'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [serviceName]
              properties:
                serviceName:
                  type: string
                reason:
                  type: string
      responses:
        '202':
          description: Restart scheduled
          content:
            application/json:
              schema:
                type: object
                properties:
                  actionId:
                    $ref: '#/components/schemas/UUID'
                  scheduledAt:
                    $ref: '#/components/schemas/Timestamp'
        '403':
          description: Forbidden

  /servers/{serverId}/services:
    get:
      tags: [services]
      summary: List monitored services on a server
      security:
        - bearerAuth: []
      parameters:
        - $ref: '#/components/parameters/serverId'
      responses:
        '200':
          description: List of services
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Service'
    post:
      tags: [services, admin]
      summary: Register a service to monitor on a server
      security:
        - bearerAuth: []
      parameters:
        - $ref: '#/components/parameters/serverId'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [serviceName]
              properties:
                serviceName:
                  type: string
                desiredState:
                  type: string
                  enum: [running, stopped]
                critical:
                  type: boolean
      responses:
        '201':
          description: Service registered
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Service'

  /services/{serviceId}:
    parameters:
      - $ref: '#/components/parameters/serviceId'
    get:
      tags: [services]
      summary: Get service details and latest events
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Service detail
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Service'
    delete:
      tags: [services, admin]
      summary: Unregister service
      security:
        - bearerAuth: []
      responses:
        '204':
          description: Deleted

  /services/{serviceId}/events:
    parameters:
      - $ref: '#/components/parameters/serviceId'
    get:
      tags: [services]
      summary: Get service state change history
      security:
        - bearerAuth: []
      parameters:
        - in: query
          name: limit
          schema:
            type: integer
            default: 50
      responses:
        '200':
          description: Events list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/ServiceEvent'

  /services/{serviceId}/actions/restart:
    post:
      tags: [services]
      summary: Restart the service
      security:
        - bearerAuth: []
      parameters:
        - $ref: '#/components/parameters/serviceId'
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                reason:
                  type: string
      responses:
        '202':
          description: Restart queued
          content:
            application/json:
              schema:
                type: object
                properties:
                  actionId:
                    $ref: '#/components/schemas/UUID'
        '403':
          description: Forbidden
        '404':
          description: Service not found

  /servers/{serverId}/tasks:
    get:
      tags: [tasks]
      summary: List scheduled tasks for a server
      parameters:
        - $ref: '#/components/parameters/serverId'
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Tasks list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/ScheduledTask'

  /tasks/{taskId}:
    parameters:
      - $ref: '#/components/parameters/taskId'
    get:
      tags: [tasks]
      summary: Get scheduled task details
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Task detail
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ScheduledTask'
    delete:
      tags: [tasks, admin]
      summary: Remove scheduled task from monitoring
      security:
        - bearerAuth: []
      responses:
        '204':
          description: Deleted

  /tasks/{taskId}/runs:
    parameters:
      - $ref: '#/components/parameters/taskId'
    get:
      tags: [tasks]
      summary: Get run history for a scheduled task
      security:
        - bearerAuth: []
      parameters:
        - in: query
          name: limit
          schema:
            type: integer
            default: 20
      responses:
        '200':
          description: Run history
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/TaskRun'

  /tasks/{taskId}/trigger:
    post:
      tags: [tasks]
      summary: Trigger a scheduled task manually
      security:
        - bearerAuth: []
      parameters:
        - $ref: '#/components/parameters/taskId'
      responses:
        '202':
          description: Task triggered
          content:
            application/json:
              schema:
                type: object
                properties:
                  runId:
                    $ref: '#/components/schemas/UUID'
                  startTime:
                    $ref: '#/components/schemas/Timestamp'
                  status:
                    type: string
                    enum: [queued, running, failed]
        '404':
          description: Task not found

  /servers/{serverId}/iis/app-pools:
    get:
      tags: [iis]
      summary: List app pools for a server
      parameters:
        - $ref: '#/components/parameters/serverId'
      security:
        - bearerAuth: []
      responses:
        '200':
          description: App pools list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/AppPool'

  /iis/app-pools/{appPoolId}/actions/recycle:
    post:
      tags: [iis]
      summary: Recycle an IIS application pool
      parameters:
        - $ref: '#/components/parameters/appPoolId'
      security:
        - bearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                reason:
                  type: string
      responses:
        '202':
          description: Recycle scheduled
          content:
            application/json:
              schema:
                type: object
                properties:
                  actionId:
                    $ref: '#/components/schemas/UUID'

  /servers/{serverId}/iis/sites:
    get:
      tags: [iis]
      summary: List IIS sites for server with status and health endpoint
      parameters:
        - $ref: '#/components/parameters/serverId'
      security:
        - bearerAuth: []
      responses:
        '200':
          description: List sites
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/IISSite'

  /logs/search:
    post:
      tags: [logs]
      summary: Search logs across servers and files
      security:
        - bearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                serverIds:
                  type: array
                  items:
                    $ref: '#/components/schemas/UUID'
                from:
                  $ref: '#/components/schemas/Timestamp'
                to:
                  $ref: '#/components/schemas/Timestamp'
                q:
                  type: string
                severity:
                  type: array
                  items:
                    type: string
                page:
                  type: integer
                  default: 1
                pageSize:
                  type: integer
                  default: 50
      responses:
        '200':
          description: Search results
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/PagedLogEntries'
        '400':
          description: Bad request

  /logs/stream:
    get:
      tags: [logs]
      summary: Tail a log file (SSE/WebSocket recommended)
      parameters:
        - in: query
          name: logFileId
          required: true
          schema:
            $ref: '#/components/schemas/UUID'
        - in: query
          name: from
          schema:
            $ref: '#/components/schemas/Timestamp'
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Streaming log entries (SSE or WebSocket)

  /log-patterns:
    get:
      tags: [logs, admin]
      summary: Get registered log patterns/rules
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Patterns list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/LogPattern'
    post:
      tags: [logs, admin]
      summary: Create a log pattern
      security:
        - bearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/LogPattern'
      responses:
        '201':
          description: Created

  /alerts:
    get:
      tags: [alerts]
      summary: List alerts with filters
      security:
        - bearerAuth: []
      parameters:
        - in: query
          name: status
          schema:
            type: string
            enum: [open, acknowledged, closed]
        - in: query
          name: severity
          schema:
            type: string
            enum: [info, warning, critical]
        - in: query
          name: serverId
          schema:
            $ref: '#/components/schemas/UUID'
        - in: query
          name: sourceType
          schema:
            type: string
      responses:
        '200':
          description: Alerts list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Alert'
    post:
      tags: [alerts, admin]
      summary: Manually create an alert (admin)
      security:
        - bearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              type: object
              required: [alertType, severity]
              properties:
                serverId:
                  $ref: '#/components/schemas/UUID'
                sourceType:
                  type: string
                sourceId:
                  $ref: '#/components/schemas/UUID'
                alertType:
                  type: string
                severity:
                  type: string
                message:
                  type: string
      responses:
        '201':
          description: Alert created
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Alert'

  /alerts/{alertId}:
    parameters:
      - $ref: '#/components/parameters/alertId'
    get:
      tags: [alerts]
      summary: Get alert detail
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Alert detail (with related logs and notifications)
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Alert'
    post:
      tags: [alerts]
      summary: (Reserved) update alert - not recommended
      security:
        - bearerAuth: []

  /alerts/{alertId}/ack:
    post:
      tags: [alerts]
      summary: Acknowledge an alert
      parameters:
        - $ref: '#/components/parameters/alertId'
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                note:
                  type: string
                userId:
                  $ref: '#/components/schemas/UUID'
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Acknowledged
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Alert'

  /alerts/{alertId}/close:
    post:
      tags: [alerts]
      summary: Close / resolve an alert
      parameters:
        - $ref: '#/components/parameters/alertId'
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                resolution:
                  type: string
                userId:
                  $ref: '#/components/schemas/UUID'
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Closed
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Alert'

  /alerts/{alertId}/notifications:
    parameters:
      - $ref: '#/components/parameters/alertId'
    get:
      tags: [alerts, notifications]
      summary: List notifications sent for an alert
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Notifications list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Notification'

  /alerts/{alertId}/notifications/{notificationId}/resend:
    post:
      tags: [alerts, notifications]
      summary: Resend a notification
      parameters:
        - $ref: '#/components/parameters/alertId'
        - $ref: '#/components/parameters/notificationId'
      security:
        - bearerAuth: []
      responses:
        '202':
          description: Resend queued

  /notification-channels:
    get:
      tags: [admin]
      summary: List notification channels (SMTP, Slack, Teams)
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Channels list
    post:
      tags: [admin]
      summary: Create or update a notification channel
      security:
        - bearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                type:
                  type: string
                  enum: [smtp, slack, teams, webhook]
                config:
                  type: object
      responses:
        '201':
          description: Channel created

  /servers/{serverId}/metrics:
    get:
      tags: [metrics]
      summary: Query time-series metrics for a server
      parameters:
        - $ref: '#/components/parameters/serverId'
        - in: query
          name: metric
          schema:
            type: string
            description: cpuPercent, memoryPercent, disk.free, etc.
        - in: query
          name: from
          schema:
            $ref: '#/components/schemas/Timestamp'
        - in: query
          name: to
          schema:
            $ref: '#/components/schemas/Timestamp'
        - in: query
          name: interval
          schema:
            type: string
            description: Aggregation interval (1m,5m,1h)
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Metric series
          content:
            application/json:
              schema:
                type: object
                properties:
                  metric:
                    type: string
                  points:
                    type: array
                    items:
                      type: object
                      properties:
                        timestamp:
                          $ref: '#/components/schemas/Timestamp'
                        value:
                          type: number

  /collectors:
    get:
      tags: [collectors, admin]
      summary: List registered collectors
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Collectors
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Collector'
    post:
      tags: [collectors]
      summary: Register a collector (agent identifies itself)
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [name, type]
              properties:
                name:
                  type: string
                type:
                  type: string
                config:
                  type: object
      responses:
        '201':
          description: Collector registered
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Collector'

  /collectors/{collectorId}/heartbeat:
    post:
      tags: [collectors]
      summary: Collector heartbeat with health and last seen
      parameters:
        - $ref: '#/components/parameters/collectorId'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                lastSeen:
                  $ref: '#/components/schemas/Timestamp'
                health:
                  type: object
      security:
        - bearerAuth: []

  /collectors/{collectorId}/push/service-events:
    post:
      tags: [collectors]
      summary: Collector pushes service events (batch)
      parameters:
        - $ref: '#/components/parameters/collectorId'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: array
              items:
                type: object
                required: [serviceName, state, timestamp]
                properties:
                  serviceName:
                    type: string
                  state:
                    type: string
                  message:
                    type: string
                  timestamp:
                    $ref: '#/components/schemas/Timestamp'
      security:
        - bearerAuth: []
      responses:
        '202':
          description: Accepted

  /collectors/{collectorId}/push/logs:
    post:
      tags: [collectors, logs]
      summary: Collector pushes parsed log entries
      parameters:
        - $ref: '#/components/parameters/collectorId'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: array
              items:
                $ref: '#/components/schemas/LogEntry'
      security:
        - bearerAuth: []
      responses:
        '202':
          description: Logs accepted

  /admin/users:
    get:
      tags: [admin]
      summary: List users
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Users list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/User'
    post:
      tags: [admin]
      summary: Create a new user
      security:
        - bearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [username, email]
              properties:
                username:
                  type: string
                displayName:
                  type: string
                email:
                  type: string
      responses:
        '201':
          description: User created
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'

  /admin/roles:
    get:
      tags: [admin]
      summary: List roles
      security:
        - bearerAuth: []
      responses:
        '200':
          description: Roles list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Role'
    post:
      tags: [admin]
      summary: Create role
      security:
        - bearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/Role'
      responses:
        '201':
          description: Created

  /maintenance-windows:
    get:
      tags: [admin]
      summary: List maintenance windows
      security:
        - bearerAuth: []
      responses:
        '200':
          description: List
    post:
      tags: [admin]
      summary: Create maintenance window
      security:
        - bearerAuth: []
      requestBody:
        content:
          application/json:
            schema:
              type: object
              properties:
                name:
                  type: string
                startTime:
                  $ref: '#/components/schemas/Timestamp'
                endTime:
                  $ref: '#/components/schemas/Timestamp'
                servers:
                  type: array
                  items:
                    $ref: '#/components/schemas/UUID'
      responses:
        '201':
          description: Created

# Example global responses
components:
  responses:  {}   # Placeholder to avoid duplication in this file

# End of OpenAPI document
