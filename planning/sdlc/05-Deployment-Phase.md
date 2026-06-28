# Deployment Phase Documentation

## 1. Infrastructure Architecture

### 1.1 MVP Architecture (VPS + Docker Compose)

```
┌─────────────────────────────────────────────────────────────────┐
│                    VPS Infrastructure (MVP)                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                 Caddy / Nginx (Reverse Proxy)               │ │
│  │            (SSL Termination, Static Assets)                 │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                   Docker Compose                             │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │    Frontend     │    │   Backend API   │                │ │
│  │  │    (React)      │    │  (ASP.NET Core) │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  │                                                             │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │   PostgreSQL    │    │      Redis      │                │ │
│  │  │   (Database)    │    │     (Cache)     │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  │                                                             │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │     MinIO       │    │   Prometheus    │                │ │
│  │  │ (File Storage)  │    │   + Grafana     │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  └─────────────────────────────────────────────────────────────┘ │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                    Local Volumes                            │ │
│  │    /data/postgres  /data/redis  /data/minio  /data/logs    │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 1.2 Scale Architecture (Cloud - Future)

```
┌─────────────────────────────────────────────────────────────────┐
│                   Cloud Architecture (Scale)                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                    CDN + Load Balancer                      │ │
│  │              (CloudFlare / Cloud Provider)                  │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                   Kubernetes Cluster                         │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │  Frontend Pods  │    │  Backend Pods   │                │ │
│  │  │   (Replicas)    │    │   (Replicas)    │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  └─────────────────────────┬───────────────────────────────────┘ │
│                            │                                     │
│  ┌─────────────────────────▼───────────────────────────────────┐ │
│  │                   Managed Services                          │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │ Managed Postgres│    │  Managed Redis  │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  │  ┌─────────────────┐    ┌─────────────────┐                │ │
│  │  │   S3-Compatible │    │    OpenSearch   │                │ │
│  │  │  Object Storage │    │    (Optional)   │                │ │
│  │  └─────────────────┘    └─────────────────┘                │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 1.3 Environment Configuration

#### MVP Production Environment (VPS)
```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  caddy:
    image: caddy:2-alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./Caddyfile:/etc/caddy/Caddyfile
      - caddy_data:/data
      - caddy_config:/config
    depends_on:
      - frontend
      - backend
    restart: unless-stopped

  frontend:
    image: ghcr.io/your-org/braavo-frontend:${TAG:-latest}
    expose:
      - "80"
    environment:
      - VITE_API_URL=https://api.braavo.com
    restart: unless-stopped

  backend:
    image: ghcr.io/your-org/braavo-backend:${TAG:-latest}
    expose:
      - "5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=braavo;Username=braavo;Password=${DB_PASSWORD}
      - ConnectionStrings__Redis=redis:6379
      - FileStorage__Type=MinIO
      - FileStorage__Endpoint=minio:9000
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
    restart: unless-stopped

  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: braavo
      POSTGRES_USER: braavo
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - /data/postgres:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U braavo"]
      interval: 10s
      timeout: 5s
      retries: 5
    restart: unless-stopped

  redis:
    image: redis:7-alpine
    command: redis-server --appendonly yes
    volumes:
      - /data/redis:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5
    restart: unless-stopped

  minio:
    image: minio/minio
    command: server /data --console-address ":9001"
    environment:
      MINIO_ROOT_USER: ${MINIO_USER}
      MINIO_ROOT_PASSWORD: ${MINIO_PASSWORD}
    volumes:
      - /data/minio:/data
    expose:
      - "9000"
      - "9001"
    restart: unless-stopped

  prometheus:
    image: prom/prometheus:latest
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
      - /data/prometheus:/prometheus
    expose:
      - "9090"
    restart: unless-stopped

  grafana:
    image: grafana/grafana:latest
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=${GRAFANA_PASSWORD}
    volumes:
      - /data/grafana:/var/lib/grafana
    expose:
      - "3000"
    restart: unless-stopped

volumes:
  caddy_data:
  caddy_config:
```

#### Caddyfile (Reverse Proxy + SSL)
```
# Caddyfile
braavo.com {
    reverse_proxy frontend:80
}

api.braavo.com {
    reverse_proxy backend:5000
}

minio.braavo.com {
    reverse_proxy minio:9001
}

grafana.braavo.com {
    reverse_proxy grafana:3000
}
```

#### VPS Requirements (MVP)
| Resource | Minimum | Recommended |
|----------|---------|-------------|
| CPU | 4 cores | 8 cores |
| RAM | 8 GB | 16 GB |
| Storage | 100 GB SSD | 250 GB SSD |
| Providers | Hetzner, DigitalOcean, Linode, Vultr |

## 2. Containerization

### 2.1 Docker Configuration

#### Frontend Dockerfile
```dockerfile
# src/Frontend/Dockerfile
FROM node:20-alpine AS builder
WORKDIR /build
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Production image
FROM caddy:2-alpine
COPY --from=builder /build/dist /srv
COPY Caddyfile /etc/caddy/Caddyfile
EXPOSE 80
```

#### Frontend Caddyfile
```
# src/Frontend/Caddyfile
:80 {
    root * /srv
    file_server
    
    # SPA fallback - serve index.html for client-side routing
    try_files {path} /index.html
    
    # Cache static assets
    @static path *.js *.css *.png *.jpg *.svg *.woff2
    header @static Cache-Control "public, max-age=31536000, immutable"
    
    # Security headers
    header {
        X-Content-Type-Options nosniff
        X-Frame-Options DENY
        Referrer-Policy strict-origin-when-cross-origin
    }
}
```

#### Backend Dockerfile
```dockerfile
# src/Backend/Braavo.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /build
COPY *.sln .
COPY Braavo.Api/*.csproj Braavo.Api/
COPY Braavo.Core/*.csproj Braavo.Core/
COPY Braavo.Infrastructure/*.csproj Braavo.Infrastructure/
RUN dotnet restore
COPY . .
RUN dotnet publish Braavo.Api -c Release -o /publish

# Production image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /publish .
RUN addgroup --gid 1001 --system dotnet && \
    adduser --uid 1001 --system --gid 1001 dotnet
USER dotnet
EXPOSE 5000
ENTRYPOINT ["dotnet", "Braavo.Api.dll"]
```

### 2.2 Docker Compose for Development
```yaml
# docker-compose.dev.yml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: braavo_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 30s
      timeout: 10s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 30s
      timeout: 10s
      retries: 5

  backend:
    build:
      context: .
      dockerfile: src/Backend/Braavo.Api/Dockerfile.dev
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5000
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=braavo_dev;Username=postgres;Password=password
      - ConnectionStrings__Redis=redis:6379
      - JWT__Secret=dev-secret
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
    volumes:
      - ./src/Backend:/app/src/Backend
    command: dotnet watch run --project src/Backend/Braavo.Api

  frontend:
    build:
      context: .
      dockerfile: src/Frontend/Dockerfile.dev
    ports:
      - "3000:3000"
    environment:
      - VITE_API_URL=http://localhost:5000/api
    depends_on:
      - backend
    volumes:
      - ./src/Frontend:/app/src/Frontend
      - /app/src/Frontend/node_modules
    command: npm run dev

volumes:
  postgres_data:
  redis_data:
```

## 3. CI/CD Pipeline

### 3.1 GitHub Actions Workflow (VPS Deployment)
```yaml
# .github/workflows/deploy.yml
name: Build, Test & Deploy

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  REGISTRY: ghcr.io
  IMAGE_PREFIX: ${{ github.repository }}

jobs:
  test:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: password
          POSTGRES_DB: braavo_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432

      redis:
        image: redis:7-alpine
        ports:
          - 6379:6379

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: src/Frontend/package-lock.json

      - name: Install frontend dependencies
        run: npm ci
        working-directory: ./src/Frontend

      - name: Run frontend linting
        run: npm run lint
        working-directory: ./src/Frontend

      - name: Run frontend type checking
        run: npm run type-check
        working-directory: ./src/Frontend

      - name: Run frontend tests
        run: npm run test
        working-directory: ./src/Frontend

      - name: Restore .NET dependencies
        run: dotnet restore
        working-directory: ./src/Backend

      - name: Run backend tests
        run: dotnet test --collect:"XPlat Code Coverage"
        working-directory: ./src/Backend
        env:
          ConnectionStrings__DefaultConnection: Host=localhost;Database=braavo_test;Username=postgres;Password=password
          ConnectionStrings__Redis: localhost:6379

      - name: Build backend
        run: dotnet publish -c Release -o ./publish
        working-directory: ./src/Backend/Braavo.Api

      - name: Build frontend
        run: npm run build
        working-directory: ./src/Frontend

  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main' || github.ref == 'refs/heads/develop'
    permissions:
      contents: read
      packages: write

    outputs:
      tag: ${{ steps.meta.outputs.version }}

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Log in to GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}
          tags: |
            type=sha,prefix=
            type=ref,event=branch

      - name: Build and push frontend
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./src/Frontend/Dockerfile
          push: true
          tags: ${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}-frontend:${{ steps.meta.outputs.version }}

      - name: Build and push backend
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./src/Backend/Braavo.Api/Dockerfile
          push: true
          tags: ${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}-backend:${{ steps.meta.outputs.version }}

  deploy:
    needs: build-and-push
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    environment: production

    steps:
      - name: Deploy to VPS via SSH
        uses: appleboy/ssh-action@v1.0.3
        with:
          host: ${{ secrets.VPS_HOST }}
          username: ${{ secrets.VPS_USER }}
          key: ${{ secrets.VPS_SSH_KEY }}
          script: |
            cd /opt/braavo
            
            # Pull latest images
            echo ${{ secrets.GITHUB_TOKEN }} | docker login ghcr.io -u ${{ github.actor }} --password-stdin
            docker compose pull
            
            # Deploy with zero-downtime
            docker compose up -d --remove-orphans
            
            # Cleanup old images
            docker image prune -f
            
            # Health check
            sleep 10
            curl -f http://localhost:5000/api/health || exit 1

      - name: Run smoke tests
        run: |
          curl -f https://api.braavo.com/api/health
          curl -f https://braavo.com
```

### 3.2 Deployment Scripts (VPS)
```bash
#!/bin/bash
# scripts/deploy.sh - Deploy to VPS via SSH

set -euo pipefail

VPS_HOST="${VPS_HOST:-braavo.com}"
VPS_USER="${VPS_USER:-deploy}"
DEPLOY_PATH="/opt/braavo"
TAG="${1:-latest}"

echo "Deploying tag '$TAG' to $VPS_HOST..."

# SSH to VPS and deploy
ssh "$VPS_USER@$VPS_HOST" << EOF
  set -euo pipefail
  cd $DEPLOY_PATH

  # Set the image tag
  export TAG=$TAG

  # Pull latest images
  docker compose pull

  # Deploy with zero-downtime rolling update
  docker compose up -d --remove-orphans

  # Wait for services to be healthy
  echo "Waiting for services to be healthy..."
  sleep 10

  # Health checks
  curl -sf http://localhost:5000/api/health || { echo "Backend health check failed"; exit 1; }
  curl -sf http://localhost:80 || { echo "Frontend health check failed"; exit 1; }

  # Cleanup old images
  docker image prune -f

  echo "Deployment completed successfully!"
EOF

# External health check
echo "Running external health checks..."
curl -sf "https://api.$VPS_HOST/api/health" || { echo "External API health check failed"; exit 1; }
curl -sf "https://$VPS_HOST" || { echo "External frontend health check failed"; exit 1; }

echo "All health checks passed!"
```

#### Initial VPS Setup Script
```bash
#!/bin/bash
# scripts/setup-vps.sh - One-time VPS setup

set -euo pipefail

# Install Docker
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER

# Install Docker Compose plugin
sudo apt-get update
sudo apt-get install -y docker-compose-plugin

# Create deployment directory
sudo mkdir -p /opt/braavo
sudo chown $USER:$USER /opt/braavo

# Copy docker-compose.prod.yml and .env
scp docker-compose.prod.yml $VPS_USER@$VPS_HOST:/opt/braavo/docker-compose.yml
scp .env.production $VPS_USER@$VPS_HOST:/opt/braavo/.env
scp Caddyfile $VPS_USER@$VPS_HOST:/opt/braavo/Caddyfile

# Create data directories
sudo mkdir -p /data/{postgres,redis,minio,prometheus,grafana}
sudo chown -R 1000:1000 /data

echo "VPS setup complete. Run ./scripts/deploy.sh to deploy."
```

## 4. Database Migration

### 4.1 Migration Strategy
```bash
#!/bin/bash
# scripts/migrate.sh
set -euo pipefail

ENVIRONMENT="${1:-staging}"
DRY_RUN="${DRY_RUN:-false}"
BACKUP_DIR="${BACKUP_DIR:-backups}"
TIMESTAMP="$(date -u +%Y%m%dT%H%M%SZ)"
BACKUP_NAME="braavo-${ENVIRONMENT}-backup-${TIMESTAMP}.sql"

echo "Running EF Core migrations for ${ENVIRONMENT} environment..."

if [ "$DRY_RUN" = "true" ]; then
  dotnet ef migrations list \
    --project src/Backend/Braavo.Infrastructure \
    --startup-project src/Backend/Braavo.Api
  exit 0
fi

mkdir -p "$BACKUP_DIR"
pg_dump "$DATABASE_URL" > "$BACKUP_DIR/$BACKUP_NAME"

if ! dotnet ef database update \
  --project src/Backend/Braavo.Infrastructure \
  --startup-project src/Backend/Braavo.Api; then
  echo "EF Core migration failed"

  if [ "$ENVIRONMENT" = "production" ]; then
    echo "Restoring production database from backup..."
    psql "$DATABASE_URL" < "$BACKUP_DIR/$BACKUP_NAME"
  fi

  exit 1
fi

echo "EF Core migrations completed successfully"
```

## 5. Environment Variables

### 5.1 Environment Configuration
```bash
# .env.production
NODE_ENV=production
PORT=3001

# Database
DATABASE_URL=postgresql://username:password@prod-db.amazonaws.com:5432/braavo
DATABASE_SSL=true
DATABASE_POOL_SIZE=20

# Redis
REDIS_URL=redis://prod-redis.amazonaws.com:6379
REDIS_CLUSTER_MODE=true

# JWT
JWT_SECRET=super-secret-production-key
JWT_EXPIRATION=24h

# AWS
AWS_REGION=us-east-1
AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE
AWS_SECRET_ACCESS_KEY=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY
AWS_S3_BUCKET=braavo-prod-assets

# OpenAI
OPENAI_API_KEY=sk-example-key
OPENAI_MODEL=gpt-4
OPENAI_MAX_TOKENS=4000

# External APIs
FIGMA_CLIENT_ID=figma-client-id
FIGMA_CLIENT_SECRET=figma-client-secret
GITHUB_CLIENT_ID=github-client-id
GITHUB_CLIENT_SECRET=github-client-secret

# Monitoring
SENTRY_DSN=https://example@sentry.io/123456
NEW_RELIC_LICENSE_KEY=example-key
LOG_LEVEL=info

# Email
SMTP_HOST=smtp.sendgrid.net
SMTP_PORT=587
SMTP_USER=apikey
SMTP_PASS=sendgrid-api-key
FROM_EMAIL=noreply@braavo.com

# Security
CORS_ORIGIN=https://braavo.com
RATE_LIMIT_WINDOW=15
RATE_LIMIT_MAX=100
```

### 5.2 Secrets Management
```yaml
# kubernetes/secrets.yaml
apiVersion: v1
kind: Secret
metadata:
  name: braavo-secrets
  namespace: braavo
type: Opaque
data:
  database-url: <base64-encoded-value>
  jwt-secret: <base64-encoded-value>
  openai-api-key: <base64-encoded-value>
  aws-access-key-id: <base64-encoded-value>
  aws-secret-access-key: <base64-encoded-value>
  figma-client-secret: <base64-encoded-value>
  github-client-secret: <base64-encoded-value>
  smtp-password: <base64-encoded-value>
```

## 6. Monitoring and Logging

### 6.1 Application Monitoring
```typescript
// src/middleware/monitoring.ts
import { Request, Response, NextFunction } from 'express';
import { Counter, Histogram, register } from 'prom-client';

const httpRequestsTotal = new Counter({
  name: 'http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status_code']
});

const httpRequestDuration = new Histogram({
  name: 'http_request_duration_seconds',
  help: 'Duration of HTTP requests in seconds',
  labelNames: ['method', 'route', 'status_code'],
  buckets: [0.1, 0.5, 1, 2, 5, 10]
});

export const metricsMiddleware = (req: Request, res: Response, next: NextFunction) => {
  const start = Date.now();

  res.on('finish', () => {
    const duration = (Date.now() - start) / 1000;
    const route = req.route?.path || req.path;

    httpRequestsTotal.labels(req.method, route, res.statusCode.toString()).inc();
    httpRequestDuration.labels(req.method, route, res.statusCode.toString()).observe(duration);
  });

  next();
};

export const metricsEndpoint = (req: Request, res: Response) => {
  res.set('Content-Type', register.contentType);
  res.end(register.metrics());
};
```

### 6.2 Logging Configuration
```typescript
// src/utils/logger.ts
import winston from 'winston';
import { CloudWatchLogsClient } from '@aws-sdk/client-cloudwatch-logs';

const logger = winston.createLogger({
  level: process.env.LOG_LEVEL || 'info',
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.errors({ stack: true }),
    winston.format.json()
  ),
  defaultMeta: {
    service: 'braavo-api',
    environment: process.env.NODE_ENV,
    version: process.env.APP_VERSION
  },
  transports: [
    new winston.transports.File({ filename: 'logs/error.log', level: 'error' }),
    new winston.transports.File({ filename: 'logs/combined.log' })
  ]
});

if (process.env.NODE_ENV !== 'production') {
  logger.add(new winston.transports.Console({
    format: winston.format.simple()
  }));
}

if (process.env.NODE_ENV === 'production') {
  // Add CloudWatch transport for production
  logger.add(new winston.transports.Console({
    format: winston.format.json()
  }));
}

export { logger };
```

## 7. Health Checks and Monitoring

### 7.1 Health Check Endpoints
```typescript
// src/routes/health.ts
import { Router } from 'express';
import { prisma } from '../database/prisma';
import { redis } from '../database/redis';

const router = Router();

interface HealthCheck {
  status: 'healthy' | 'unhealthy';
  timestamp: string;
  version: string;
  uptime: number;
  services: {
    database: 'healthy' | 'unhealthy';
    redis: 'healthy' | 'unhealthy';
    external_apis: 'healthy' | 'unhealthy';
  };
}

router.get('/health', async (req, res) => {
  const healthCheck: HealthCheck = {
    status: 'healthy',
    timestamp: new Date().toISOString(),
    version: process.env.APP_VERSION || '1.0.0',
    uptime: process.uptime(),
    services: {
      database: 'healthy',
      redis: 'healthy',
      external_apis: 'healthy'
    }
  };

  try {
    // Check database connection
    await prisma.$queryRaw`SELECT 1`;
  } catch (error) {
    healthCheck.services.database = 'unhealthy';
    healthCheck.status = 'unhealthy';
  }

  try {
    // Check Redis connection
    await redis.ping();
  } catch (error) {
    healthCheck.services.redis = 'unhealthy';
    healthCheck.status = 'unhealthy';
  }

  try {
    // Check external APIs
    const openaiResponse = await fetch('https://api.openai.com/v1/models', {
      headers: {
        'Authorization': `Bearer ${process.env.OPENAI_API_KEY}`
      }
    });

    if (!openaiResponse.ok) {
      throw new Error('OpenAI API not responding');
    }
  } catch (error) {
    healthCheck.services.external_apis = 'unhealthy';
    healthCheck.status = 'unhealthy';
  }

  const statusCode = healthCheck.status === 'healthy' ? 200 : 503;
  res.status(statusCode).json(healthCheck);
});

router.get('/ready', async (req, res) => {
  // Readiness check - more thorough than health check
  try {
    await prisma.$queryRaw`SELECT 1`;
    await redis.ping();

    res.status(200).json({
      status: 'ready',
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    res.status(503).json({
      status: 'not ready',
      timestamp: new Date().toISOString(),
      error: error.message
    });
  }
});

export { router as healthRouter };
```

### 7.2 Alerting Configuration
```yaml
# monitoring/alerts.yml
groups:
  - name: braavo-alerts
    rules:
      - alert: HighErrorRate
        expr: rate(http_requests_total{status_code=~"5.."}[5m]) > 0.1
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "High error rate detected"
          description: "Error rate is above 10% for 5 minutes"

      - alert: HighResponseTime
        expr: histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m])) > 2
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High response time detected"
          description: "95th percentile response time is above 2 seconds"

      - alert: DatabaseDown
        expr: up{job="postgres"} == 0
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "Database is down"
          description: "PostgreSQL database is not responding"

      - alert: RedisDown
        expr: up{job="redis"} == 0
        for: 1m
        labels:
          severity: warning
        annotations:
          summary: "Redis is down"
          description: "Redis cache is not responding"

      - alert: HighMemoryUsage
        expr: container_memory_usage_bytes / container_spec_memory_limit_bytes > 0.8
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High memory usage"
          description: "Memory usage is above 80%"

      - alert: LowDiskSpace
        expr: node_filesystem_avail_bytes / node_filesystem_size_bytes < 0.1
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "Low disk space"
          description: "Disk space is below 10%"
```

This comprehensive deployment documentation provides everything needed to deploy and maintain the Braavo platform in production environments, including infrastructure setup, containerization, CI/CD pipelines, monitoring, and alerting.
